using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;
using HFFitnessApp.Models;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> SavePlan(string workoutPlan, string mealPlan, string notes)
        {
            // Get the logged-in user's ID
            var userId = User?.Identity?.IsAuthenticated == true
                ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                : null;

            if (userId == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            // Save the workout and meal plan to the database
            var savedPlan = new SavedPlan
            {
                UserId = userId,
                WorkoutPlan = workoutPlan,
                MealPlan = mealPlan,
                Notes = notes
            };

            _context.SavedPlans.Add(savedPlan);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewSavedPlans");
        }

        [HttpPost]
        public IActionResult DeleteSavedPlan(int id)
        {
            // Find the saved plan by ID
            var savedPlan = _context.SavedPlans.FirstOrDefault(p => p.Id == id);

            if (savedPlan != null)
            {
                // Remove the plan from the database
                _context.SavedPlans.Remove(savedPlan);
                _context.SaveChanges(); // Save changes to the database

                TempData["Message"] = "Plan deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Plan not found!";
            }

            // Redirect back to the ViewSavedPlans page
            return RedirectToAction("ViewSavedPlans");
        }


        public async Task<IActionResult> ViewSavedPlans()
        {
            // Get the logged-in user's ID
            var userId = User?.Identity?.IsAuthenticated == true
                ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                : null;

            if (userId == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            // Retrieve saved plans for the logged-in user
            var savedPlans = await _context.SavedPlans
                .Where(sp => sp.UserId == userId)
                .ToListAsync();

            return View(savedPlans);
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
                            new
                            {
                                        text = $@"Create a detailed **workout plan** and **meal plan** for a user with the following details:
                                        - Age: {healthData.Age}
                                        - Height: {healthData.Height} cm
                                        - Weight: {healthData.Weight} kg
                                        - Gender: {healthData.Gender}
                                        - Main Fitness Goal: {healthData.FitnessGoal}
                                        - Fitness Goal: {fitnessGoal}
                                        - Dietary Preferences: {healthData.DietaryPreferences}
                                        - Food Preferences: {healthData.FoodPreferences}
                                        - Health Conditions: {healthData.HealthConditions}
                                        - Allergies: {healthData.Allergies}
                                        - Activity Level: {healthData.ActivityLevel}
                                        - Occupation: {healthData.Occupation}
                                        - Sleep Patterns: {healthData.SleepPatterns}
                                        - Injury History: {healthData.InjuryHistory}

                                        Please provide the response in a well-formatted and structured manner with sections, clear headings, and bullet points for readability. Each section (workout and meal plan) should be separate and concise.
                                        Don't write <br>, </br>, <b>. </b> in the text.

                                        Combine the food preference meals with verified ones. feel free to use other popular meals that related to the food preference
                                        YOUR Response should be in this format, Workout plan, meal plan and Note. don't use the details bolow. the code is an example for a neck size fitness goal. The words in the notes is liable to change, it sould be like an advice area(
        ========================================
                      WORKOUT PLAN
        ===============================================

        1. Warm-up:
           - 5 minutes of light cardio and dynamic stretching.

        2. Main Workout:
           - Neck Stretches (3 sets of 10 reps each):
             * Chin tucks
             * Neck rotations
             * Side neck stretches
           - Shoulder Exercises (3 sets of 12 reps each):
             * Lateral raises
             * Front raises
             * Shoulder shrugs
           - Cardio (20 minutes):
             * Jogging
             * Cycling
             * Swimming

        3. Cool-down:
           - 5 minutes of static stretching.

        ========================================
                      MEAL PLAN
        ===============================================

        1. Breakfast:
           * (The meal in less than 10 words).

        2. Lunch:
           * (The meal in less than 10 words).

        3. Dinner:
           * (The meal in less than 10 words).

        4. Snacks:
           * (The meal in less than 10 words).

        5. Dietary Recommendations:
           * (Recomendation in less than 20 words).
           * (Recomendation in less than 20 words).
           * (Recomendation in less than 20 words).

        ----------------------------------------
        NOTE: 
        (A quick short note note less than 20 words, not more than 40 words)
        ----------------------------------------
        )
                                        **Workout Plan**:
                                        1. Warm-up: Specify exercises and time (e.g., 10 minutes of dynamic stretching) NOT MORE THAN 10 WORDS
                                        2. Main Workout: Exercises with sets, reps, and proper structure (e.g., 3 sets of 10 push-ups) NOT MORE THAN 10 WORDS
                                        3. Cool-down: Specify exercises and time (e.g., 5 minutes of static stretching) NOT MORE THAN 10 WORDS

                                        **Meal Plan**: (Use native names only for native foods)
                                        1. Breakfast: Example meal (e.g., scrambled eggs with avocado toast) - NOT MORE THAN 10 WORDS
                                        2. Lunch: Example meal (e.g., grilled chicken with vegetables) - NOT MORE THAN 10 WORDS
                                        3. Dinner: Example meal (e.g., salmon with quinoa and salad) - NOT MORE THAN 10 WORDS
                                        4. Snacks: Example snacks between meals (e.g., fruit, nuts) - NOT MORE THAN 10 WORDS
                                        5. Dietary Recommendations: Specific advice based on the user's dietary preferences and health conditions. - NOT MORE THAN 20 WORDS"
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

                string textContent = "";

                if (!string.IsNullOrEmpty(response.Content))
                {
                    // Parse the response content
                    var jsonResponse = JObject.Parse(response.Content);

                    // Extract the text content from the response
                    textContent = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    if (!string.IsNullOrEmpty(textContent))
                    {
                        // Format the response before extracting sections
                        string formattedContent = FormatResponse(textContent);

                        // Extract sections
                        string workoutPlan, mealPlan, notes;
                        ExtractSections(formattedContent, out workoutPlan, out mealPlan, out notes);

                        // Pass the extracted sections to the view
                        ViewBag.WorkoutPlan = workoutPlan;
                        ViewBag.MealPlan = mealPlan;
                        ViewBag.Notes = notes;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No workout plan found in the response.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = $"Error: {response.StatusCode} - {response.Content}";
                }

                return View("Result");
            }
            catch (Exception ex)
            {
                // Handle any exceptions and show error message
                ViewBag.ErrorMessage = $"Exception: {ex.Message}";
                return View("Result");
            }
        }

        // Helper method to format the response
        private string FormatResponse(string textContent)
        {
            // Apply simple text-based formatting for sections marked by ** (e.g., **Workout Plan**)
            textContent = Regex.Replace(textContent, @"\*\*(.+?)\*\*", "$1:");

            // Replace the equals signs (===) sections with proper section dividers
            textContent = Regex.Replace(textContent, @"={20,}", "\r\n========================================\r\n");

            // Trim leading and trailing spaces and ensure only one newline between sections for readability
            textContent = Regex.Replace(textContent, @"\n{2,}", "\r\n");

            // Optionally, format lists for better visual presentation (replace * with dash for bullet points)
            textContent = textContent.Replace("* ", "- ");

            // Ensure newlines between numbered sections (1., 2., 3., etc.) for better readability without excessive spacing
            textContent = Regex.Replace(textContent, @"\n{2,}(\d\.)", "\r\n$1");

            // Remove any trailing newlines for a cleaner output
            textContent = textContent.TrimEnd();

            return textContent;
        }


        private void ExtractSections(string textContent, out string workoutPlan, out string mealPlan, out string notes)
        {
            // Initialize the variables
            workoutPlan = "";
            mealPlan = "";
            notes = "";

            // Define patterns to extract the sections using Regex
            string workoutPattern = @"Workout Plan([\s\S]+?)(?=Meal Plan)";
            string mealPattern = @"Meal Plan([\s\S]+?)(?=NOTE)";
            string notePattern = @"NOTE:([\s\S]+)";

            // Extract the Workout Plan
            var workoutMatch = Regex.Match(textContent, workoutPattern, RegexOptions.IgnoreCase);
            if (workoutMatch.Success)
            {
                workoutPlan = workoutMatch.Value.Trim();
            }

            // Extract the Meal Plan
            var mealMatch = Regex.Match(textContent, mealPattern, RegexOptions.IgnoreCase);
            if (mealMatch.Success)
            {
                mealPlan = mealMatch.Value.Trim();
            }

            // Extract the Note section
            var noteMatch = Regex.Match(textContent, notePattern, RegexOptions.IgnoreCase);
            if (noteMatch.Success)
            {
                notes = noteMatch.Value.Trim();
            }
        }

        // Method to extract text between Meal Plan numbers
        private void ExtractMealSteps(string mealPlan, out string step1, out string step2, out string step3, out string step4, out string afterStep5)
        {
            // Initialize the variables
            step1 = "";
            step2 = "";
            step3 = "";
            step4 = "";
            afterStep5 = "";

            // Define the regular expression patterns for the steps
            string step1Pattern = @"1\.\s*([\s\S]+?)(?=2\.)";
            string step2Pattern = @"2\.\s*([\s\S]+?)(?=3\.)";
            string step3Pattern = @"3\.\s*([\s\S]+?)(?=4\.)";
            string step4Pattern = @"4\.\s*([\s\S]+?)(?=5\.)";
            string afterStep5Pattern = @"5\.\s*([\s\S]+)";

            // Match the text between "1." and "2."
            var step1Match = Regex.Match(mealPlan, step1Pattern);
            if (step1Match.Success)
            {
                step1 = step1Match.Groups[1].Value.Trim();
            }

            // Match the text between "2." and "3."
            var step2Match = Regex.Match(mealPlan, step2Pattern);
            if (step2Match.Success)
            {
                step2 = step2Match.Groups[1].Value.Trim();
            }

            // Match the text between "3." and "4."
            var step3Match = Regex.Match(mealPlan, step3Pattern);
            if (step3Match.Success)
            {
                step3 = step3Match.Groups[1].Value.Trim();
            }

            // Match the text between "4." and "5."
            var step4Match = Regex.Match(mealPlan, step4Pattern);
            if (step4Match.Success)
            {
                step4 = step4Match.Groups[1].Value.Trim();
            }

            // Capture any text after the last step ("5.")
            var afterStep5Match = Regex.Match(mealPlan, afterStep5Pattern);
            if (afterStep5Match.Success)
            {
                afterStep5 = afterStep5Match.Groups[1].Value.Trim();
            }
        }
    }
}
