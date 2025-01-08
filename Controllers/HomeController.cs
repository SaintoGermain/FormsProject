using System.Diagnostics;
using FormsProyect.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using FormsProyect.Data;
using FormsProyect.ViewModels;
using FormsProyect.Models.Entities;
using System.Security.Claims;
using System;
using Microsoft.EntityFrameworkCore;

namespace FormsProyect.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _appDBContext;
        public HomeController(ILogger<HomeController> logger, AppDBContext appDbContext)
        {
            _logger = logger;
            _appDBContext = appDbContext;
        }

        public IActionResult Page()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditForm(int id)
        {

            var forms = _appDBContext.Forms
                .Include(f => f.FormTags)
                .ThenInclude(ft => ft.Tags)
                .Include(f => f.Questions)
                .FirstOrDefault(f => f.NoForm == id);

            var topics = _appDBContext.Topics.ToList();
            var tags = _appDBContext.Tags.Select(t => t._TagName).ToList();
            var tagNames = forms.FormTags.Select(ft => ft.Tags._TagName).ToList();

            var viewModel = new FormViewModel
            {
                Title = forms.Title,
                Description = forms.Descr,
                SelectedTopicId = forms.TopicID,
                IsPublic = forms.IsPublic,
                QuestionsEdit = forms.Questions.Select(t => t._Type).ToList(),
                Topics = topics,
                TagsL = tags,
                TagsEdit = tagNames,
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Profile()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Access");
            }

            var forms = _appDBContext.Forms
                .Where(f => f.UserId == int.Parse(userId))
                .ToList();

            var viewModel = new ProfileViewModel
            {
                Forms = forms,
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Access");
        }
    }
}
