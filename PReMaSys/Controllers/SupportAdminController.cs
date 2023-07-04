using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using OfficeOpenXml;
using PReMaSys.Data;
using PReMaSys.Models;
using PReMaSys.ViewModel;
using System.Data;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text;

namespace PReMaSys.Controllers
{
    [Authorize(Roles = "Support")]
    public class SupportAdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public SupportAdminController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult SupportPage() //Good
        {
            int notificationCount = _context.Purchase
                .Count(p => p.DateModified == null);

            // Pass the count to the view
            ViewData["NotificationCount"] = notificationCount;
            return View();
        }
        public IActionResult NotificationPage() //Good
        {
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));

            var list = _context.Purchase
             .Where(p => p.DateModified == null)
             .ToList();

            return View(list);
        }

        public IActionResult EditStatus(int? id) //Good
        {
            if (id == null)
            {
                return RedirectToAction("NotificationPage");
            }

            var status = _context.Purchase.Where(r => r.PurchaseId == id).SingleOrDefault();

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
            var list = _context.Users.Where(c => c.user == user && c.IsArchived == null).ToList();
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

            var userRecord = await _context.Users.FindAsync(se.Id);
            var emp = _context.SERecord.SingleOrDefault(s => s.SERId == userRecord);

            if (User == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                se.IsArchived = DateTime.Now;
                var result = await _userManager.UpdateAsync(se);

                if (result.Succeeded)
                {
                    emp.IsArchived = DateTime.Now;
                    _context.SERecord.Update(emp);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ListSalesEmployee");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                
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
            string returnUrl = Url.Content("~/");

            var cUser = await _userManager.GetUserAsync(User);

            var user = new ApplicationUser
            {
                UserName = se.Email,
                Email = se.Email,
                user = cUser,

            };
            if (se.Password == se.ConfirmPassword)
            {
                var insertrec = await _userManager.CreateAsync(user, se.Password);
                if (insertrec.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //Email
                    await _emailSender.SendEmailAsync(se.Email, "Confirm your email",
                      $"<table class=\"wrapper\" role=\"module\" data-type=\"image\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"table-layout: fixed;\" data-muid=\"84b7c821-5416-4af9-b441-8e85085affb3\">\r\n    <tbody>\r\n        <tr>\r\n            <td style=\"font-size:6px; line-height:10px; padding:0px 0px 0px 0px;\" valign=\"top\" align=\"center\">\r\n                <img class=\"max-width\" border=\"0\" style=\"display:block; color:#000000; text-decoration:none; font-family:Helvetica, arial, sans-serif; font-size:16px; max-width:25% !important; width:25%; height:auto !important;\" width=\"240\" alt=\"\" data-proportionally-constrained=\"true\" data-responsive=\"true\" src=\"http://cdn.mcauto-images-production.sendgrid.net/ba5ff8c16d24e60e/55096f2c-876a-4cc2-8e2e-6832cbb37c14/500x500.png\">\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>" +
                      $"<table data-type=\"text\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"50%\" style=\"table-layout: fixed; margin-right:auto; margin-left:auto\" data-muid=\"d2b3a335-427c-4e78-b490-99e4b86ff853\" data-mc-module-version=\"2019-10-22\">\r\n    <tbody>\r\n        <tr>\r\n            <td style=\"padding:18px 0px 18px 0px; line-height:22px; text-align:inherit;\" height=\"100%\" valign=\"top\" bgcolor=\"\" role=\"module-content\">\r\n                <div>\r\n                    <h2>\r\n                        <span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 24px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial;text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing:0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top; line-height: 1.5\">Greetings and welcome to PReMaSys!</span>\r\n                        <div style=padding:5px></div>\r\n                        <div style=\"font-family: inherit; text-align: inherit; margin-left: 0px\"><span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing: 0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top\">  We’re so happy that you’re here and can’t wait to help you reach your goals when it comes to managing your sales employees, tracking their performance, boosting their morale, and recognizing their efforts. We can’t wait to finally have you in our community, but first, there are a few pre-screening steps you must complete to get started with the web application. You can start by doing this task./span>&nbsp;</div><div></div>\r\n                        <br>\r\n                        <span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 24px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing: 0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top; line-height: 1.5\">Let's confirm your email address.</span>\r\n                    </h2>\r\n                    <div style=\"font-family: inherit; text-align: inherit; margin-left: 0px\"><span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing: 0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top\">By clicking on the following link, you are confirming your email address.</span>&nbsp;</div><div></div>\r\n                </div>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"module\" data-role=\"module-button\" data-type=\"button\" role=\"module\" style=\"table-layout:fixed;\" width=\"100%\" data-muid=\"1c7c9076-cfcb-4233-a2e4-731694c53ed8\">\r\n    <tbody>\r\n        <tr>\r\n            <td align=\"center\" bgcolor=\"\" class=\"outer-td\" style=\"padding:0px 0px 0px 0px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"wrapper-mobile\" style=\"text-align:center;\">\r\n                    <tbody>\r\n                        <tr>\r\n                            <td align=\"center\" bgcolor=\"#55d97e\" class=\"inner-td\" style=\"border-radius:6px; font-size:16px; text-align:center; background-color:inherit;\">\r\n" +
                      $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style=\"background-color:#55d97e; border:1px solid #09d463; border-color:#09d463; border-radius:6px; border-width:1px; color:#ffffff; display:inline-block; font-size:14px; font-weight:normal; letter-spacing:0px; line-height:normal; padding:12px 18px 12px 18px; text-align:center; text-decoration:none; border-style:solid;\" target=\"_blank\">Click Here</a>\r\n                            </td>\r\n                        </tr>\r\n                    </tbody>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n<table class=\"module\" role=\"module\" data-type=\"text\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"table-layout: fixed;\" data-muid=\"952ac095-dfb9-4cd3-85ba-08f4779d7c91\" data-mc-module-version=\"2019-10-22\">\r\n    <tbody>\r\n        <tr>\r\n            <td style=\"padding:18px 0px 18px 0px; line-height:22px; text-align:inherit;\" height=\"100%\" valign=\"top\" bgcolor=\"\" role=\"module-content\"><div><div style=\"font-family: inherit; text-align: center\"><span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 12px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: center; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(253, 253, 253); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; float: none; display: inline\">© PreMaSys Inc. 2544 Taft Ave, Malate, Manila, 1004 Metro Manila</span></div><div></div></div></td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n\r\n<table class=\"module\" role=\"module\" data-type=\"social\" align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"table-layout: fixed;\" data-muid=\"ea5afea2-fe0a-4d3d-9aaa-bc20bdd0375c\">\r\n    <tbody>\r\n        <tr>\r\n            <td valign=\"top\" style=\"padding:0px 0px 0px 0px; font-size:6px; line-height:10px;\" align=\"center\">\r\n                <table align=\"center\" style=\"-webkit-margin-start:auto;-webkit-margin-end:auto;\">\r\n                    <tbody>\r\n                        <tr align=\"center\">\r\n                            <td style=\"padding: 0px 5px;\" class=\"social-icon-column\">\r\n                                <a role=\"social-icon-link\" href=\"https://www.facebook.com/ylananjr.michael/\" target=\"_blank\" alt=\"Facebook\" title=\"Facebook\" style=\"display:inline-block; background-color:#3B579D; height:21px; width:21px;\">\r\n                                    <img role=\"social-icon\" alt=\"Facebook\" title=\"Facebook\" src=\"https://mc.sendgrid.com/assets/social/white/facebook.png\" style=\"height:21px; width:21px;\" height=\"21\" width=\"21\">\r\n                                </a>\r\n                            </td>\r\n                            <td style=\"padding: 0px 5px;\" class=\"social-icon-column\">\r\n                                <a role=\"social-icon-link\" href=\"https://twitter.com/mbyjr15\" target=\"_blank\" alt=\"Twitter\" title=\"Twitter\" style=\"display:inline-block; background-color:#7AC4F7; height:21px; width:21px;\">\r\n                                    <img role=\"social-icon\" alt=\"Twitter\" title=\"Twitter\" src=\"https://mc.sendgrid.com/assets/social/white/twitter.png\" style=\"height:21px; width:21px;\" height=\"21\" width=\"21\">\r\n                                </a>\r\n                            </td>\r\n                            <td style=\"padding: 0px 5px;\" class=\"social-icon-column\">\r\n                                <a role=\"social-icon-link\" href=\"https://www.linkedin.com/in/michael-jr-ylanan-98043623a/\" target=\"_blank\" alt=\"LinkedIn\" title=\"LinkedIn\" style=\"display:inline-block; background-color:#0077B5; height:21px; width:21px;\">\r\n                                    <img role=\"social-icon\" alt=\"LinkedIn\" title=\"LinkedIn\" src=\"https://mc.sendgrid.com/assets/social/white/linkedin.png\" style=\"height:21px; width:21px;\" height=\"21\" width=\"21\">\r\n                                </a>\r\n                            </td>\r\n                        </tr>\r\n                    </tbody>                  \r\n                </table>\r\n                </td>\r\n                </tr>");

                    TempData["ResultMessage"] = "A confirmation link has been sent to your email address. Please verify your email to complete registration.";
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
            string aS = getId?.Id.ToString();
            var userzz = new SERecord   
            {
                SERId = getId,
                AppSerId = aS,
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
                Quantity = record.Quantity,
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

            return View(rewards);
        }

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

        [HttpPost]
        public async Task<ActionResult> UploadExcel(IFormFile file)
        {
            var cUser = await _userManager.GetUserAsync(User);

            string returnUrl = Url.Content("~/");

            if (file == null || file.Length <= 0)
            {
                TempData["ResultMessage"] = "No file uploaded.";
                return View("SERecord");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the LicenseContext property

            DataTable userData = ExtractDataFromExcel(file);

            if (userData == null || userData.Rows.Count == 0)
            {
                TempData["ResultMessage"] = "No data found in the uploaded file.";
                return View("SERecord");
            }

            foreach (DataRow row in userData.Rows)
            {
                string email = row["Email"].ToString();
                string password = row["Password"].ToString();

                ApplicationUser user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    user = cUser
                };

                var result = _userManager.CreateAsync(user, password).Result;
                
                var latest = user.Id;
                var getId = _context.ApplicationUsers.FirstOrDefault(u => u.Id == latest);
                string AS = getId?.Id.ToString();

                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //Email
                    await _emailSender.SendEmailAsync(email, "Confirm your email",
                      $"<table class=\"wrapper\" role=\"module\" data-type=\"image\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"table-layout: fixed;\" data-muid=\"84b7c821-5416-4af9-b441-8e85085affb3\">\r\n    <tbody>\r\n        <tr>\r\n            <td style=\"font-size:6px; line-height:10px; padding:0px 0px 0px 0px;\" valign=\"top\" align=\"center\">\r\n                <img class=\"max-width\" border=\"0\" style=\"display:block; color:#000000; text-decoration:none; font-family:Helvetica, arial, sans-serif; font-size:16px; max-width:25% !important; width:25%; height:auto !important;\" width=\"240\" alt=\"\" data-proportionally-constrained=\"true\" data-responsive=\"true\" src=\"http://cdn.mcauto-images-production.sendgrid.net/ba5ff8c16d24e60e/55096f2c-876a-4cc2-8e2e-6832cbb37c14/500x500.png\">\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>" +
                      $"<table data-type=\"text\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"50%\" style=\"table-layout: fixed; margin-right:auto; margin-left:auto\" data-muid=\"d2b3a335-427c-4e78-b490-99e4b86ff853\" data-mc-module-version=\"2019-10-22\">\r\n    <tbody>\r\n        <tr>\r\n            <td style=\"padding:18px 0px 18px 0px; line-height:22px; text-align:inherit;\" height=\"100%\" valign=\"top\" bgcolor=\"\" role=\"module-content\">\r\n                <div>\r\n                    <h2>\r\n                        <span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 24px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial;text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing:0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top; line-height: 1.5\">Greetings and welcome to PReMaSys!</span>\r\n                        <div style=padding:5px></div>\r\n                        <div style=\"font-family: inherit; text-align: inherit; margin-left: 0px\"><span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing: 0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top\">  We’re so happy that you’re here and can’t wait to help you reach your goals when it comes to managing your sales employees, tracking their performance, boosting their morale, and recognizing their efforts. We can’t wait to finally have you in our community, but first, there are a few pre-screening steps you must complete to get started with the web application. You can start by doing this task./span>&nbsp;</div><div></div>\r\n                        <br>\r\n                        <span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 24px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing: 0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top; line-height: 1.5\">Let's confirm your email address.</span>\r\n                    </h2>\r\n                    <div style=\"font-family: inherit; text-align: inherit; margin-left: 0px\"><span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing: 0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top\">By clicking on the following link, you are confirming your email address.</span>&nbsp;</div><div></div>\r\n                </div>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"module\" data-role=\"module-button\" data-type=\"button\" role=\"module\" style=\"table-layout:fixed;\" width=\"100%\" data-muid=\"1c7c9076-cfcb-4233-a2e4-731694c53ed8\">\r\n    <tbody>\r\n        <tr>\r\n            <td align=\"center\" bgcolor=\"\" class=\"outer-td\" style=\"padding:0px 0px 0px 0px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"wrapper-mobile\" style=\"text-align:center;\">\r\n                    <tbody>\r\n                        <tr>\r\n                            <td align=\"center\" bgcolor=\"#55d97e\" class=\"inner-td\" style=\"border-radius:6px; font-size:16px; text-align:center; background-color:inherit;\">\r\n" +
                      $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style=\"background-color:#55d97e; border:1px solid #09d463; border-color:#09d463; border-radius:6px; border-width:1px; color:#ffffff; display:inline-block; font-size:14px; font-weight:normal; letter-spacing:0px; line-height:normal; padding:12px 18px 12px 18px; text-align:center; text-decoration:none; border-style:solid;\" target=\"_blank\">Click Here</a>\r\n                            </td>\r\n                        </tr>\r\n                    </tbody>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n<table class=\"module\" role=\"module\" data-type=\"text\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"table-layout: fixed;\" data-muid=\"952ac095-dfb9-4cd3-85ba-08f4779d7c91\" data-mc-module-version=\"2019-10-22\">\r\n    <tbody>\r\n        <tr>\r\n            <td style=\"padding:18px 0px 18px 0px; line-height:22px; text-align:inherit;\" height=\"100%\" valign=\"top\" bgcolor=\"\" role=\"module-content\"><div><div style=\"font-family: inherit; text-align: center\"><span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 12px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: center; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(253, 253, 253); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; float: none; display: inline\">© PreMaSys Inc. 2544 Taft Ave, Malate, Manila, 1004 Metro Manila</span></div><div></div></div></td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n\r\n<table class=\"module\" role=\"module\" data-type=\"social\" align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"table-layout: fixed;\" data-muid=\"ea5afea2-fe0a-4d3d-9aaa-bc20bdd0375c\">\r\n    <tbody>\r\n        <tr>\r\n            <td valign=\"top\" style=\"padding:0px 0px 0px 0px; font-size:6px; line-height:10px;\" align=\"center\">\r\n                <table align=\"center\" style=\"-webkit-margin-start:auto;-webkit-margin-end:auto;\">\r\n                    <tbody>\r\n                        <tr align=\"center\">\r\n                            <td style=\"padding: 0px 5px;\" class=\"social-icon-column\">\r\n                                <a role=\"social-icon-link\" href=\"https://www.facebook.com/ylananjr.michael/\" target=\"_blank\" alt=\"Facebook\" title=\"Facebook\" style=\"display:inline-block; background-color:#3B579D; height:21px; width:21px;\">\r\n                                    <img role=\"social-icon\" alt=\"Facebook\" title=\"Facebook\" src=\"https://mc.sendgrid.com/assets/social/white/facebook.png\" style=\"height:21px; width:21px;\" height=\"21\" width=\"21\">\r\n                                </a>\r\n                            </td>\r\n                            <td style=\"padding: 0px 5px;\" class=\"social-icon-column\">\r\n                                <a role=\"social-icon-link\" href=\"https://twitter.com/mbyjr15\" target=\"_blank\" alt=\"Twitter\" title=\"Twitter\" style=\"display:inline-block; background-color:#7AC4F7; height:21px; width:21px;\">\r\n                                    <img role=\"social-icon\" alt=\"Twitter\" title=\"Twitter\" src=\"https://mc.sendgrid.com/assets/social/white/twitter.png\" style=\"height:21px; width:21px;\" height=\"21\" width=\"21\">\r\n                                </a>\r\n                            </td>\r\n                            <td style=\"padding: 0px 5px;\" class=\"social-icon-column\">\r\n                                <a role=\"social-icon-link\" href=\"https://www.linkedin.com/in/michael-jr-ylanan-98043623a/\" target=\"_blank\" alt=\"LinkedIn\" title=\"LinkedIn\" style=\"display:inline-block; background-color:#0077B5; height:21px; width:21px;\">\r\n                                    <img role=\"social-icon\" alt=\"LinkedIn\" title=\"LinkedIn\" src=\"https://mc.sendgrid.com/assets/social/white/linkedin.png\" style=\"height:21px; width:21px;\" height=\"21\" width=\"21\">\r\n                                </a>\r\n                            </td>\r\n                        </tr>\r\n                    </tbody>                  \r\n                </table>\r\n                </td>\r\n                </tr>");


                    TempData["ResultMessage"] = "A confirmation link has been sent to your email address. Please verify your email to complete registration.";

                    SERecord seRecord = new SERecord
                    {
                        SERId = getId,
                        AppSerId = AS,
                        EmployeeNo = row["EmployeeNo"].ToString(),
                        EmployeeFirstname = row["EmployeeFirstname"].ToString(),
                        EmployeeLastname = row["EmployeeLastname"].ToString(),
                        EmployeeAddress = row["EmployeeAddress"].ToString(),
                        EmployeeBirthdate = row["EmployeeBirthdate"].ToString()
                    };

                    _context.SERecord.Add(seRecord);
                    _context.SaveChanges();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            TempData["ResultMessage"] = "File uploaded and data imported successfully.";
            return View("SERecord");
        }

        private DataTable ExtractDataFromExcel(IFormFile file)
        {
            DataTable data = new DataTable();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    data = ConvertExcelWorksheetToDataTable(worksheet);
                }
            }

            return data;
        }

        private DataTable ConvertExcelWorksheetToDataTable(ExcelWorksheet worksheet)
        {
            DataTable data2 = new DataTable();

            for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
            {
                if (row == 1)
                {
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        data2.Columns.Add(worksheet.Cells[row, col].Value.ToString());
                    }
                }
                else
                {
                    DataRow dataRow = data2.NewRow();
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Value;
                    }

                    data2.Rows.Add(dataRow);
                }
            }

            return data2;
        }
    }
}

