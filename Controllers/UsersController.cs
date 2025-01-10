using Microsoft.AspNetCore.Mvc;

using FormsProyect.Data;
using FormsProyect.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace FormsProyect.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public UsersController(AppDBContext appDbContext)
        {
            _appDBContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> UsersList()
        {
            List<Users> listU = await _appDBContext.Users.ToListAsync();
            return View(listU);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Users users = await _appDBContext.Users.FirstAsync(u => u.UserId == id);

            _appDBContext.Users.Remove(users);
            await _appDBContext.SaveChangesAsync();

            var currentUser = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (currentUser == users.Email)
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Register", "Access");
            }

            bool hasUsers = await _appDBContext.Users.AnyAsync();
            if (!hasUsers)
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Register", "Access");
            }

            return RedirectToAction(nameof(UsersList));
        }

        [HttpGet]
        public async Task<IActionResult> Block(int id)
        {
            Users users = await _appDBContext.Users.FirstAsync(u => u.UserId == id);

            users.Active = !users.Active;

            var currentUser = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (currentUser == users.Email )
            {
                _appDBContext.Users.Update(users);
                await _appDBContext.SaveChangesAsync();
                await HttpContext.SignOutAsync();
                return RedirectToAction("Register", "Access");   
            }

            _appDBContext.Users.Update(users);
            await _appDBContext.SaveChangesAsync();
            return RedirectToAction(nameof(UsersList));
        }

        [HttpGet]
        public async Task<IActionResult> AddAdmin(int id)
        {
            Users users = await _appDBContext.Users.FirstAsync(u => u.UserId == id);

            users.Admin = !users.Admin;

            var currentUser = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (currentUser == users.Email)
            {
                _appDBContext.Users.Update(users);
                await _appDBContext.SaveChangesAsync();
                await HttpContext.SignOutAsync();
                return RedirectToAction("Login", "Access");
            }

            _appDBContext.Users.Update(users);
            await _appDBContext.SaveChangesAsync();
            return RedirectToAction(nameof(UsersList));

        }
    }
}
