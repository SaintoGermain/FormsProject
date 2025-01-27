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
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NetCoreForce.Client;
using NetCoreForce.Client.Models;
using System;
using System.Threading.Tasks;

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
                Title = forms.Title,
                Description = forms.Descr,
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
        public async Task<IActionResult> EditQuestions(FormViewModel model)
        {
            var existingQuestions = _appDBContext.Questions
                .Where(q => q.NoForm == model.NoForm)
                .ToList();
            Console.WriteLine($"Total Questions in model: {model.Questions.Count}");
            Console.WriteLine($"Total Existing Questions: {existingQuestions.Count}");

            foreach (var questionModel in model.Questions)
            {
                var existingQuestion = existingQuestions.FirstOrDefault(q => q.IDQuest == questionModel.QuestionID);
                Console.WriteLine(existingQuestion.IDQuest);
                Console.WriteLine(questionModel.QuestionID);
                foreach (var question in existingQuestions)
                {
                    Console.WriteLine($"ID: {question.IDQuest}, Title: {question.TitleQ}, Description: {question.DescrQ}, Type: {question._Type}, Show: {question._Show}");
                }
                if (existingQuestion != null)
                {
                    Console.WriteLine("EXISTE");
                    existingQuestion.TitleQ = questionModel.QuestionTitle.Length > 20
                        ? questionModel.QuestionTitle.Substring(0, 20)
                        : questionModel.QuestionTitle;

                    existingQuestion.DescrQ = questionModel.QuestionDescription.Length > 50
                        ? questionModel.QuestionDescription.Substring(0, 50)
                        : questionModel.QuestionDescription;

                    existingQuestion._Show = questionModel.QuestionShow;
                    existingQuestion._Type = questionModel.QuestionType;
                }
            }

            await _appDBContext.SaveChangesAsync();
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

        [HttpGet]
        public IActionResult GetCurrentUser()
        {
            var user = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userDetails = _appDBContext.Users
                .Where(u => u.Email == user)
                .Select(u => new UserViewModel
                {
                    _Name = u._Name,
                    Email = u.Email
                })
                .FirstOrDefault();

            if (userDetails != null)
            {
                return Json(userDetails);
            }

            return NotFound("Usuario no encontrado");
        }

        [HttpPost]
        [Route("Home/CreateSalesforceAccount")]
        public async Task<IActionResult> CreateSalesforceAccount([FromBody] UserViewModel userDetails)
        {
            try
            {
                if (userDetails == null)
                {
                    return BadRequest("userDetails es nulo.");
                }
                Console.WriteLine($"Datos recibidos - Nombre: {userDetails._Name}, Email: {userDetails.Email}");

                string instanceUrl = "https://login.salesforce.com/services/oauth2/token";
                string clientId = "3MVG91oqviqJKoEGNcfx.RHrQ2kT3r2UIATQsGxLyYOnphRqR3uI0e_n_b5JSWSZ.IprmRDLHZtzJA2qnD5IT";
                string clientSecret = "E32E5D3802259D52446D93D095E6B0D9D29B8DE92A5353A0AAE6C20F2B24FFA0";
                string username = "noiremagg@hotmail.com";
                string password = "NoireSka:33" + "HhvEt2OaPqQ2BZOHeWv719XlN";

                var auth = new AuthenticationClient();
                await auth.UsernamePasswordAsync(clientId, clientSecret, username, password, instanceUrl);
                Console.WriteLine("Autenticación exitosa. Token de acceso: " + auth.AccessInfo.AccessToken);

                var client = new ForceClient(auth.AccessInfo.InstanceUrl, auth.ApiVersion, auth.AccessInfo.AccessToken);

                var account = new
                {
                    Name = userDetails._Name,
                    Email__c = userDetails.Email
                };

                var createResponse = await client.CreateRecord("Account", account);

                if (createResponse != null)
                {
                    return Ok(new { message = "Account created successfully!" });
                }
                else
                {
                    return Ok(new { message = "An account with this email already exists." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error during account creation: " + ex.Message });
            }
        }

    }
}
