using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using PReMaSys.Data;
using PReMaSys.Models;
using PReMaSys.ViewModel;
using System.Data;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe.Infrastructure;
using Stripe.Checkout;
using Stripe;

namespace PReMaSys.Controllers
{
   //[Authorize(Roles = "Domain")]
    public class DomainController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        

        public DomainController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;


        }
        public IActionResult DomainPage()
        {
            return View();
        }

  
        public IActionResult Payment(string amount)
        {
            StripeConfiguration.SetApiKey("sk_test_51NQXLfLnmEYUKKTC7j8BPrLJEoD0qlsRvojPu2n0sJJ45PKwWfVYykTQkiyrNt3QSyTHePD2VXHgY9Cl7oJPUok400Su8fYcCZ");
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
        {
          new SessionLineItemOptions
          {
            PriceData = new SessionLineItemPriceDataOptions
            {
              UnitAmount =Convert.ToInt32(5000)*100,
              Currency = "php",
              ProductData = new SessionLineItemPriceDataProductDataOptions
              {
                Name = "1-Month Premasys Subscription",
              },

            },
            Quantity = 1,
          },
        },
                Mode = "payment",
                SuccessUrl = "https://localhost:7126/Domain/PaymentSuccess",
                CancelUrl = "https://localhost:7126/Domain/PaymentCancel",
            };

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentSuccess()
        {
            //@TODO 
            string au = _userManager.GetUserId(HttpContext.User);

            var users = _context.ApplicationUsers.Where(r => r.Id == au).SingleOrDefault();

            users.DatePaid = DateTime.Now;
            var tempDate = DateTime.Now;
            tempDate = tempDate.AddMonths(1);
            users.DateExpiration = tempDate;
            users.IsActive = true;

            _context.ApplicationUsers.Update(users);
            _context.SaveChanges();

            return View();
        }

        public IActionResult PaymentCancel()
        {
            return View();
        }

        public IActionResult Subscription()
        {
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;
            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");

            var list = _context.Rewards.Where(r => r.ApplicationUser == checker && r.Status != Status.Approved).ToList();
            int totalRewards = list.Count;

            ViewBag.TotalRewards = totalRewards;

            return View();
        }

        //Reports
        public IActionResult ReportsPage() 
        {
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;
            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");

            var list = _context.Rewards.Where(r => r.ApplicationUser == checker && r.Status != Status.Approved).ToList();
            int totalRewards = list.Count;

            ViewBag.TotalRewards = totalRewards;
            return View();
        }       

        public IActionResult Ranking() 
        {
            
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;
            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Admin");

            var sPerformance = _context.SalesPerformances.Where(s => s.LoggedUser == checker.Id).ToList();

            var combP = sPerformance.GroupBy(s => s.SalesPerson).Select(g => new SalesPerformance
                {
                    SalesPerson = g.Key,
                    UnitsSold = g.Sum(s => s.UnitsSold),
                    SalesRevenue = g.Sum(s => s.SalesRevenue),
                    SalesProfit = g.Sum(s => s.SalesProfit)
                }).ToList();

            var topThree = combP
                .OrderByDescending(s => s.UnitsSold)
                .Take(3)
                .ToList();

            var remaining = combP
                .Except(topThree)
                .ToList();

            var model = new SalesPerformanceRankingViewModel
            {
                TopThreeP = topThree,
                RemainingP = remaining
            };

            var checker2 = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");


            var listR = _context.Rewards.Where(r => r.ApplicationUser == checker2 && r.Status != Status.Approved).ToList();
            int totalRewards = listR.Count;

            ViewBag.TotalRewards = totalRewards;

            return View(model);
        }

        public IActionResult Forecasts() 
        {           
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;

            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Admin");
            var checker2 = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");


            var list = _context.SalesPerformances.Where(a => a.LoggedUser == checker.Id).ToList();
            ViewBag.User = list;

            var listR = _context.Rewards.Where(r => r.ApplicationUser == checker2 && r.Status != Status.Approved).ToList();
            int totalRewards = listR.Count;

            ViewBag.TotalRewards = totalRewards;

            return View();
        }

        public IActionResult Diagnostic() 
        {
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;

            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Admin");

            var list = _context.SalesPerformances.Where(a => a.LoggedUser == checker.Id).ToList();
            ViewBag.User = list;


            var checker2 = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");
            var listR = _context.Rewards.Where(r => r.ApplicationUser == checker2 && r.Status != Status.Approved).ToList();
            int totalRewards = listR.Count;

            ViewBag.TotalRewards = totalRewards;

            return View();
        }

        public IActionResult Descriptive() 
        {
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;

            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Admin");

            var list = _context.SalesPerformances.Where(a => a.LoggedUser == checker.Id).ToList();

            ViewBag.User = list;

            var checker2 = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");


            var listR = _context.Rewards.Where(r => r.ApplicationUser == checker2 && r.Status != Status.Approved).ToList();
            int totalRewards = listR.Count;

            ViewBag.TotalRewards = totalRewards;

            return View();
        }

        /*APPROVAL OF REWARDS-------------------------------------------------------------------------------------------------------------------------------------*/
        public IActionResult ApproveRewards(string id)
        {
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;
            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");

            var list = _context.Rewards.Where(r => r.ApplicationUser == checker && r.Status != Status.Approved ).ToList();
            
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
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;
            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");

            var list = _context.Rewards.Where(r => r.ApplicationUser == checker && r.Status != Status.Approved).ToList();
            int totalRewards = list.Count;

            ViewBag.TotalRewards = totalRewards;

            var roles = _roleManager.Roles;

            return View(roles);
        }

        /*ADMIN ROLES CRUD---------------------------------------------------------------------------------------------------------------------------------*/

        [HttpGet]
        public IActionResult ListAdminRoles()
        {
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;
            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");

            var listR = _context.Rewards.Where(r => r.ApplicationUser == checker && r.Status != Status.Approved).ToList();
            int totalRewards = listR.Count;

            ViewBag.TotalRewards = totalRewards;

            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            var list = _context.Users.Where(c => c.user == user && c.IsArchived == null).ToList();
            return View(list);
        }



        /*CREATE NEW ADMIN ROLE*/
        public IActionResult AddRole()
        {
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;
            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");

            var list = _context.Rewards.Where(r => r.ApplicationUser == checker && r.Status != Status.Approved).ToList();
            int totalRewards = list.Count;

            ViewBag.TotalRewards = totalRewards;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddRole(AdminUser admin)
        {
            string returnUrl = Url.Content("~/");
            var current = await _userManager.GetUserAsync(User);
            string au = _userManager.GetUserId(HttpContext.User);

            var user = new ApplicationUser
            {
                UserName = admin.Email,
                Email = admin.Email,
                user = current,
                AddedBy = au
                
            };

            if(admin.Password == admin.ConfirmPassword)
            {
                var insertrec = await _userManager.CreateAsync(user, admin.Password);
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
                    await _emailSender.SendEmailAsync(admin.Email, "Confirm your email",
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
                ViewBag.message2 = "Password Mismatch";
            }
           

            return View();
        }

        /*EDIT ADMIN ROLES Account------------------------------------------------*/
        [HttpGet]
        public async Task<IActionResult> EditAdminRole(string id)
        {
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;
            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");

            var list = _context.Rewards.Where(r => r.ApplicationUser == checker).ToList();
            int totalRewards = list.Count;

            ViewBag.TotalRewards = totalRewards;

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
                var result = await _userManager.UpdateAsync(admin);

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
            ApplicationUser au = _userManager.GetUserAsync(HttpContext.User).Result;

            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.user == au && a.Role == "Support");

            var list = _context.SERecord.Where(s => s.SupportId == checker).ToList();

            var listR = _context.Rewards.Where(r => r.ApplicationUser == checker && r.Status != Status.Approved).ToList();
            int totalRewards = listR.Count;

            ViewBag.TotalRewards = totalRewards;
            return View(list);
        }

    }
}
