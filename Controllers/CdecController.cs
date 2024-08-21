using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CdekService.Models;

namespace CdekService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CdekController : ControllerBase
    {
        private readonly string _login = "wqGwiQx0gg8mLtiEKsUinjVSICCjtTEP";
        private readonly string _secret = "RmAmgvSgSl1yirlz9QupbzOJVqhCxcP5";
        private readonly string _baseUrl = "https://api.edu.cdek.ru/v2";
        private readonly HttpClient _httpClient;

        private string _authToken;

        public CdekController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Process([FromQuery] string action, [FromQuery] int? size = null, [FromBody] JsonElement? body = null)
        {
            if (string.IsNullOrEmpty(action))
            {
                return BadRequest(new { message = "Action is required" });
            }

            await GetAuthTokenAsync();

            string response;
            switch (action.ToLower())
            {
                case "offices":
                    response = await GetOffices(size);
                    break;
                case "calculate":
                    response = await Calculate(body);
                    break;
                default:
                    return BadRequest(new { message = "Unknown action" });
            }

            return Content(response, "application/json");
        }

        [HttpPost("order")]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest(new { message = "Order data is required" });
            }

            await GetAuthTokenAsync();

            var orderJson = JsonSerializer.Serialize(order);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/orders")
            {
                Content = new StringContent(orderJson, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_authToken}");
            request.Headers.Add("X-App-Name", "widget_pvz");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Content(responseContent, "application/json");
            }
            else
            {
                return StatusCode((int)response.StatusCode, new { message = responseContent });
            }
        }

        private async Task GetAuthTokenAsync()
        {
            var requestContent = new StringContent(
                $"grant_type=client_credentials&client_id={_login}&client_secret={_secret}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync($"{_baseUrl}/oauth/token", requestContent);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(responseBody);

            if (tokenData.TryGetProperty("access_token", out var token))
            {
                _authToken = token.GetString();
            }
            else
            {
                throw new Exception("Access token not found in the response.");
            }
        }

        private async Task<string> GetOffices(int? size)
        {
            var url = $"{_baseUrl}/deliverypoints";
            if (size.HasValue)
            {
                url += $"?size={size.Value}";
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {_authToken}");
            request.Headers.Add("X-App-Name", "widget_pvz");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> Calculate(JsonElement? body)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/calculator/tarifflist")
            {
                Content = new StringContent(body?.ToString() ?? string.Empty, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_authToken}");
            request.Headers.Add("X-App-Name", "widget_pvz");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}