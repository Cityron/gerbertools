using MongoDB.Entities;

namespace GerberBackend.Core.MongoDB.Entities;

public class UserAction : Entity
{
    public string UserId { get; set; }
    public string Action { get; set; }
    public DateTime Timestamp { get; set; }
}
