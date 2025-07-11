using Microsoft.EntityFrameworkCore;
using MedikoData.Entities;

namespace MedikoData.Repos
{
    public class SQLLogsRepo : ILogsRepo
    {
        private readonly MedikoDbContext _context;

        public SQLLogsRepo(MedikoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewLogAsync(int logBookId, string userId, DateTime logTime, float? value1, float? value2, float? value3, string? comment)
        {
            var logBook = await _context.LogBooks.FindAsync(logBookId);
            if (logBook == null) return false;
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            Log newLog = new Log
            {
                LogBook = logBook,
                Creator = user,
                LogTime = logTime,
                Value1 = value1,
                Value2 = value2,
                Value3 = value3,
                Comment = comment
            };
            await _context.Logs.AddAsync(newLog);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<Log?> GetLogByIdAsync(int logId)
        {
            return await _context.Logs.Where(x => x.LogId == logId).Include(x => x.LogBook).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Log>> GetUserLogsByLogbookIdAsync(string userId, int logbookId)
        {
            return await _context.Logs
                .Where(x => x.Creator.Id == userId && x.LogBook.LogBookId == logbookId).OrderByDescending(x => x.LogTime).ToListAsync();
        }

        public async Task<bool> RemoveLogAsync(Log log)
        {
            var result = _context.Logs.Remove(log);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateLogAsync(Log log)
        {
            var logUpd = await GetLogByIdAsync(log.LogId);
            if (logUpd == null) return false;
            var result = _context.Logs.Update(logUpd);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
