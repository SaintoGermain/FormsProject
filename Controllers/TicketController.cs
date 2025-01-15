using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Claims;

namespace FormsProyect.Controllers
{
    public class TicketController : Controller
    {
        private readonly string jiraUrl = "https://itraforms.atlassian.net";
        private readonly string apiToken = "ATATT3xFfGF0xNDSUWB5tjmPzyD3TQfoC3mb_o6Q8p4m1VH2fnRktvWfSv339aRdiFRmwRkYHkUyd8tZKrG_SE0FlRFmZCxW45FgRFqhCgN0AO-Viwq5h-v9Z1x-KUrK6TVBOwSnMnadU2-Z73qZgsSrHP5EV4WBdGRm6aGgyvSqXFP6nSOoo1Q=F3944709";
        private readonly string email = "noiremagg@hotmail.com";

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
    }
}
