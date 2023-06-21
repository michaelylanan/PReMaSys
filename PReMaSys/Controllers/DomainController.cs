using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using PReMaSys.Data;
using PReMaSys.Models;
using PReMaSys.ViewModel;
using System.Data;
using System.Linq;

namespace PReMaSys.Controllers
{
   // [Authorize(Roles = "Domain")]
    public class DomainController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DomainController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult DomainPage()
        {
            return View();
        }

        //Reports
        public IActionResult ReportsPage() 
        {
            return View();
        }       

        public IActionResult Ranking() 
        {
            return View();
        }

        public IActionResult Forecasts() 
        {
            return View();
        }

        public IActionResult Diagnostic() 
        {
            return View();
        }

        public IActionResult Descriptive() 
        {
            return View();
        }

        /*APPROVAL OF REWARDS-------------------------------------------------------------------------------------------------------------------------------------*/
        public IActionResult ApproveRewards(string id)
        {
           /* ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));

            //Get all administrators
            var ad = _context.ApplicationUsers.FirstOrDefault(a => a.user == user).Id;

            //Get ID From Roles
            var sRole = _context.Roles.FirstOrDefault(r => r.Name.Contains("Support")).Id;

            //Get UserId From UserRoles
            var checkz = _context.UserRoles.FirstOrDefault(c=> c.RoleId == sRole).UserId;*/

            /*list = _context.Rewards.Where(l => l.ApplicationUser.Id == checkz).ToList();*/

            //Get Reward List
            var list = _context.Rewards.ToList();
            return View(list);

        }

        public IActionResult ApproveR(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ApproveRewards");
            }

            var rewards = _context.Rewards.Where(r => r.RewardsInformationId == id).SingleOrDefault();

            if (rewards == null)
            {
                return RedirectToAction("ApproveRewards");
            }

            return View(rewards);
        }

        [HttpPost]
        public IActionResult ApproveR(int? id, Rewards record)
        {
            var rewards = _context.Rewards.Where(r => r.RewardsInformationId == id).SingleOrDefault();
            rewards.Picture = record.Picture;
            rewards.RewardName = record.RewardName;
            rewards.Description = record.Description;
            rewards.RewardCost = record.RewardCost;
            rewards.PointsCost = record.PointsCost;
            rewards.DateModified = DateTime.Now;
            rewards.Category = record.Category;
            rewards.Status = record.Status;

            _context.Rewards.Update(rewards);
            _context.SaveChanges();

            return RedirectToAction("ApproveRewards");
        }

        /* ------------------------------------------------------------------------------------------------------------------------------------------------*/
        public IActionResult AdminAllRoles()
        {
            var roles = _roleManager.Roles;

            return View(roles);
        }

        /*ADMIN ROLES CRUD---------------------------------------------------------------------------------------------------------------------------------*/

        [HttpGet]
        public IActionResult ListAdminRoles()
        {
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            var list = _context.Users.Where(c => c.user == user).ToList();
            return View(list);
        }



        /*CREATE NEW ADMIN ROLE*/
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddRole(AdminUser admin)
        {
            ApplicationUser userz = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));

            var user = new ApplicationUser
            {
                UserName = admin.Email,
                Email = admin.Email,
                EmailConfirmed = true,
                user = userz
            };
            if(admin.Password == admin.ConfirmPassword)
            {
                var insertrec = await _userManager.CreateAsync(user, admin.Password);
                if (insertrec.Succeeded)
                {
                    ViewBag.message = "The User \t " + admin.Email + "\tIs Saved Succesfully..!!";
                }
                else
                {
                    foreach (var error in insertrec.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ViewBag.message2 = "Password Mismatch";
            }
           

            return View();
        }

        /*EDIT ADMIN ROLES Account------------------------------------------------*/
        [HttpGet]
        public async Task<IActionResult> EditAdminRole(string id)
        {
            var admin = await _userManager.FindByIdAsync(id);

            if (admin == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(admin);
            var userRoles = await _userManager.GetRolesAsync(admin);

            var model = new EditAdminUserViewModel
            {
                Id = admin.Id,
                Email = admin.Email,
                UserName = admin.UserName,
                /*Claims = User.Claims.Select(c => c.Value).ToList(),*/
                Roles = userRoles
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAdminRole(EditAdminUserViewModel model)
        {

            var admin = await _userManager.FindByIdAsync(model.Id);

            if (admin == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                admin.UserName = model.Email;
                admin.Email = model.Email;

                //Password
               /* var insertrec = await _userManager.ChangePasswordAsync(admin, model.Password,model.Password);
                if (insertrec.Succeeded)
                {
                    ViewBag.message = "Successfully Updated";
                }
                else
                {
                    foreach (var error in insertrec.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }*/

                var result = await _userManager.UpdateAsync(admin);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListAdminRoles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        //DELETE ADMIN ACCOUNT
        public async Task<IActionResult> DeleteAdminAccount(string id)
        {
            var admin = await _userManager.FindByIdAsync(id);
            if (User == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                admin.IsArchived = DateTime.Now;
                var result = await _userManager.DeleteAsync(admin);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListAdminRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListAdminRoles");
            }
        }
       

        /*SALES-POINTS PROFIT CRUD---------------------------------------------------------------------------------------------------------------------------------*/

        public IActionResult SEPoints() 
        {
            var list = _context.SERecord.ToList();
            return View(list);
        }

    }
}
