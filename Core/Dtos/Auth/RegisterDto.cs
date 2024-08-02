using System.ComponentModel.DataAnnotations;

namespace GerberBackend.Core.Dtos.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "Введите ваше имя!")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Введите ваше отчество!")]
    public string SecondName { get; set; }

    [Required(ErrorMessage = "Введите вашу фамилию!")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Введите имя пользователя!")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Введите номер телефона")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Введите ваш Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; }

    public string City { get; set; }
}
