using GerberBackend.Core.MongoDB.Entities;
using MongoDB.Driver;
using MongoDB.Entities;
using ILogger = GerberBackend.Core.Contracts.ILogger;

namespace GerberBackend.Core.Services;

public class LoggerService : ILogger
{
    public async Task LogErrorServerAsync(string errorMessage, string userId = null)
    {
        var errorLog = new ErrorLogServer
        {
            ErrorMessage = errorMessage,
            Timestamp = DateTime.Now,
            UserId = userId,
        };

        await errorLog.SaveAsync();
    }

    public async Task LogUserActionAsync(string userId, string action)
    {
        var userAction = new UserAction
        {
            UserId = userId,
            Action = action,
            Timestamp = DateTime.Now
        };

        await userAction.SaveAsync();
    }

    public async Task LogErrorClientAsync(string errorMessage, string userId = null)
    {
        var errorLog = new ErrorLogClient
        {
            ErrorMessage = errorMessage,
            Timestamp = DateTime.Now,
            UserId = userId,
        };

        await errorLog.SaveAsync();
    }

    public async Task<List<ErrorLogServer>> GetAllErrorsServerAsync()
    {
        return await DB.Find<ErrorLogServer>().ExecuteAsync();
    }

    public async Task<List<ErrorLogServer>> GetUserIdErrorsServerAsync(string userId)
    {
        var collection = DB.Collection<ErrorLogServer>();

        var filter = Builders<ErrorLogServer>.Filter.Eq(ua => ua.UserId, userId);

        var userActions = await collection.Find(filter).ToListAsync();
        return userActions;
    }

    public async Task<List<ErrorLogClient>> GetAllErrorsClientAsync()
    {
        return await DB.Find<ErrorLogClient>().ExecuteAsync();
    }

    public async Task<List<ErrorLogClient>> GetUserIdErrorsClientAsync(string userId)
    {
        var collection = DB.Collection<ErrorLogClient>();

        var filter = Builders<ErrorLogClient>.Filter.Eq(ua => ua.UserId, userId);

        var userActions = await collection.Find(filter).ToListAsync();
        return userActions;
    }

    public async Task<List<UserAction>> GetAllUsersActionsAsync()
    {
        return await DB.Find<UserAction>().ExecuteAsync();
    }

    public async Task<List<UserAction>> GetUserIdActionsAsync(string userId)
    {
        var collection = DB.Collection<UserAction>();

        var filter = Builders<UserAction>.Filter.Eq(ua => ua.UserId, userId);

        var userActions = await collection.Find(filter).ToListAsync();
        return userActions;
    }
}
