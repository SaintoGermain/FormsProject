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
using Newtonsoft.Json;

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
            var questionsByType = forms.Questions.GroupBy(q => q._Type)
                .ToDictionary(g => g.Key, g => g.Count());

            var topics = _appDBContext.Topics.ToList();
            var tags = _appDBContext.Tags.Select(t => t._TagName).ToList();
            var tagNames = forms.FormTags.Select(ft => ft.Tags._TagName).ToList();

            var viewModel = new FormViewModel
            {
                NoForm = forms.NoForm,
                Title = forms.Title,
                Description = forms.Descr,
                SelectedTopicId = forms.TopicID,
                IsPublic = forms.IsPublic,
                Topics = topics,
                TagsL = tags,
                TagsEdit = tagNames,
                numberOfSingleLineQuestions = questionsByType.ContainsKey(1) ? questionsByType[1] : 0,
                numberOfMultipleLinesQuestions = questionsByType.ContainsKey(2) ? questionsByType[2] : 0,
                numberOfPositiveIntegersQuestions = questionsByType.ContainsKey(3) ? questionsByType[3] : 0,
                numberOfCheckboxQuestions = questionsByType.ContainsKey(4) ? questionsByType[4] : 0
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditForm(FormViewModel model)
        {
            var form = _appDBContext.Forms
                    .Include(f => f.FormTags)
                    .FirstOrDefault(f => f.NoForm == model.NoForm);

            form.Title = model.Title;
            form.Descr = model.Description;
            form.TopicID = model.SelectedTopicId;
            form.IsPublic = model.IsPublic;

            var tagsJson = model.Tags;
            var tagList = JsonConvert.DeserializeObject<List<TagModel>>(tagsJson);
            var tagsToSave = new List<Tags>();
            var formTagsToSave = new List<FormTags>();

            foreach (var tag in tagList)
            {
                var existingTag = await _appDBContext.Tags.FirstOrDefaultAsync(t => t._TagName == tag.Value);
                if (existingTag == null)
                {
                    // Create new tag if it doesn't exist
                    var newTag = new Tags
                    {
                        _TagName = tag.Value
                    };
                    tagsToSave.Add(newTag);
                }
            }

            // Add new tags
            if (tagsToSave.Any())
            {
                _appDBContext.Tags.AddRange(tagsToSave);
                await _appDBContext.SaveChangesAsync();
            }
            // Elimina las preguntas anteriores y agrega las actualizadas

            foreach (var tag in tagList)
            {
                var existingTag = await _appDBContext.Tags.FirstOrDefaultAsync(t => t._TagName == tag.Value);
                int TagIDSearch = 0;

                if (existingTag != null)
                {
                    TagIDSearch = existingTag.TagID;
                }

                var formTags = new FormTags
                {

                    NoForm = form.NoForm,
                    TagID = TagIDSearch,
                };
                formTagsToSave.Add(formTags);
            }
            _appDBContext.FormTags.RemoveRange(form.FormTags);
            await _appDBContext.FormTags.AddRangeAsync(formTagsToSave);

            // Guarda los cambios
            await _appDBContext.SaveChangesAsync();

            return RedirectToAction("Profile", "Home", new
            {
                model.numberOfSingleLineQuestions,
                model.numberOfMultipleLinesQuestions,
                model.numberOfPositiveIntegersQuestions,
                model.numberOfCheckboxQuestions,
                form.NoForm,
            });
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
