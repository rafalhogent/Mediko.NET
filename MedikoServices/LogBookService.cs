using MedikoData.Entities;
using MedikoData.Repos;

namespace MedikoServices
{
    public class LogBookService 
    {
        private ILogBookRepo _logBookRepo;
        public LogBookService(ILogBookRepo logBookRepo)
        {
            _logBookRepo = logBookRepo;
        }

        public async Task AddNewLogBook(LogBook logbook)
        {
            await _logBookRepo.AddNewLogBook(logbook);
        }

        public async Task DeleteLogBook(int id)
        {
            await _logBookRepo.DeleteLogBook(id);
        }

        public async Task<IEnumerable<LogBook>> GetChoosenLogbooks(string id)
        {
           return await _logBookRepo.GetChoosenLogbooks(id);
        }

        public async Task<LogBook?> GetLogBookById(int? id)
        {
            return await _logBookRepo.GetLogBookById(id);
        }

        public async Task<IEnumerable<LogBook>> GetLogBooksForUser(string userId)
        {
           return await  _logBookRepo.GetLogBooksForUser(userId);
            
        }

        public async Task AddChoosenLogbookForUser(string userId, int logbookId)
        {
            await _logBookRepo.AddChoosenLogbookForUser(userId, logbookId);
        }

        public async Task Update(LogBook logBook)
        {
            await _logBookRepo.Update(logBook);
        }

        public async Task RemoveFromChoosenLogbooksForUser(string userId, int logbookId)
        {
            await _logBookRepo.RemoveFromChoosenLogbooksForUser(userId, logbookId);
        }

        public async Task<IEnumerable<LogBook>> GetUsersLogbooksAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
