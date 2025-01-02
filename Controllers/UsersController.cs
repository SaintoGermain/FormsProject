using Microsoft.AspNetCore.Mvc;

using FormsProyect.Data;
using FormsProyect.Models.Entities;
using Microsoft.EntityFrameworkCore;

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
    }
}
