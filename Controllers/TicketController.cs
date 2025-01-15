using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Claims;
using FormsProyect.ViewModels;
using FormsProyect.Data;

namespace FormsProyect.Controllers
{
    public class TicketController : Controller
    {
        private readonly string jiraUrl = "https://itraforms.atlassian.net";
        private readonly string apiToken = "";
        private readonly string email = "noiremagg@hotmail.com";

        private readonly AppDBContext _appDBContext;
        public TicketController(AppDBContext appDbContext)
        {
            _appDBContext = appDbContext;
        }

        //[HttpGet]
        //public async Task<IActionResult> CreateTicket()
        //{
        //    var user = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        //    var template = _appDBContext.Forms.Select(t => t.NoForm).ToList();

        //    var ticket = new FormViewModel
        //    {
        //        _Name = user,

        //    };
        //}

        [HttpPost]
        public async Task<IActionResult> CreateTicket(string summary, string priority, string link, string currentUser, string currentStatus, string currentTemplate)
        {
            var user = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            var payload = new
            {
                fields = new
                {
                    project = new { key = "IT" },
                    summary = summary,
                    description = $"Reported by: {user}\nLink: {link}",
                    priority = new { name = priority },
                }
            };

            var client = new RestClient(jiraUrl);
            var request = new RestRequest("/rest/api/3/issue", Method.Post)
                .AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{email}:{apiToken}")))
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(payload);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var ticketData = JsonSerializer.Deserialize<dynamic>(response.Content);
                return Ok(new { link = $"{jiraUrl}/browse/{ticketData?.id}" });
            }
            else
            {
                return BadRequest("Failed to create ticket: " + response.Content);
            }
        }

        [HttpGet("Ticket/GetFormTitle/{id}")]
        public IActionResult GetFormTitle(int id)
        {
            // Busca el formulario en la base de datos
            var form = _appDBContext.Forms.FirstOrDefault(f => f.NoForm == id);

            if (form == null)
            {
                return NotFound("Formulario no encontrado.");
            }

            return Json(new { Title = form.Title });
        }



    }
}
