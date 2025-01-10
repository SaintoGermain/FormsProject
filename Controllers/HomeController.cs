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
                .FirstOrDefault(f => f.NoForm == id);

            var topics = _appDBContext.Topics.ToList();
            var tags = _appDBContext.Tags.Select(t => t._TagName).ToList();
            var tagNames = forms.FormTags.Select(ft => ft.Tags._TagName).ToList();

            var viewModel = new FormViewModel
            {
                NoForm = forms.NoForm,
                Title = forms.Title.Length > 20
                    ? forms.Title.Substring(0, 20)
                    : forms.Title,
                Description = forms.Descr.Length > 50
                    ? forms.Title.Substring(0, 50)
                    : forms.Title,
                SelectedTopicId = forms.TopicID,
                IsPublic = forms.IsPublic,
                Topics = topics,
                TagsL = tags,
                TagsEdit = tagNames,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteForm(int id)
        {
            Forms forms = await _appDBContext.Forms.FirstAsync(u => u.NoForm == id);

            _appDBContext.Forms.Remove(forms);
            await _appDBContext.SaveChangesAsync();

            return RedirectToAction("Profile", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditForm(FormViewModel model)
        {
            var form = _appDBContext.Forms
                    .Include(f => f.FormTags)
                    .FirstOrDefault(f => f.NoForm == model.NoForm);

            form.Title = model.Title.Length > 20
                    ? model.Title.Substring(0, 20)
                    : model.Title;
            form.Descr = model.Description.Length > 50
                    ? model.Description.Substring(0, 50)
                    : model.Description;
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
            return RedirectToAction("EditQuestions", new
            {
                model.NoForm,
            });
        }

        [HttpGet]
        public IActionResult EditQuestions(int NoForm)
        {
            var forms = _appDBContext.Forms
               .Include(f => f.Questions)
               .FirstOrDefault(f => f.NoForm == NoForm);

            var model = new FormViewModel
            {
                Questions = forms.Questions.Select(q => new QDetailsViewModel
                {
                    QuestionID = q.IDQuest,
                    QuestionTitle = q.TitleQ,
                    QuestionDescription = q.DescrQ,
                    QuestionType = q._Type,
                    QuestionShow = q._Show
                }).ToList(),
                NoForm = NoForm,
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditQuestions()
        {
            return RedirectToAction("Page","Home");
        }

        [HttpGet]
        [Route("Home/DeleteQuestion/{id}/{NoForm}")]
        public async Task<IActionResult> DeleteQuestion(int id, int NoForm)
        {
            var questions = await _appDBContext.Questions.FirstOrDefaultAsync(u => u.IDQuest == id);
            
            _appDBContext.Questions.Remove(questions);
            await _appDBContext.SaveChangesAsync();

            return RedirectToAction("EditQuestions", new
            {
                NoForm,
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
