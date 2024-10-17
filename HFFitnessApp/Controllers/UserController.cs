using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HFFitnessApp.Models; // Adjust this namespace as needed to match your project
using System.Threading.Tasks;
using HFFitnessApp.Models;

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

        // Action method for the Sign-Up page
        public IActionResult Index()
        {
            return View();
        }

        // Register method for handling user sign-up
        [HttpPost]
        public async Task<IActionResult> Register(UserProfile model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Username, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("HealthData");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View("Index", model); // Return to Sign-Up page if registration fails
        }

        // Action method for displaying the Health Data Input page
        public IActionResult HealthData()
        {
            return View();
        }

        // Action method for saving health data
        [HttpPost]
        public async Task<IActionResult> SaveHealthData(HealthData model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = _userManager.GetUserId(User); // Associate health data with the current user
                _context.HealthData.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard"); // Redirect to a dashboard or confirmation page
            }
            return View("HealthData", model); // Return to Health Data page if validation fails
        }

        // Action method for the Sign-In page
        public IActionResult SignIn()
        {
            return View();
        }

        // SignIn method for handling user login
        [HttpPost]
        public async Task<IActionResult> SignIn(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard"); // Redirect to a dashboard or main page after successful sign-in
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }
    }
}
