using MedikoData.Entities;
using MedikoData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MedikoWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace MedikoWeb.Controllers
{
    [Authorize(Roles="Admin")]
    public class SecurityController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SecurityController(UserManager<AppUser> userManager,
                    SignInManager<AppUser> signInManager,
                    RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signinManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var allUsers = _userManager.Users;
            return View(allUsers);
        }

        public async Task<IActionResult> UserDetail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return RedirectToAction("Index");

            UserDetailViewModel userDetailVM = new UserDetailViewModel();
            userDetailVM.UserName = user.UserName;
            userDetailVM.UserId = user.Id;



            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in _roleManager.Roles)
            {
                userDetailVM.RoleSelections
                    .Add(new RoleSelection(role.NormalizedName, userRoles.Contains(role.Name)));
            }

            return View(userDetailVM);
        }



        [HttpPost]
        public async Task<IActionResult> UserRolesUpdate(UserDetailViewModel userDetailVM)
        {
            var user = await _userManager.FindByIdAsync(userDetailVM.UserId);

            foreach (var item in userDetailVM.RoleSelections)
            {
                if (item.IsSelected)
                {
                    if (!_userManager.IsInRoleAsync(user, item.RoleName).Result)
                    {
                        await _userManager.AddToRoleAsync(user, item.RoleName);
                    }
                }
                else
                {
                    if (_userManager.IsInRoleAsync(user, item.RoleName).Result)
                    {
                        await _userManager.RemoveFromRoleAsync(user, item.RoleName);
                    }
                }
            }

            return RedirectToAction(nameof(UserDetail), new { id = user.Id });
        }


        [HttpPost]
        public async Task<IActionResult> RemoveUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
