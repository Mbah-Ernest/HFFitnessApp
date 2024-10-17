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

        // Action method to handle user input and call Hugging Face API
        [HttpPost]
        public async Task<IActionResult> GenerateWorkout(string fitnessGoal)
        {
            // Replace with your Hugging Face API Key
            string apiKey = "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";

            // Model and API setup
            string model = "facebook/blenderbot-400M-distill";
            string apiUrl = $"https://api-inference.huggingface.co/models/{model}";

            //// Prepare the client and request
            //var client = new RestClient(apiUrl);
            //var request = new RestRequest();
            //request.Method = Method.Post;
            //request.AddHeader("Authorization", $"Bearer {apiKey}");
            //request.AddHeader("Content-Type", "application/json");

            //// Use fitnessGoal as part of the prompt
            //var prompt = new { inputs = $"Create a workout plan for someone with the goal: {fitnessGoal}" };
            //request.AddJsonBody(prompt);

            //// Send the request
            //var response = await client.ExecuteAsync(request);
            //string generatedWorkout = "";

            //if (response.IsSuccessful)
            //{
            //    var jsonResponse = JArray.Parse(response.Content);
            //    generatedWorkout = jsonResponse[0]["generated_text"].ToString();
            //}
            //else
            //{
            //    generatedWorkout = "Error generating workout plan.";
            //}

            string generatedWorkout = "Sample workout plan for UI development: \n1. Warm-up for 10 minutes \n2. 3 sets of push-ups \n3. 3 sets of squats \n4. Cool down for 5 minutes";

            // Pass the response to the view
            ViewBag.GeneratedWorkout = generatedWorkout;
            return View("Result");
        }
    }
}
