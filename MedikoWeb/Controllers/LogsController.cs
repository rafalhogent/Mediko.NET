using MedikoData.Entities;
using MedikoWeb.Models;
using MedikoServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace MedikoWeb.Controllers
{
    public class LogsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly LogBookService _logbookService;
        private readonly LogsService _logsService;
        private static string? _message;

        public LogsController(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager,
                              LogBookService logBookService,
                              LogsService logsService)
        {
            _userManager = userManager;
            _signinManager = signInManager;
            _logbookService = logBookService;
            _logsService = logsService;
        }

        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "User");

            return RedirectToAction("Dashboard", "User");
        }

        public async Task<IActionResult> Diary(int logbookId)
        {
            if (logbookId < 1) return RedirectToAction("Dashboard", "User");
            if (User?.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "User");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "User");

            DiaryViewModel diaryVM = new DiaryViewModel();
            var logbook = _logbookService.GetLogBookById(logbookId).Result;
            if (logbook == null) RedirectToAction("Index", "Home");

            diaryVM.Message = !string.IsNullOrWhiteSpace(_message) ? _message + "" : null;
            _message = null;
            diaryVM.Logbook = _logbookService.GetLogBookById(logbookId).Result;
            diaryVM.UserLogs = new List<Log>();

            diaryVM.NewLog = new LogViewModel();

            diaryVM.NewLog.LogbookId = logbookId;
            diaryVM.NewLog.DateAndTime = DateTime.Now;

            var userLogs = await _logsService.GetUserLogsByLogbookIdAsync(currentUser.Id, logbookId);
            diaryVM.UserLogs = userLogs != null ? userLogs.ToList() : new List<Log>();

            return View(diaryVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddLog(DiaryViewModel diaryVM)
        {
            var logBook = await _logbookService.GetLogBookById(diaryVM.NewLog.LogbookId);
            if (logBook == null)
            {
                ErrorViewModel error = new ErrorViewModel { Message = "Logboek niet gevonden" };
                return View("Error", error);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ErrorViewModel error = new ErrorViewModel { Message = "Gebruiker niet gevonden" };
                return View("Error", error);
            }

            Log newLog = new Log();

            newLog.LogTime = diaryVM.NewLog.DateAndTime;
            newLog.Value1 = diaryVM.NewLog.Value1;
            newLog.Value2 = diaryVM.NewLog.Value2;
            newLog.Value3 = diaryVM.NewLog.Value3;
            newLog.Comment = diaryVM.NewLog.Comment;

            newLog.LogBook = logBook;

            var result = await _logsService.AddNewLog(
                                         newLog.LogBook.LogBookId,
                                         user.Id,
                                         newLog.LogTime,
                                         newLog.Value1,
                                         newLog.Value2,
                                         newLog.Value3,
                                         newLog.Comment);
            if (result == false)
            {
                ErrorViewModel error = new ErrorViewModel { Message = "Het toevoegen van gebruiker is mislukt" };
                return View("Error", error);
            }

            return RedirectToAction(nameof(Diary), new { logbookId = logBook.LogBookId });
        }


        public async Task<IActionResult> DeleteLog(int logid)
        {
            var log = await _logsService.GetLogByIdAsync(logid);
            if (log != null)
            {
                var result = await _logsService.RemoveLogAsync(log);
                if (result) _message = $"Log met id : {logid} verwijderd";
                else _message = $"Verwijderen Log met id : {logid} mislukt";
                return RedirectToAction(nameof(Diary), new { logbookId = log.LogBook.LogBookId });
            }
            _message = $"Log met id : {logid} niet gevonden";
            return RedirectToAction(nameof(Diary), new { logbookId = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLog(DiaryViewModel diaryVM)
        {
            if (diaryVM.NewLog.logId == null || diaryVM.NewLog.logId == 0)
            {
                _message = $"Log 0 nniet gevonden";
                return RedirectToAction(nameof(Diary), new { logbookId = diaryVM.NewLog.LogbookId });
            }
            var logToUpdate = await _logsService.GetLogByIdAsync(diaryVM.NewLog.logId.Value);
            if (logToUpdate != null)
            {
                logToUpdate.LogTime = diaryVM.NewLog.DateAndTime;
                logToUpdate.Value1 = diaryVM.NewLog.Value1;
                logToUpdate.Value2 = diaryVM.NewLog.Value2;
                logToUpdate.Value3 = diaryVM.NewLog.Value3;
                logToUpdate.Comment = diaryVM.NewLog.Comment;

                if (await _logsService.UpdateLogAsync(logToUpdate)) _message = $"Log {logToUpdate.LogId} gewijzigd";
                else _message = $"Log {logToUpdate.LogId} update failed";
                return RedirectToAction(nameof(Diary), new { logbookId = diaryVM.NewLog.LogbookId });
            }
            _message = $"Log met id : {diaryVM.NewLog.logId} niet gevonden";
            return RedirectToAction(nameof(Index));
        }
    }
}
