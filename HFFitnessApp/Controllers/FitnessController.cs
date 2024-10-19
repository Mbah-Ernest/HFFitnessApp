using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace FitnessApp.Controllers
{
    public class FitnessController : Controller
    {
        // Action method to render the input form
        public IActionResult Index()
        {
            return View();
        }

        // Action method to handle user input and call Gemini API
        [HttpPost]
        public async Task<IActionResult> GenerateWorkout(string fitnessGoal)
        {
            // Replace this with your actual Gemini AI API Key
            string apiKey = "AIzaSyCuzgLXTIh-Y7-FaaBnWNSGD_SXhMQRkFQ";

            // Gemini API endpoint
            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";

            // Create a new RestSharp client
            var client = new RestClient(apiUrl);
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/json");

            // Use fitnessGoal as part of the request body
            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"Create a workout plan for someone with the goal: {fitnessGoal}" }
                        }
                    }
                }
            };
            request.AddJsonBody(body);

            try
            {
                // Send the request and get the response
                var response = await client.ExecuteAsync(request);

                string generatedWorkout = "";

                if (response.IsSuccessful)
                {
                    // Parse the response content
                    var jsonResponse = JObject.Parse(response.Content);

                    // Extract the text content from the response
                    var textContent = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    if (!string.IsNullOrEmpty(textContent))
                    {
                        // Assign the generated workout to the view
                        generatedWorkout = textContent;
                    }
                    else
                    {
                        generatedWorkout = "No workout plan found in the response.";
                    }
                }
                else
                {
                    generatedWorkout = $"Error: {response.StatusCode} - {response.Content}";
                }

                // Pass the generated workout to the view
                ViewBag.GeneratedWorkout = generatedWorkout;
                return View("Result");
            }
            catch (Exception ex)
            {
                // Handle any exceptions and show error message
                ViewBag.GeneratedWorkout = $"Exception: {ex.Message}";
                return View("Result");
            }
        }
    }
}
