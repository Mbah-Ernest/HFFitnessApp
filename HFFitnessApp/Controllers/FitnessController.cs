using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;
using HFFitnessApp.Models;

namespace FitnessApp.Controllers
{
    public class FitnessController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FitnessController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action method to render the input form
        public IActionResult Index()
        {
            return View();
        }

        // Action method to handle user input and call Gemini API
        [HttpPost]
        public async Task<IActionResult> GenerateWorkout(string fitnessGoal)
        {
            // Get the logged-in user's ID
            var userId = User?.Identity?.IsAuthenticated == true ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value : null;

            if (userId == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            // Fetch the user's health data from the database
            var healthData = _context.HealthData.FirstOrDefault(h => h.UserId == userId);
            if (healthData == null)
            {
                return RedirectToAction("HealthData", "User"); // Redirect to input health data if not found
            }

            // Replace this with your actual Gemini AI API Key
            string apiKey = "AIzaSyCuzgLXTIh-Y7-FaaBnWNSGD_SXhMQRkFQ";

            // Gemini API endpoint
            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";

            // Create a new RestSharp client
            var client = new RestClient(apiUrl);
            var request = new RestRequest();
            request.Method = Method.Post;  // Correct usage of Method.Post
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
                            new
                            {
                                text = $@"Create a detailed **workout plan** and **meal plan** for a user with the following details:
                                - Age: {healthData.Age}
                                - Height: {healthData.Height} cm
                                - Weight: {healthData.Weight} kg
                                - Gender: {healthData.Gender}
                                - Fitness Goal: {fitnessGoal}
                                - Dietary Preferences: {healthData.DietaryPreferences}
                                - Food Preferences: {healthData.FoodPreferences}
                                - Health Conditions: {healthData.HealthConditions}
                                - Allergies: {healthData.Allergies}
                                - Activity Level: {healthData.ActivityLevel}
                                - Occupation: {healthData.Occupation}
                                - Sleep Patterns: {healthData.SleepPatterns}
                                - Injury History: {healthData.InjuryHistory}

                                Please provide a clear and structured response in the following format:

                                **Workout Plan**:
                                1. Warm-up: Specify exercises and time
                                2. Main Workout: List exercises with sets and repetitions
                                3. Cool-down: Specify exercises and time

                                **Meal Plan**:
                                1. Breakfast: Example meal
                                2. Lunch: Example meal
                                3. Dinner: Example meal
                                4. Snacks: Suggested snacks between meals
                                5. Dietary Recommendations: Specific advice based on the user's dietary preferences and health conditions."
                            }
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

                if (!string.IsNullOrEmpty(response.Content))
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
