using GerberBackend.Core.Contracts;
using System.Collections.Concurrent;

namespace GerberBackend.Core.Session;

public class MemorySessionStore : ISessionStore
{

    private readonly ConcurrentBag<SessionData> _sessions = new();
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    private readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(1);


    public MemorySessionStore()
    {
        Task.Run(() => PeriodicallyRemoveExpiredSessionsAsync());
    }

    public void AddFiles(Guid sessionId, Files files)
    {
        _lock.EnterReadLock();

        var directory = Directory.GetCurrentDirectory();

        var existDerectory = new DirectoryInfo(directory + "/Session");

        if (existDerectory.Exists)
        {
            Directory.CreateDirectory(directory + "/Session");
        }



        try
        {
            var session = IsValidSession(sessionId);
            if (session is SessionData)
            {
                var filePath = Path.Combine(directory, "Session", session.SessionId.ToString());

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (FileStream fstream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] file1LengthBytes = BitConverter.GetBytes(files.files.file1.Length);
                    fstream.Write(file1LengthBytes, 0, file1LengthBytes.Length);

                    byte[] file2LengthBytes = BitConverter.GetBytes(files.files.file2.Length);
                    fstream.Write(file2LengthBytes, 0, file2LengthBytes.Length);

                    fstream.Write(files.files.file1, 0, files.files.file1.Length);

                    fstream.Write(files.files.file2, 0, files.files.file2.Length);
                }
            }
        }
        finally { _lock.ExitReadLock(); }
    }

    public Files GetFiles(Guid sessionId)
    {
        _lock.EnterReadLock();

        try
        {
            var session = _sessions.FirstOrDefault(s => s.SessionId.Equals(sessionId));

            if (session is SessionData)
            {
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "Session");
                var filePath = Path.Combine(directory, session.SessionId.ToString());

                if (!File.Exists(filePath))
                {
                    return null;
                }

                using (FileStream fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] file1LengthBytes = new byte[sizeof(int)];
                    fstream.Read(file1LengthBytes, 0, file1LengthBytes.Length);
                    int file1Length = BitConverter.ToInt32(file1LengthBytes, 0);

                    byte[] file2LengthBytes = new byte[sizeof(int)];
                    fstream.Read(file2LengthBytes, 0, file2LengthBytes.Length);
                    int file2Length = BitConverter.ToInt32(file2LengthBytes, 0);

                    byte[] file1Buffer = new byte[file1Length];
                    int bytesReadFile1 = fstream.Read(file1Buffer, 0, file1Length);

                    byte[] file2Buffer = new byte[file2Length];
                    int bytesReadFile2 = fstream.Read(file2Buffer, 0, file2Length);

                    var files = new Files
                    {
                        files = (file1Buffer, file2Buffer)
                    };

                    return files;
                }
            }
            return null;
        }
        finally { _lock.ExitReadLock(); }
    }

    private SessionData AddSession()
    {
        var session = new SessionData { SessionId = Guid.NewGuid(), SessionTimeLeft = DateTime.UtcNow };

        _sessions.Add(session);

        return session;

    }

    private SessionData ExtendSession(Guid sessionId)
    {

        var session = IsValidSession(sessionId);

        if (session is SessionData)
        {
            session.SessionTimeLeft = DateTime.UtcNow;
            return session;
        }
        else
            return null;

    }

    public SessionData GetSession(Guid sessionId)
    {
        _lock.EnterReadLock();
        try
        {
            var session = IsValidSession(sessionId);

            if (session is SessionData)
            {
                if (DateTime.UtcNow - session.SessionTimeLeft > _sessionTimeout)
                {
                    var resultSession = ExtendSession(sessionId);

                    if (resultSession != null)
                    {

                        return resultSession;
                    }

                    var sessionResult = AddSession();
                    return sessionResult;
                }

                return session;
            }
            else
            {
                var sessionResult = AddSession();
                return sessionResult;
            }

        }
        finally { _lock.ExitReadLock(); }
    }


    private async Task PeriodicallyRemoveExpiredSessionsAsync()
    {
        while (true)
        {
            try
            {
                await Task.Run(() => RemoveExpiredSessions());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing expired sessions: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(2));
        }
    }

    private void RemoveExpiredSessions()
    {
        _lock.EnterWriteLock();
        try
        {
            var expiredSessions = _sessions
                .Where(s => DateTime.UtcNow - s.SessionTimeLeft > _sessionTimeout)
                .ToList();

            foreach (var expiredSession in expiredSessions)
            {
                _sessions.TryTake(out _);
                RemoveFile(expiredSession.SessionId);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void RemoveFile(Guid sessionId)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Session");
        var filePath = Path.Combine(directory, sessionId.ToString());

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public SessionData IsValidSession(Guid sessionId)
    {
        var session = _sessions.FirstOrDefault(x => x.SessionId.Equals(sessionId));

        if (session is SessionData)
            return session;
        return null;
    }

}
