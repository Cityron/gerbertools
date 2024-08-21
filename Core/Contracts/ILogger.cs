using GerberBackend.Core.MongoDB.Entities;

namespace GerberBackend.Core.Contracts;

public interface ILogger
{
    Task LogErrorServerAsync(string errorMessage, string userId = null);
    Task LogUserActionAsync(string userId, string action);
    Task LogErrorClientAsync(string errorMessage, string userId = null);
    Task<List<ErrorLogServer>> GetAllErrorsServerAsync();
    Task<List<ErrorLogServer>> GetUserIdErrorsServerAsync(string userId);
    Task<List<ErrorLogClient>> GetAllErrorsClientAsync();
    Task<List<ErrorLogClient>> GetUserIdErrorsClientAsync(string userId);
    Task<List<UserAction>> GetAllUsersActionsAsync();
    Task<List<UserAction>> GetUserIdActionsAsync(string userId);
}
