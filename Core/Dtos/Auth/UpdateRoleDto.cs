using GerberBackend.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace GerberBackend.Core.Dtos.Auth;

public class UpdateRoleDto
{
    [Required(ErrorMessage = "Введите имя пользователя")]
    public string UserName { get; set; }

    public RoleType NewRole { get; set; }
}
