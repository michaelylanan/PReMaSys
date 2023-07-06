using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PReMaSys.Data;
using PReMaSys.ViewModel;

namespace PReMaSys.Controllers
{
    //[Authorize(Roles ="Domain")]
    public class ManageController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        //create constructore
        public ManageController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public ActionResult DisableExpiredUsers()
        {
            DateTime currentDate = DateTime.Now.Date;

            var expiredUsers = _context.ApplicationUsers
                .Where(u => u.DateExpiration < currentDate)
                .ToList();

            foreach (var user in expiredUsers)
            {
                // Perform the necessary action to disable user access based on the user ID.
                // For example, you can update the RoleId of the corresponding user in prmsUsersRoles table to null.

                UpdateUserAccess(user.Id);
            }

            // Return an appropriate response indicating the action was completed.
            return Content("Expired users have been disabled.");
        }

        // Helper method to update user access
        private void UpdateUserAccess(string userId)
        {
            var userRole = _context.UserRoles
                .SingleOrDefault(ur => ur.UserId == userId);

            if (userRole != null)
            {
                userRole.RoleId = null;
                _context.UserRoles.Update(userRole);
                _context.SaveChanges();
            }
        }


        public IActionResult ListAllRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult AddRoles()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoles(AddRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new()
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"No role with Id '{id}' was found";
                return View("Error");
            }
            EditRoleViewModel model = new()
            {
                Id = role.Id,
                RoleName = role.Name,
                Users = new List<string>()
            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name) && user.user == currentUser)
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"No Role with Id '{model.Id}' was found";
                return View("Error");
            }
            else
            {
                role.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if(role == null)
            {
                ViewData["ErrorMessage"] = $"No role with id '{id}' was found";
                return View("Error");
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRoles");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,error.Description);
                }
                return View(role);
            }
        }

            [HttpGet]
            public async Task<IActionResult> EditUsersInRole (string roleId)
            {
                ViewBag.roleId = roleId;   
            
                var role = await _roleManager.FindByIdAsync(roleId);

                var cUser = await _userManager.GetUserAsync(User);

                var model = new List<UserRoleViewModel>();

                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id '{roleId}' cannot be found";
                    return View("NotFound");
                }

                var rolesz = _context.UserRoles.Where(r => r.RoleId == roleId).Select(r => r.UserId);

                foreach (var user in _userManager.Users)
                {
                    if (user.user == cUser && rolesz.Contains(user.Id) || !user.IsChecked && user.user == cUser)
                    {

                        var userRoleViewModel = new UserRoleViewModel
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                        };

                        if (await _userManager.IsInRoleAsync(user, role.Name))
                        {
                            userRoleViewModel.IsSelected = true;
                        }
                        else
                        {
                            userRoleViewModel.IsSelected = false;
                        }

                        model.Add(userRoleViewModel);
                    }               
                }

                return View(model);
            }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id '{roleId}' cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                    user.IsChecked = true;
                    user.Role = role.Name;
                }
                else if (!model[i].IsSelected && (await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                    user.IsChecked = false;
                    user.Role = null;
                }
                else
                {
                    continue;
                }

                await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
            }
            return RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
