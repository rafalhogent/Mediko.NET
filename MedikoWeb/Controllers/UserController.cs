using MedikoData.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedikoWeb.Models;
using MedikoServices;

namespace MedikoWeb.Controllers
{
    
    public class UserController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly LogBookService _logbookService;
        public string? _message;

        public UserController(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager,
                              LogBookService logBookService
            )
        {
            _userManager = userManager;
            _signinManager = signInManager;
            _logbookService = logBookService;
        }


        [Route("Account")]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction(nameof(Dashboard));

            return RedirectToAction("Index", "Home");
        }


        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            ViewData["userName"] = User?.Identity?.Name + "";
            if (User?.Identity?.IsAuthenticated != true)
                return RedirectToAction(nameof(Login));

            return View();
        }


        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userVM.UserName) || string.IsNullOrEmpty(userVM.Password))
                {
                    ModelState.AddModelError("LoginError", "Gebruikersnaam en wachtwoor zij verplicht");
                    return View();
                }
                var foundUser = await _userManager.FindByNameAsync(userVM.UserName);
                if (foundUser == null)
                {
                    ModelState.AddModelError("LoginError", "Gebruikersnaam of wachtoord niet correct");
                    return View();
                }

                var result = await _signinManager
                    .PasswordSignInAsync(foundUser, userVM.Password, false, false);

                if (result.Succeeded) return RedirectToAction(nameof(Dashboard));
                ModelState.AddModelError("LoginError", "Gebruikersnaam of wachtoord niet correct");

            }
            
            return View();
        }


        public async Task<IActionResult> Demo()
        {
            var foundUser = await _userManager.FindByNameAsync("Demo");
            if (foundUser == null)
            {

                return RedirectToAction(nameof(Login));
            }


            var result = await _signinManager
                        .PasswordSignInAsync(foundUser, "Demo2022#", false, false);

            if (result.Succeeded) return RedirectToAction(nameof(Dashboard));
            return RedirectToAction(nameof(Login));
        }


        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }


        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserRegisterViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = userVM.UserName,
                };

                var existingUser = await _userManager.FindByNameAsync(userVM.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserExists", "Gebruikersnaam al bestaat, kies andere naam");
                    return View();
                }
                if (user.UserName.Contains(' '))
                {
                    ModelState.AddModelError("BadName", 
                        $"Gebruikersnaam niet correct. Probeer {userVM.UserName.Trim().Replace(' ', '_')}");
                    return View();
                }
                var result = await _userManager.CreateAsync(user, userVM.Password);

                if (result.Succeeded)
                {
                    var nieuwUser = _userManager.FindByNameAsync(user.UserName).Result;

                    if (!_userManager.IsInRoleAsync(nieuwUser, "Patient").Result)
                    {
                        await _userManager.AddToRoleAsync(nieuwUser, "Patient");
                    }
                    var logbooksForUser = await _logbookService.GetLogBooksForUser(nieuwUser.Id);
                    var choosenLogbooks = await _logbookService.GetChoosenLogbooks(nieuwUser.Id);

                    foreach (var logbook in logbooksForUser)
                    {
                        if (!choosenLogbooks.Contains(logbook))
                        {
                            await _logbookService.AddChoosenLogbookForUser(nieuwUser.Id, logbook.LogBookId);
                        }
                    }
                    await _signinManager.SignInAsync(nieuwUser, isPersistent: false);

                    return RedirectToAction(nameof(Index), "Home");
                }
                else
                {
                    ModelState.AddModelError("RegFailed", "Registartie mislukt, controleer gegevens");
                    return View("Register", userVM);
                }

            }
            return View(userVM);
        }

        [Route("Options")]
        public async Task<IActionResult> Options()
        {
            if (User?.Identity?.IsAuthenticated != true)
                return RedirectToAction(nameof(Login));

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Editor");

            var logbooksForUser = await _logbookService.GetLogBooksForUser(user.Id);
            var choosenLogbooks = await _logbookService.GetChoosenLogbooks(user.Id);

            List<LogbookSelection> logbooksSelections = new List<LogbookSelection>();

            foreach (var logbook in logbooksForUser)
            {
                logbooksSelections.Add(new LogbookSelection
                {
                    LogbookName = logbook.Name,
                    LogbookId = logbook.LogBookId,
                    IsSelected = choosenLogbooks.Contains(logbook),
                    Editable = isAdmin && logbook.Creator == null || logbook.Creator?.Id == user.Id,
                });
            }

            UserOptionsViewModel optionsVM = new UserOptionsViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Message = _message,
                LogbookSelections = logbooksSelections,
                PasswordEditingAllowed = user.UserName == "Demo" ? false : true                
            };
            return View(optionsVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateChoosenLogbooks(UserOptionsViewModel optionsVM)
        {
            var logbooksForUser = await _logbookService.GetLogBooksForUser(optionsVM.UserId);
            var choosenLogbooks = await _logbookService.GetChoosenLogbooks(optionsVM.UserId);

            foreach (var selection in optionsVM.LogbookSelections)
            {
                if (selection.IsSelected)
                {
                    if (!choosenLogbooks.Contains(_logbookService.GetLogBookById(selection.LogbookId).Result))
                    {
                        await _logbookService.AddChoosenLogbookForUser(optionsVM.UserId, selection.LogbookId);
                    }
                }
                else
                {
                    if (choosenLogbooks.Contains(_logbookService.GetLogBookById(selection.LogbookId).Result))
                    {
                        await _logbookService.RemoveFromChoosenLogbooksForUser(optionsVM.UserId, selection.LogbookId);
                    }
                }
            }

            return RedirectToAction(nameof(Options));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            if (User?.Identity?.IsAuthenticated != true)
                return RedirectToAction(nameof(Login));

            var user = await _userManager.GetUserAsync(User);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordConfirmed(UserEditPasswordViewModel editPassVM)
        {
            var user = await _userManager.GetUserAsync(User);
            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = _userManager.ResetPasswordAsync(user, code, editPassVM.NewPassword);
            if (result.Result.Succeeded)
            {
                _message = "Wachtwoord gewijzigd";
            }
            else
            {
                _message = "wijziging mislukt";
            }
            return RedirectToAction(nameof(Options));

        }
    }
}
