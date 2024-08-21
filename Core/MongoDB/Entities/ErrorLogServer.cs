using MongoDB.Entities;

namespace GerberBackend.Core.MongoDB.Entities;

public class ErrorLogServer : Entity
{
    public string ErrorMessage { get; set; }
    public string UserId { get; set; } = null;
    public DateTime Timestamp { get; set; }
}
