using System.ComponentModel.DataAnnotations;

namespace GerberBackend.Core.Dtos.Auth;

public class LoginDto
{
    [Required(ErrorMessage = "Введите имя пользователя")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; }
}
