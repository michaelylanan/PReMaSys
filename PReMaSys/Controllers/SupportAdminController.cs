using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PReMaSys.Data;
using PReMaSys.Models;
using PReMaSys.ViewModel;
using System.Data;
using System.Linq;

namespace PReMaSys.Controllers
{
    //[Authorize(Roles = "Support")]
    public class SupportAdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupportAdminController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult SupportPage() //Good
        {
            return View();
        }
        public IActionResult NotificationPage() //Good
        {
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));

            var list = _context.Purchase.ToList();
            return View(list);
        }

        public IActionResult EditStatus(int? id) //Good
        {
            if (id == null)
            {
                return RedirectToAction("NotificationPage");
            }

            //variable product that retrieves the existing record from the Rewards table.
            var status = _context.Purchase.Where(r => r.PurchaseId == id).SingleOrDefault();

            //if the reward record is not present the view will redirect to the Index action.
            if (status == null)
            {
                return RedirectToAction("IndexSE");
            }

            //Rewards model object will be included to be rendered by the View method
            return View(status);
        }

        [HttpPost]
        public IActionResult EditStatus(int? id, Purchase record) //Good
        {
            var status = _context.Purchase.Where(s => s.PurchaseId == id).SingleOrDefault();

            status.EmployeeName = record.EmployeeName;
            status.RewardName = record.RewardName;
            status.Stat = record.Stat;
            status.DateModified = DateTime.Now;
            _context.Purchase.Update(status);
            _context.SaveChanges();

            return RedirectToAction("NotificationPage");
        }



        /*New Methods*/
        public IActionResult EmployeeRole() //Good
        {
            var roles = _roleManager.Roles;

            return View(roles);
        }

        [HttpGet]
        public IActionResult ListSalesEmployee() //Good
        {
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            var list = _context.Users.Where(c => c.user == user).ToList();
            return View(list);
        }

        /*EDIT ADMIN ROLES Account------------------------------------------------*/
        [HttpGet]
        public async Task<IActionResult> EditSalesLC(string id) //Good
        {
            var se = await _userManager.FindByIdAsync(id);

            if (se == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(se);
            var userRoles = await _userManager.GetRolesAsync(se);

            var model = new EditSalesUserViewModel
            {
                Id = se.Id,
                Email = se.Email,
                UserName = se.UserName,
                /*Claims = User.Claims.Select(c => c.Value).ToList(),*/
                Roles = userRoles
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditSalesLC(EditSalesUserViewModel model) //Good
        {
            var se = await _userManager.FindByIdAsync(model.Id);

            if (se == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                se.UserName = model.Email;
                se.Email = model.Email;
                var result = await _userManager.UpdateAsync(se);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListSalesEmployee");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        //DELETE ADMIN ACCOUNT
        public async Task<IActionResult> DeleteSLC(string id) //Good
        {            
            var se = await _userManager.FindByIdAsync(id);

            /*var check = _context.SERecord.FirstOrDefault(s => s.SERId == se).SEmployeeRecordsID;
            rec = _context.SERecord.FirstOrDefault(r => r.SEmployeeRecordsID == check);*/

            /*var check = _context.SERecord.FirstOrDefault(s => s.SERId == se).SEmployeeRecordsID;*/
            var emp = _context.SERecord.Where(s => s.SERId == se).SingleOrDefault();

            if (User == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(se);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListSalesEmployee");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                
                _context.SERecord.Remove(emp);
                _context.SaveChanges();

                return View("ListAdminRoles");              
            }
        }


        /*CREATE NEW ADMIN ROLE*/
        public IActionResult SERecord() //Good
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SERecord(SalesUser se) //Good
        {
            
            ApplicationUser userz = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));

            var user = new ApplicationUser
            {
                UserName = se.Email,
                Email = se.Email,
                EmailConfirmed = true,
                user = userz,

            };
            if (se.Password == se.ConfirmPassword)
            {
                var insertrec = await _userManager.CreateAsync(user, se.Password);
                if (insertrec.Succeeded)
                {
                    TempData["ResultMessage"] = "The User \t " + se.Email + "\tIs Saved Succesfully..!!";
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
                TempData["ResultMessage2"] = "Password Mismatch";
            }

            var latest = user.Id;
            var getId = _context.ApplicationUsers.FirstOrDefault(u => u.Id == latest);
            var userzz = new SERecord   
            {
               /* SERId = latest,*/
                SERId = getId,
                EmployeeNo = se.EmployeeNo,
                EmployeeFirstname = se.EmployeeFirstname,
                EmployeeLastname = se.EmployeeLastname,
                EmployeeAddress = se.EmployeeAddress,
                EmployeeBirthdate = se.EmployeeBirthdate,
            };
            _context.SERecord.Add(userzz);
            _context.SaveChanges();

            return View();
        }
  
       
        /*REWARDS CRUD--------------------------------------------------------------------------------------------------------------------------------------------------------*/
        public IActionResult RewardsRecord() //Good
        {
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            var list = _context.Rewards.Where(c => c.ApplicationUser == user).ToList();
            return View(list);
        }

        //(1) Create Reward Information
        public IActionResult CreateR() //Good
        {
            return View();
        }

        //Overriding exising Create method through Replica
        [HttpPost]
        public IActionResult CreateR(Rewards record, IFormFile Picture) //Good
        {
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            var rewards = new Rewards()
            {
                ApplicationUser = user,
                RewardName = record.RewardName,
                Description = record.Description,
                RewardCost = record.RewardCost,
                PointsCost = record.PointsCost,
                DateAdded = DateTime.Now,
                Category = record.Category,
                Status = record.Status,
            };

            if (Picture != null)
            {
                if (Picture.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/rewards", Picture.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Picture.CopyTo(stream);
                    }
                    rewards.Picture = "~/img/rewards/" + Picture.FileName;
                }
            }

            //Add existing value 
            _context.Rewards.Add(rewards);
            _context.SaveChanges();

            return RedirectToAction("RewardsRecord");
        }

        //(2) Edit Reward Informaiton
        public IActionResult EditR(int? id) //Good
        {
            if (id == null)
            {
                return RedirectToAction("RewardsRecord");
            }

            //variable product that retrieves the existing record from the Rewards table.
            var rewards = _context.Rewards.Where(r => r.RewardsInformationId == id).SingleOrDefault();

            //if the reward record is not present the view will redirect to the Index action.
            if (rewards == null)
            {
                return RedirectToAction("RewardsRecord");
            }

            //Rewards model object will be included to be rendered by the View method
            return View(rewards);
        }
        //2.2 Override Existing Edit Method by a Replica using a nullable integer id and reward object as the parameters.
        [HttpPost]
        public IActionResult EditR(int? id, Rewards record) //Good
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

            return RedirectToAction("RewardsRecord");
        }

        public IActionResult DeleteR(int? id) //Good
        {
            if (id == null)
            {
                return RedirectToAction("RewardsRecord");
            }

            var rewards = _context.Rewards.Where(r => r.RewardsInformationId == id).SingleOrDefault();
            if (rewards == null)
            {
                return RedirectToAction("RewardsRecord");
            }
            _context.Rewards.Remove(rewards);
            _context.SaveChanges();

            return RedirectToAction("RewardsRecord");
        }
    }
}
