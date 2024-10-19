using HFFitnessApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HFFitnessApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // This is the Sign-Up page (Register page)
        public IActionResult Index()
        {
            return View(); // Views/User/Index.cshtml (Sign-Up page)
        }

        // POST method to handle registration
        [HttpPost]
        public async Task<IActionResult> Register(UserProfile model)
        {
            if (ModelState.IsValid)
            {
                // Create Identity user
                var user = new IdentityUser { UserName = model.Username, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Save user profile in the custom UserProfiles table
                    model.Password = ""; // Do not store the plain password
                    _context.UserProfiles.Add(model);
                    await _context.SaveChangesAsync();

                    // Automatically sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to the Health Data page
                    return RedirectToAction("HealthData");
                }

                // Display any errors that occurred during sign-up
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View("Index", model); // Return to sign-up page if registration fails
        }

        // Sign-in page
        [AllowAnonymous]
        public IActionResult SignIn()
        {
            return View(); // Views/User/SignIn.cshtml
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            // Get the current logged-in user's ID
            var userId = _userManager.GetUserId(User);

            // Fetch the health data associated with the user
            var healthData = _context.HealthData.FirstOrDefault(h => h.UserId == userId);

            // If health data is not found, redirect to the HealthData form
            if (healthData == null)
            {
                return RedirectToAction("HealthData");
            }

            // Pass the health data to the Dashboard view
            return View(healthData);
        }


        // POST method for sign-in
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Get the current user's ID
                var userId = _userManager.GetUserId(User);

                // Check if health data exists for the user
                var healthDataExists = _context.HealthData.Any(h => h.UserId == userId);

                if (healthDataExists)
                {
                    // If health data exists, redirect to the Dashboard
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    // If no health data, redirect to Health Data input page
                    return RedirectToAction("HealthData");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }


        // Health Data page (after sign-in)
        [Authorize]
        public IActionResult HealthData()
        {
            return View(); // Views/User/HealthData.cshtml
        }

        // POST method to save health data
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveHealthData(HealthData model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "User");
            }


            // Get the current logged-in user's ID
            var userId = _userManager.GetUserId(User);

            // Check if the user is logged in
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("SignIn", "User");
            }

            // Assign the UserId to the health data model
            model.UserId = userId;

            // Remove ModelState error for UserId since we set it manually
            ModelState.Remove("UserId");

            // Validate the model state
            if (ModelState.IsValid)
            { 
                // Save the health data to the database
                _context.HealthData.Add(model);
                await _context.SaveChangesAsync();

                // Redirect to the dashboard after saving
                return RedirectToAction("Dashboard");
            }

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Any())
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                        }
                    }
                }
            }

            // If the model state is invalid, return the form with errors
            return View("HealthData", model);
        }
    }
}
