using MedikoData.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedikoData.Repos
{
    public class SQLLogBookRepo : ILogBookRepo
    {
        private MedikoDbContext _context;
        public SQLLogBookRepo(MedikoDbContext context)
        {
            _context = context;
        }

        public async Task AddNewLogBook(LogBook logbook)
        {
            await _context.LogBooks.AddAsync(logbook);
            _context.SaveChanges();
        }

        public async Task DeleteLogBook(int id)
        {
            var logbook = await _context.LogBooks.FindAsync(id);
            if (logbook != null)
            {
                _context.LogBooks.Remove(logbook);
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<LogBook>> GetChoosenLogbooks(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
                return await _context.LogBooks.Where(x => x.UsersWhoChoosen.Contains(user)).ToListAsync();
            return Enumerable.Empty<LogBook>();
        }

        public async Task<LogBook?> GetLogBookById(int? id)
        {
            if (id == null || _context.LogBooks == null)
            {
                return null;
            }

            return await _context.LogBooks.FindAsync(id);



        }

        public async Task<IEnumerable<LogBook>> GetLogBooksForUser(string userId)
        {
            return await _context.LogBooks
                .Where(x => x.Creator == null || x.Creator.Id == userId).ToListAsync();
        }

        public async Task AddChoosenLogbookForUser(string userId, int logbookId)
        {
            var user = await _context.Users.FindAsync(userId);
            var logbook = await _context.LogBooks.FindAsync(logbookId);

            if (user != null && logbook != null) logbook.UsersWhoChoosen.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(LogBook logBook)
        {
            _context.Update(logBook);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromChoosenLogbooksForUser(string userId, int logbookId)
        {
            var user = await _context.Users.FindAsync(userId);

            var logbook = await _context.LogBooks
                                        .Include(x => x.UsersWhoChoosen)
                                        .Where(x => x.LogBookId == logbookId).FirstOrDefaultAsync();

            if (user != null && logbook != null && logbook.UsersWhoChoosen.Contains(user))
            {
                logbook.UsersWhoChoosen.Remove(user);
                await _context.SaveChangesAsync();
            }

        }
    }
}
