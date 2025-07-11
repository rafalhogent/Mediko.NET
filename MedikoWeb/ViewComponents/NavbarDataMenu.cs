using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MedikoData.Entities;
using MedikoWeb.Models;
using MedikoServices;

namespace MedikoWeb.ViewComponents
{
    public class NavbarDataMenu : ViewComponent
    {
        private readonly SignInManager<AppUser> _signinManager;
        private readonly LogBookService _logbookService;
        private readonly UserManager<AppUser> _userManager;

        public NavbarDataMenu(SignInManager<AppUser> signinManager,
                              LogBookService logbookservice,
                              UserManager<AppUser> userManager)
        {
            _signinManager = signinManager;
            _logbookService = logbookservice;
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            if (_signinManager.IsSignedIn((System.Security.Claims.ClaimsPrincipal)User))
            {
                DataMenuViewModel viewModel = new DataMenuViewModel();
                var user = _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User).Result;
                var choosenLogbooks = _logbookService.GetChoosenLogbooks(user.Id).Result;

                viewModel.ChoosenLogbooks = choosenLogbooks
                    .Select(x => new LogBookViewModel {Id = x.LogBookId, Name = x.Name}).ToList();

                return View("NavbarDataMenu", viewModel);
            }
            
            else return View();
        }
    }
}
