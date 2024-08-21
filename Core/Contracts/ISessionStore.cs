namespace GerberBackend.Core.Contracts;

public interface ISessionStore
{
    SessionData GetSession(Guid sessionId);
    void AddFiles(Guid sessionId, Files files);
    Files GetFiles(Guid sessionId);

    void RemoveFile(Guid sessionId);

    SessionData IsValidSession(Guid sessionId);
}

public class SessionData
{
    public Guid SessionId { get; set; }
    public DateTime SessionTimeLeft { get; set; }
}

public class Files
{
    public (byte[] file1, byte[] file2) files { get; set; }
}
