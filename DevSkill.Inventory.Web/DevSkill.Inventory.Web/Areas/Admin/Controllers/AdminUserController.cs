using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Infrastructure;
using DevSkill.Inventory.Infrastructure.Identity;
using DevSkill.Inventory.Web.Areas.Admin.Models; // Ensure the namespace for your view models
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Required for ToListAsync

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Web.Areas.Admin.Controllers
{
    [Area("admin"), Authorize]
    public class AdminUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AdminUserController> _logger;
        private readonly IUserService _userService;

        public AdminUserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ILogger<AdminUserController> logger, IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Method to get users in a format suitable for DataTables
        [HttpPost]
        public async Task<JsonResult> GetUsersJsonData([FromBody] UserWithRolesViewModel model)
        {
            var users = _userManager.Users.AsQueryable();

            // Implement filtering
            if (!string.IsNullOrEmpty(model.Search.Value))
            {
                var searchValue = model.Search.Value.ToLower();
                users = users.Where(u => (u.FirstName.ToLower() + " " + u.LastName.ToLower()).Contains(searchValue) ||
                                         u.Email.ToLower().Contains(searchValue));
            }

            // Count total records
            var recordsTotal = await _userManager.Users.CountAsync();

            // Count filtered records
            var recordsFiltered = await users.CountAsync();

            // Implement sorting
            string orderBy = model.FormatSortExpression("FirstName", "LastName", "Email", "Roles", "CreateDate") ?? "CreateDate desc";

            // Apply sorting based on FirstName and LastName to avoid exceptions
            if (orderBy == "FullName")
            {
                users = users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName);
            }
            else if (orderBy == "Roles")
            {
                // Get user roles to sort by
                var userRoles = await users.Select(u => new
                {
                    User = u,
                    Roles = _userManager.GetRolesAsync(u).Result // Fetch roles synchronously
                }).ToListAsync();

                users = userRoles.OrderBy(ur => string.Join(",", ur.Roles)).Select(ur => ur.User).AsQueryable();
            }
            else
            {
                users = users.OrderBy(orderBy); // Apply any other sorting logic
            }

            // Implement pagination
            var pagedUsers = await users.Skip((model.PageIndex - 1) * model.PageSize)
                                        .Take(model.PageSize)
                                        .ToListAsync();

            var localTimeZone = TimeZoneInfo.Local;

            // Prepare user data
            var userList = new List<UserWithRolesViewModel>();
            foreach (var user in pagedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                // Convert CreateDate from UTC to local time
                var localCreateDate = TimeZoneInfo.ConvertTimeFromUtc(user.CreateDate, localTimeZone);

                userList.Add(new UserWithRolesViewModel
                {
                    UserId = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    Roles = roles.ToList(),
                    CreatedDate = localCreateDate // Use the local time
                });
            }

            var result = new
            {
                recordsTotal,
                recordsFiltered,
                data = userList
            };

            return Json(result);
        }


        // GET: AdminUser/Create
        public async Task<IActionResult> Create()
        {
            var model = new UserCreateModel
            {
                AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()
            };
            return View("CreateUser", model);
        }

        // POST: AdminUser/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList(); // Repopulate available roles on validation failure
                return View(model);
            }

            // Create a new ApplicationUser instance
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                CreateDate = DateTime.UtcNow,
                IsActive = true
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

            // Create the user using the UserService
            var result = await _userService.CreateUserWithRoleAndPermissions(user, model.Roles, model.Password);

            if (result.Succeeded)
            {
                // Redirect to the user list or another appropriate page
                return RedirectToAction(nameof(Index));
            }

            // If user creation failed, add errors to the ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList(); // Repopulate available roles on failure
            return View(model);
        }



        // GET: Admin/User/Update/5
        public async Task<IActionResult> Update(Guid id)
        {
            // Log the ID to ensure it's being passed correctly
            Console.WriteLine($"User ID passed to Update: {id}");

            // Check if _userManager.Users contains the user
            var userExists = await _userManager.Users.AnyAsync(u => u.Id == id);
            if (!userExists)
            {
                Console.WriteLine("User not found in _userManager.Users");
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserUpdateModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive, 
                DateOfBirth = user.DateOfBirth, 
                Gender = user.Gender,
                ProfilePicturePath = !string.IsNullOrEmpty(user.ProfilePicture) ? user.ProfilePicture : "/images/user/user_default.jpg",
                AvailableRoles = await _roleManager.Roles.AsQueryable().Select(r => r.Name).ToListAsync(),
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            };

            return View("UpdateUser", model);
        }


        // POST: Admin/User/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update( UserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync(); // Repopulate roles
                return View("UpdateUser", model);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.IsActive = model.IsActive;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;
            

            // Handle the image upload
            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                var uploadsPath = Path.Combine("wwwroot", "images", "user");
                Directory.CreateDirectory(uploadsPath); // Ensure the directory exists
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ProfilePicture.FileName)}"; // Generate unique filename
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(fileStream);
                }

                // Update the user's profile picture path
                user.ProfilePicture = $"/images/user/{uniqueFileName}"; // Save the image URL or path
            }

            // Update the user's password if provided
            if (!string.IsNullOrEmpty(model.Password))
            {
                var passwordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
                user.PasswordHash = passwordHash; // Set the new password hash
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                // Update roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, model.Roles);

                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction("Index"); // Adjust to redirect to the desired action
            }

            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.AvailableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync(); // Repopulate roles on error
            return View("UpdateUser", model);
        }

      

        // Create new role
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError(string.Empty, "Role name is required.");
                return View();
            }

            var role = new ApplicationRole
            {
                Name = roleName
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Role created successfully",
                    Type = ResponseTypes.Success
                });
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        // Create new user
        public IActionResult CreateUser()
        {
            var availableRoles = _roleManager.Roles.Select(r => r.Name).ToList(); // Fetch available roles
            var model = new UserCreateModel
            {
                AvailableRoles = availableRoles // Pass the available roles to the view model
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    CreateDate = DateTime.UtcNow // Use UTC for consistency
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign roles if provided
                    if (model.Roles != null && model.Roles.Any())
                    {
                        var roleResult = await _userManager.AddToRolesAsync(user, model.Roles);
                        if (!roleResult.Succeeded)
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "User created successfully",
                        Type = ResponseTypes.Success
                    });
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // In case of error, repopulate available roles
            model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        // Delete action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // Get the currently logged-in user's ID
                var currentUserId = _userManager.GetUserId(User);

                var user = await _userManager.FindByIdAsync(id.ToString());

                // Check if the user is trying to delete themselves
                if (user != null && user.Id.ToString() == currentUserId)
                {
                    // Return an error message indicating that users cannot delete their own accounts
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "You cannot delete your own account.",
                        Type = ResponseTypes.Danger
                    });

                    return RedirectToAction("Index"); // or return an appropriate view
                }
                if (user == null)
                {
                    return NotFound();
                }
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "User deleted successfully",
                        Type = ResponseTypes.Success
                    });
                }
                else
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "User deletion failed",
                        Type = ResponseTypes.Danger
                    });
                    _logger.LogError("User deletion failed: {Errors}", string.Join(", ", result.Errors));
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user");
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "An error occurred while deleting the user.",
                    Type = ResponseTypes.Danger
                });
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    var idList = ids.Split(',').Select(Guid.Parse).ToList(); // Convert the comma-separated string into a list of GUIDs
                    foreach (var id in idList)
                    {
                        // Get the currently logged-in user's ID
                        var currentUserId = _userManager.GetUserId(User);

                        // Find the user by ID
                        var user = await _userManager.FindByIdAsync(id.ToString());

                        // Check if the user is trying to delete themselves
                        if (user != null && user.Id.ToString() == currentUserId)
                        {
                            // Return an error message indicating that users cannot delete their own accounts
                            TempData.Put("ResponseMessage", new ResponseModel
                            {
                                Message = "You cannot delete your own account.",
                                Type = ResponseTypes.Danger
                            });

                            return RedirectToAction("Index"); // or return an appropriate view
                        }
                        if (user != null)
                        {
                            // First, remove user roles
                            var roles = await _userManager.GetRolesAsync(user);
                            await _userManager.RemoveFromRolesAsync(user, roles);

                            var result = await _userManager.DeleteAsync(user);
                            if (!result.Succeeded)
                            {
                                // Log the errors if deletion fails
                                _logger.LogError("Failed to delete user {UserId}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
                            }
                        }
                        else
                        {
                            // Log if the user was not found
                            _logger.LogWarning("User {UserId} not found for deletion", id);
                        }
                    }

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Selected users deleted successfully",
                        Type = ResponseTypes.Success
                    });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "User deletion failed",
                    Type = ResponseTypes.Danger
                });
                _logger.LogError(ex, "Bulk user deletion failed");
            }

            return RedirectToAction("Index");
        }


    }
}
