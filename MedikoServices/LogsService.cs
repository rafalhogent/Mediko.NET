using MedikoData.Entities;
using MedikoData.Repos;


namespace MedikoServices
{
    public class LogsService
    {
        private ILogsRepo _logsRepo;
        public LogsService(ILogsRepo logsRepo)
        {
            _logsRepo = logsRepo;
        }

        public async Task<bool> AddNewLog(int logBookId, string userId, DateTime logTime, float? value1, float? value2, float? value3, string? comment)
        {
            return await _logsRepo.AddNewLogAsync(logBookId, userId, logTime, value1, value2, value3, comment);
        }

        public async Task<Log?> GetLogByIdAsync(int logId)
        {
            return await _logsRepo.GetLogByIdAsync(logId);
        }

        public async Task<IEnumerable<Log>> GetUserLogsByLogbookIdAsync(string userId, int logbookId)
        {
            return await _logsRepo.GetUserLogsByLogbookIdAsync(userId, logbookId);
        }

        public async Task<bool> RemoveLogAsync(Log log)
        {
            return await _logsRepo.RemoveLogAsync(log);
        }

        public async Task<bool> UpdateLogAsync(Log log)
        {
            return await _logsRepo.UpdateLogAsync(log);
        }
    }
}
