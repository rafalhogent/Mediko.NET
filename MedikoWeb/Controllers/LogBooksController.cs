using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedikoData;
using MedikoData.Entities;
using MedikoServices;
using Microsoft.AspNetCore.Identity;
using MedikoWeb.Models;

namespace MedikoWeb.Controllers
{
    public class LogBooksController : Controller
    {
        private readonly MedikoDbContext _context;
        private readonly LogBookService _logbookService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;

        public LogBooksController(MedikoDbContext context,
                                  LogBookService logbookService,
                                  UserManager<AppUser> userManager,
                                  SignInManager<AppUser> signinManager)
        {
            _context = context;
            _logbookService = logbookService;

            _userManager = userManager;
            _signinManager = signinManager;
        }

        // GET: LogBooks
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "User");


            string userId = _userManager.GetUserAsync(User).Result.Id;

            var logbooks = await _logbookService.GetLogBooksForUser(userId);

            return logbooks != null ? View(logbooks) : View(new List<LogBook>());


        }

        // GET: LogBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var book = await _logbookService.GetLogBookById(id);
            return View(book);
        }

        // GET: LogBooks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LogBooks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Field1,Unit1,Field2,Unit2,Field3,Unit3,Precision")] LogBookViewModel logBookVM)
        {
            if (ModelState.IsValid)
            {
                LogBook logbook = new LogBook
                {
                    Name = logBookVM.Name,
                    Field1 = logBookVM.Field1,
                    Field2 = logBookVM.Field2,
                    Field3 = logBookVM.Field3,
                    Unit1 = logBookVM.Unit1,
                    Unit2 = logBookVM.Unit2,
                    Unit3 = logBookVM.Unit3,
                    Precision = logBookVM.Precision
                };



                logbook.Creator = User.IsInRole("Admin") || User.IsInRole("Editor") ?
                    null : _userManager.GetUserAsync(User).Result;

                await _logbookService.AddNewLogBook(logbook);

                return RedirectToAction("Options", "User");
            }
            return View(logBookVM);
        }

        // GET: LogBooks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var logbook = await _logbookService.GetLogBookById(id);
            if (logbook == null) return RedirectToAction(nameof(Index));

            var logbookVM = new LogBookViewModel
            {
                Id = id.Value,
                Name = logbook.Name,
                Field1 = logbook.Field1,
                Field2 = logbook.Field2,
                Field3 = logbook.Field3,
                Unit1 = logbook.Unit1,
                Unit2 = logbook.Unit2,
                Unit3 = logbook.Unit3,
                Precision = logbook.Precision
            };

            if (id == null || logbook == null)
            {
                return NotFound();
            }

            return View(logbookVM);
        }

        // POST: LogBooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Name,Field1,Unit1,Field2,Unit2,Field3,Unit3,Precision")] LogBookViewModel logBookVM)
        {

            if (ModelState.IsValid)
            {
                if (id != null)
                {
                    var logbook = await _logbookService.GetLogBookById(id);
                    if (logbook != null)
                    {
                        logbook.Name = logBookVM.Name;
                        logbook.Field1 = logBookVM.Field1;
                        logbook.Field2 = logBookVM.Field2;
                        logbook.Field3 = logBookVM.Field3;
                        logbook.Unit1 = logBookVM.Unit1;
                        logbook.Unit2 = logBookVM.Unit2;
                        logbook.Unit3 = logBookVM.Unit3;
                        logbook.Precision = logBookVM.Precision;
                        try
                        {
                            await _logbookService.Update(logbook);
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return NotFound();
                        }
                    }

                }

                return RedirectToAction("Options", "User");
            }
            return View(logBookVM);
        }

        // GET: LogBooks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var logBook = await _logbookService.GetLogBookById(id);
            if (logBook == null)
            {
                return NotFound();
            }


            return View(logBook);
        }

        // POST: LogBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _logbookService.DeleteLogBook(id);

            return RedirectToAction("Options", "User");
        }

    }
}
