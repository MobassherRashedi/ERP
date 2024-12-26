using DevSkill.Inventory.Infrastructure.Identity;
using DevSkill.Inventory.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevSkill.Inventory.Web.Areas.Admin.Controllers
{
    [Area("Admin"),Authorize]
    public class MemberController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public MemberController(RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager) 
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddClaim()
        {
            var model = new ClaimAddModel();

            var users = from c in _userManager.Users.ToList() select c;
            model.UserId = users.First().Id;
            model.Users = new SelectList(users, "Id", "UserName");

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClaim(ClaimAddModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());
                await _userManager.AddClaimAsync(user, 
                    new System.Security.Claims.Claim(model.ClaimName, model.ClaimValue));
            }
            var users = from c in _userManager.Users.ToList() select c;
            model.UserId = users.First().Id;
            model.Users = new SelectList(users, "Id", "UserName");

            return View(model);
        }

        public IActionResult CreateRole()
        {
            var model = new RoleCreateModel();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleCreateModel model)
        {
            if(ModelState.IsValid)
            {
                await _roleManager.CreateAsync(new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    NormalizedName = model.Name.ToUpper(),
                    Name = model.Name,
                    ConcurrencyStamp = DateTime.UtcNow.Ticks.ToString()
                });
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ChangeRole()
        {
            var model = new RoleChangeModel();
            LoadValues(model);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(RoleChangeModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());
                var roles =  await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                var newRole = await _roleManager.FindByIdAsync(model.RoleId.ToString());
                await _userManager.AddToRoleAsync(user, newRole.Name);
            }
            LoadValues(model);
            return View(model);
        }

        private void LoadValues(RoleChangeModel model)
        {
            var users = from c in _userManager.Users.ToList() select c;
            var roles = from c in _roleManager.Roles.ToList() select c;

            model.UserId = users.First().Id;
            model.RoleId = roles.First().Id;

            model.Users = new SelectList(users, "Id", "UserName");
            model.Roles = new SelectList(roles, "Id", "Name");
        }
    }
}
