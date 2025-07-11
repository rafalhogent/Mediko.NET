using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MedikoData.Entities;

namespace MedikoWeb.ViewComponents
{
    public class NavbarSecurityMenu : ViewComponent
    {
        private readonly SignInManager<AppUser> _signinManager;
        private readonly UserManager<AppUser> _userManager;

        public NavbarSecurityMenu(SignInManager<AppUser> signinManager, UserManager<AppUser> userManager)
        {
            _signinManager = signinManager;
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {

            if (User.IsInRole("Admin"))
            {
                return View("NavbarSecurityMenu");
            }

            return View();

        }
    }
}
