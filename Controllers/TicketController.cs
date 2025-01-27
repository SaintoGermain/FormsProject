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

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketRequestModel ticketRequest)
        {
            var payload = new
            {
                fields = new
                {
                    project = new { key = "IT" }, // Clave del proyecto en Jira
                    summary = ticketRequest.Summary, // Resumen del ticket
                    description = new
                    {
                        type = "doc",
                        version = 1,
                        content = new[]
                        {
                    new
                    {
                        type = "paragraph",
                        content = new[]
                        {
                            new { type = "text", text = $"Reported by: {ticketRequest.CurrentUser}" },
                            new { type = "text", text = $"\nLink: {ticketRequest.Link}" },
                            new { type = "text", text = $"\nTemplate: {ticketRequest.CurrentTemplate}" }
                        }
                    }
                }
                    },
                    priority = new { name = ticketRequest.Priority },
                    issuetype = new { name = "Support Request" },
                }
            };
            Console.WriteLine("Payload a enviar: " + JsonSerializer.Serialize(payload));
            var client = new RestClient(jiraUrl);
            var request = new RestRequest("/rest/api/3/issue", Method.Post)
                .AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{email}:{apiToken}")))
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(payload);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                using var document = JsonDocument.Parse(response.Content);
                var root = document.RootElement;
                var ticketKey = root.GetProperty("key").GetString();

                return Ok(new { link = $"{jiraUrl}/browse/{ticketKey}" });
            }
            else
            {
                return BadRequest("Failed to create ticket: " + response.Content);
            }
        }



        [HttpGet("Ticket/GetFormTitle/{id}")]
        public IActionResult GetFormTitle(int id)
        {
            var form = _appDBContext.Forms.FirstOrDefault(f => f.NoForm == id);

            if (form == null)
            {
                
                return NotFound(new { Error = "The form wasn't found" });
            }
            return Json(new {form.Title });
        }

        [HttpGet("Ticket/GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var user = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if(user == null)
            {
                return NotFound(new { Error = "The user wasn't found" });

            };
            return Json(new {user});
        }

        [HttpGet]
        public async Task<IActionResult> UserTickets()
        {

            var client = new RestClient(jiraUrl);
            var request = new RestRequest("/rest/api/3/search", Method.Get)
                .AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{email}:{apiToken}")))
                .AddParameter("jql", "order by created DESC");

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                return BadRequest("Error retrieving tickets: " + response.Content);
            }

            var tickets = JsonSerializer.Deserialize<JsonElement>(response.Content);

            if (tickets.TryGetProperty("issues", out var issues))
            {
                var userTickets = new List<object>();
                var currentUser = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                foreach (var ticket in issues.EnumerateArray())
                {
                    var key = ticket.GetProperty("key").GetString();
                    var fields = ticket.GetProperty("fields");
                    if (fields.TryGetProperty("description", out var descriptionElement))
                    {
                        try
                        {
                            Console.WriteLine("Description content: " + descriptionElement);
                            if (descriptionElement.TryGetProperty("user", out var userProperty))
                            {
                                var user = userProperty.GetString();

                                if (user == currentUser)
                                {
                                    var summary = fields.GetProperty("summary").GetString();
                                    var status = fields.GetProperty("status").GetProperty("name").GetString();

                                    userTickets.Add(new
                                    {
                                        Key = key,
                                        Summary = summary,
                                        Status = status
                                    });
                                }
                            }
                            else
                            {
                                Console.WriteLine("Description JSON does not contain 'user'.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred while processing description: {ex.Message}");
                        }
                    }

                }

                return View(userTickets);
            }
            else
            {
                return BadRequest("No issues found in response.");
            }


        }

    }
}
