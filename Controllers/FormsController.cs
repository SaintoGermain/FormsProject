using FormsProyect.Data;
using FormsProyect.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormsProyect.ViewModels;
using Azure;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FormsProyect.Controllers
{
    public class FormsController : Controller
    {
        private readonly AppDBContext _appDBContext;
        Users user;
        List<Tags> associatedTags;

        public FormsController(AppDBContext appDbContext)
        {
            _appDBContext = appDbContext;
        }

        [HttpGet]
        public IActionResult Forms()
        {
            var topics = _appDBContext.Topics.ToList();

            var viewModel = new FormViewModel
            {
                Topics = topics
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Forms(FormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If the model is wrong, it returns the view
                model.Topics = await _appDBContext.Topics.ToListAsync();
                return View(model);
            }

            // Get logged user
            string? userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            // Search the user in the DB
            user = await _appDBContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized();

            // Tags
            var tagsJson = model.Tags;
            var tagList = JsonConvert.DeserializeObject<List<TagModel>>(tagsJson);

            var tagsToSave = new List<Tags>();

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



            var form = new Forms
            {
                Title = model.Title,
                Descr = model.Description,
                UserId = user.UserId,
                TopicID = model.SelectedTopicId,
            };



            return RedirectToAction("AdminPage", "Home");
            //{
            //    model.numberOfSingleLineQuestions,
            //    model.numberOfMultipleLinesQuestions,
            //    model.numberOfPositiveIntegersQuestions,
            //    model.numberOfCheckboxQuestions
            //});
        }

        [HttpPost]
        public IActionResult Questions(int numberOfSingleLineQuestions, int numberOfMultipleLinesQuestions, int numberOfPositiveIntegersQuestions, int numberOfCheckboxQuestions)
        {
            var model = new QuestionsViewModel
            {
                NumberOfSingleLineQuestions = numberOfSingleLineQuestions,
                NumberOfMultipleLinesQuestions = numberOfMultipleLinesQuestions,
                NumberOfPositiveIntegersQuestions = numberOfPositiveIntegersQuestions,
                NumberOfCheckboxQuestions = numberOfCheckboxQuestions
            };

            return View(model);
        }


    }
}