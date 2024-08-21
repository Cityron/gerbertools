using MongoDB.Entities;

namespace GerberBackend.Core.MongoDB.Entities;



public class ErrorLogClient : Entity
{
    public string ErrorMessage { get; set; }
    public string UserId { get; set; }
    public DateTime Timestamp { get; set; }
}
