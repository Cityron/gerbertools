using GerberBackend.Core.Entities.Gerber;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerberBackend.Core.Entities.Auth;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }

    public string SecondName { get; set; }

    public string LastName { get; set; }

    public string City { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<OrderGerber> GerberList { get; init; }

    [NotMapped]
    public IList<string> Roles { get; set; }
}
