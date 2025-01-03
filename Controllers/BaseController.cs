using FormsProyect.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormsProyect.Controllers
{
    public class BaseController : Controller
    {
        private readonly AppDBContext _appDBContext;
        public BaseController(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }
        protected async Task SetLayout()
        {
            var currentUserEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _appDBContext.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);

            if (user != null)
            {
                ViewData["Layout"] = user.Admin
                    ? "~/Views/Shared/_AdminLayout.cshtml"
                    : "~/Views/Shared/_UserLayout.cshtml";
            }
        }
    }
}
