using BuildyBackend.Core.Helpers;
using BuildyBackend.Infrastructure.DbContext;

namespace BuildyBackend.Infrastructure.Services;

public class LogService : ILogService
{
    private readonly ContextDB _context;
    public LogService(ContextDB context)
    {
        _context = context;
    }

    public async Task LogAction(string entity, string action, string username, string data)
    {
        var log = new Log
        {
            Entity = entity,
            Action = action,
            Username = username,
            Data = data ?? "", // Asegúrate de que no se inserte nulo
            Creation = GlobalServices.GetDatetimeUruguay()
        };
        _context.Log.Add(log);
        await _context.SaveChangesAsync();
    }
}