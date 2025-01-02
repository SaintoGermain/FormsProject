using Microsoft.AspNetCore.Mvc;

using FormsProyect.Data;
using FormsProyect.Models;
using Microsoft.EntityFrameworkCore;
using FormsProyect.ViewModels;
using FormsProyect.Models.Entities;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FormsProyect.Controllers
{
    public class AccessController : Controller
    {
        private readonly AppDBContext _appDBContext;
        public AccessController(AppDBContext appDBContext) {
            _appDBContext = appDBContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            if(model.PasswordHash != model.ConfirmPasswordHash)
            {
                ViewData["Message"] = "The passwords are not equal";
                return View();
            }
            Users user = new Users()
            {
                _Name = model._Name,
                Email = model.Email,
                PasswordHash = model.PasswordHash
            };

            await _appDBContext.Users.AddAsync(user);
            await _appDBContext.SaveChangesAsync();

            if(user.UserId != 0){return RedirectToAction("Login","Access");}
            ViewData["Message"] = "The user couldn't be created, error fatal";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("AdminPage","Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login (LoginViewModel model)
        {
            Users? user_search = await _appDBContext.Users
                .Where(u =>
                    u.Email == model.Email &&
                    u.PasswordHash == model.PasswordHash
                ).FirstOrDefaultAsync();
            if(user_search == null) {
                ViewData["Message"] = "The user couldn't be found";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user_search._Name),
                new Claim(ClaimTypes.Email, user_search.Email)
            };

            ClaimsIdentity clIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(clIdentity),
                properties
                );

            return RedirectToAction("AdminPage", "Home");
        }
    }
}
