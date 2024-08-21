using GerberBackend.Core.MongoDB.Entities;
using MongoDB.Driver;
using MongoDB.Entities;

namespace GerberBackend.Core.MongoDB;

public class DBMongoInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        var connectionString = app.Configuration.GetConnectionString("MongoDbConnection");

        await DB.InitAsync("Logger", MongoClientSettings.FromConnectionString(connectionString));

        await DB.Index<UserAction>()
            .Key(x => x.UserId, KeyType.Text)
            .Key(x => x.Action, KeyType.Text)
            .Key(x => x.Timestamp, KeyType.Ascending)
            .CreateAsync();

        await DB.Index<ErrorLogServer>()
            .Key(x => x.ErrorMessage, KeyType.Text)
            .Key(x => x.UserId, KeyType.Text)
            .Key(x => x.Timestamp, KeyType.Ascending)
            .CreateAsync();

        await DB.Index<ErrorLogClient>()
            .Key(x => x.ErrorMessage, KeyType.Text)
            .Key(x => x.UserId, KeyType.Text)
            .Key(x => x.Timestamp, KeyType.Ascending)
            .CreateAsync();
    }
}
