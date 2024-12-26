using DevSkill.Inventory.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DevSkill.Inventory.Web.Models;
using DevSkill.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using DevSkill.Inventory.Infrastructure;
using System.Security.Claims;
using DevSkill.Inventory.Application.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserProfileManagementService _userProfileManagementService;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailUtility _emailUtility;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IEmailUtility emailUtility,
            IUserProfileManagementService userProfileManagementService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailUtility = emailUtility;
            _context = context;
            _userProfileManagementService = userProfileManagementService;

        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(string returnUrl = null)
        {
            var model = new RegistrationModel();
            model.ReturnUrl = returnUrl;
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(RegistrationModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email.Trim(),
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    PhoneNumber = model.PhoneNumber,
                    IsActive = false // Consider setting this to false if you want to restrict access until confirmation
                };

                // Save Profile Picture if provided
                if (model.ProfilePicture != null)
                {
                    var uploadsPath = Path.Combine("wwwroot", "images", "user");
                    Directory.CreateDirectory(uploadsPath); // Ensure the directory exists
                    var uniqueFileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                    var filePath = Path.Combine(uploadsPath, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(fileStream);
                    }

                    user.ProfilePicture = $"/images/user/{uniqueFileName}";
                }
                else
                {
                    user.ProfilePicture = "/images/user/user_default.jpg"; // Default profile picture
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add user to the "Member" role
                    await _userManager.AddToRoleAsync(user, "Member");

                    // Generate email confirmation token
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Action("ConfirmEmail",
                        "Account",
                        values: new { area = "", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    // Send confirmation email
                     _emailUtility.SendEmail(
                        model.Email,
                        model.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                    );

                    // Redirect to confirmation view instead of logging in
                    return RedirectToAction("RegisterConfirmation", new { email = model.Email, returnUrl = model.ReturnUrl });

                }

                // Handle errors if the user creation failed
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Return the view with the model if there are validation errors
            return View(model);
        }



        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(string returnUrl = null)
        {
            var model = new SigninModel();
            model.ReturnUrl = returnUrl ?? Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<IActionResult> LoginAsync(SigninModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Attempt to sign in the user
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Fetch user details immediately after login
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    // Check if email is confirmed
                    if (!user.EmailConfirmed)
                    {
                        // Redirect to email confirmation page if email is not confirmed
                        return RedirectToAction("EmailConfirmationRequired", new { email = model.Email });
                    }

                    // Check if the user is active
                    if (!user.IsActive)
                    {
                        // Log inactive user login attempt and show an error
                        Console.WriteLine($"Inactive user {model.Email} attempted to log in.");
                        await _signInManager.SignOutAsync(); // Sign out any temporary session
                        ModelState.AddModelError(string.Empty, "Your account is inactive. Please contact support.");
                        return View(model);
                    }

                    // Log successful login (optional)
                    Console.WriteLine($"User {model.Email} logged in successfully.");

                    return LocalRedirect(model.ReturnUrl);
                }

                // Handle two-factor authentication
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
                }

                // Handle locked accounts
                if (result.IsLockedOut)
                {
                    return RedirectToAction("Lockout");
                }

                // Log invalid login attempt for monitoring
                Console.WriteLine($"Invalid login attempt for user {model.Email}.");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            else
            {
                // Log model state errors if the model is not valid
                Console.WriteLine("Model state is not valid.");
            }

            return View(model);
        }




        public async Task<IActionResult> LogoutAsync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Decode the email confirmation code
            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            // Confirm the email
            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);

            if (result.Succeeded)
            {
                // Update IsActive property to true upon successful email confirmation
                user.IsActive = true;
                await _userManager.UpdateAsync(user);
            }

            // Populate the ConfirmEmailModel and pass it to the view
            var model = new ConfirmEmailModel
            {
                Email = user.Email,
                IsConfirmed = result.Succeeded
            };

            return View(model);
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login"); // Redirect if email is missing
            }

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || await _userManager.IsEmailConfirmedAsync(user))
            {
                // Redirect if user is not found or email is already confirmed
                return RedirectToAction("Login");
            }

            // Generate a new email confirmation token
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Action("ConfirmEmail",
                "Account",
                values: new { userId = user.Id, code = code },
                protocol: Request.Scheme);

            // Send the confirmation email
            _emailUtility.SendEmail(
                user.Email,
                user.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
            );

            return RedirectToAction("RegisterConfirmation", new { email = user.Email });
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterConfirmation(string email, string returnUrl = null)
        {
            // Pass email and returnUrl to the view
            ViewData["Email"] = email;
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult EmailConfirmationRequired(string email)
        {
            ViewBag.Email = email;
            return View();
        }



        // Profile Management

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Return the profile view with the user's data
            return View(user);
        }

        // GET: /Account/ProfileEdit
        [HttpGet]
        public async Task<IActionResult> ProfileEdit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileUpdateModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileEdit(ProfileUpdateModel model)

        {
            model.FullName = $"{model.FirstName} {model.LastName}";
            if (!ModelState.IsValid)
            {
                return View(model); // Return the view with validation errors if the model state is invalid
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Update user properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;

            // Update profile picture if provided
            if (model.ProfilePicture != null)
            {
                var uploadsPath = Path.Combine("wwwroot", "images", "user");
                Directory.CreateDirectory(uploadsPath); // Ensure the directory exists
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ProfilePicture.FileName)}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(fileStream);
                }

                // Update the user's profile picture path
                user.ProfilePicture = $"/images/user/{uniqueFileName}";
            }

            // Save the updated user data
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Optionally, re-sign in the user to refresh claims if needed
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }

            // If updating fails, add errors to the model state
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model); // Return the view with any errors if the update was unsuccessful
        }



    }
}
