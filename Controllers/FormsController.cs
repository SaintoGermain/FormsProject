using FormsProyect.Data;
using FormsProyect.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormsProyect.ViewModels;
using Azure;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            var tags = _appDBContext.Tags.Select(t => t._TagName).ToList();

            var viewModel = new FormViewModel
            {
                Topics = topics,
                TagsL = tags,
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Forms(FormViewModel model)
        {
            if (!ModelState.IsValid)
            {
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
            //Allowed Users
            //var allowedUsersJson = model.Users;
            //var allowedUsersList = JsonConvert.DeserializeObject<List<AllowedUsersModel>>(allowedUsersJson);
            //var allowedUsersToSave = new List<AllowedUsers>();
            //FormTags
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

            var form = new Forms
            {
                Title = model.Title.Length > 20
                    ? model.Title.Substring(0,20)
                    : model.Title,
                Descr = model.Description.Length > 50
                    ? model.Description.Substring(0, 50)
                    : model.Description,
                UserId = user.UserId,
                TopicID = model.SelectedTopicId,
                IsPublic = model.IsPublic,
            };

            _appDBContext.Forms.Add(form);
            await _appDBContext.SaveChangesAsync();

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
            _appDBContext.FormTags.AddRange(formTagsToSave);
            await _appDBContext.SaveChangesAsync();

            //foreach (var allowUser in allowedUsersList)
            //{
            //    var existingUser = await _appDBContext.Users.FirstOrDefaultAsync(t => t.Email == allowUser.Email);
            //    int UserIDSearch = 0;

            //    if (existingUser != null)
            //    {
            //        UserIDSearch = existingUser.UserId;
            //    }

            //    var allowedUsers = new AllowedUsers
            //    {
            //        NoForm = form.NoForm,
            //        UserId = UserIDSearch,
            //    };
            //    allowedUsersToSave.Add(allowedUsers);
            //}
            //_appDBContext.AllowedUsers.AddRange(allowedUsersToSave);
            //await _appDBContext.SaveChangesAsync();

            return RedirectToAction("Questions", "Forms", new {
                model.numberOfSingleLineQuestions,
                model.numberOfMultipleLinesQuestions,
                model.numberOfPositiveIntegersQuestions,
                model.numberOfCheckboxQuestions,
                form.NoForm,
            });
        }

        [HttpGet]
        public IActionResult Questions(int numberOfSingleLineQuestions, int numberOfMultipleLinesQuestions, int numberOfPositiveIntegersQuestions, int numberOfCheckboxQuestions, int NoForm)
        {
            var model = new FormViewModel
            {
                numberOfSingleLineQuestions = numberOfSingleLineQuestions,
                numberOfMultipleLinesQuestions = numberOfMultipleLinesQuestions,
                numberOfPositiveIntegersQuestions = numberOfPositiveIntegersQuestions,
                numberOfCheckboxQuestions = numberOfCheckboxQuestions,
                FormID = NoForm,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Questions(FormViewModel model)
        {
            var questionsToSave = new List<Questions>();

            foreach (var question in model.Questions)
            {
                Console.WriteLine($"Title: {question.QuestionTitle}, Show: {question.QuestionShow}");
                var newquestion = new Questions
                {
                    TitleQ = question.QuestionTitle.Length > 20
                                ? question.QuestionTitle.Substring(0,20)
                                : question.QuestionTitle,
                    DescrQ = question.QuestionDescription.Length > 50
                                ? question.QuestionDescription.Substring(0, 50)
                                : question.QuestionDescription,
                    _Show = question.QuestionShow,
                    _Type = question.QuestionType,
                    NoForm = model.FormID,
                };
                questionsToSave.Add(newquestion);
            }   
        

            _appDBContext.Questions.AddRange(questionsToSave);
            await _appDBContext.SaveChangesAsync();

            return RedirectToAction("Page","Home");
        }


    }
}