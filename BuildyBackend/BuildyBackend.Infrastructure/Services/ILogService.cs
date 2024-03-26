namespace BuildyBackend.Infrastructure.Services;

public interface ILogService
{
    Task LogAction(string entity, string action, string username, string data);
}
