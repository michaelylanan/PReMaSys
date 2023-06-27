#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using PReMaSys.Data;

namespace PReMaSys.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Company Email")]
            public string CompanyEmail { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
            [Display(Name = "Company Name")]
            public string CompanyName { get; set; }

            [Display(Name = "Company Logo")]
            public byte[] Pic { get; set; }

            [Required]
            [Display(Name = "Company Address")]
            public string CompanyAddress { get; set; }

            [Required]
            [Display(Name = "Company Website")]
            public string CompanyWebsite { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            [Display(Name = "Company Address")]
            public DateTime CompanyBday { get; set; }

            [Required]
            [Display(Name = "Company Affiliation")]
            public string CompanyAffiliation { get; set; }

            [Required]
            [Display(Name = "Number Of Employees")]
            [Range(1, 1000, ErrorMessage = "The {0} must be between {1} and {2}.")]
            public int NumberOfEmployees { get; set; }

            [Required]
            [Display(Name = "Nature Of Business")]
            public string NatureOfBusiness { get; set; }

            [Required]
            [Display(Name = "Business Type")]
            public string BusinessType { get; set; }

            [Required]
            [Display(Name = "Security Exchange Commission Number")]
            public string SECNumber { get; set; }

            [Required]
            [Display(Name = "Business Permit Number")]
            public string BusinessPNumber { get; set; }

            [Required]
            [Display(Name = "B.I.R. TIN Registration Number")]
            public string BIRTIN { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.CompanyName = Input.CompanyName;

                //Company Logo
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);
                        user.Pic = dataStream.ToArray();
                    }
                }

                user.CompanyAddress = Input.CompanyAddress;
                user.CompanyWebsite = Input.CompanyWebsite;
                user.CompanyEmail = Input.CompanyEmail;
                user.CompanyBday = Input.CompanyBday;
                user.CompanyAffiliation = Input.CompanyAffiliation;
                user.NumberOfEmployees = Input.NumberOfEmployees;
                user.NatureOfBusiness = Input.NatureOfBusiness;
                user.BusinessType = Input.BusinessType;
                user.SECNumber = Input.SECNumber;
                user.BusinessPNumber = Input.BusinessPNumber;
                user.BIRTIN = Input.BIRTIN;

                user.DateAdded = DateTime.Now;


                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //Email
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                      $"<table class=\"wrapper\" role=\"module\" data-type=\"image\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"table-layout: fixed;\" data-muid=\"84b7c821-5416-4af9-b441-8e85085affb3\">\r\n    <tbody>\r\n        <tr>\r\n            <td style=\"font-size:6px; line-height:10px; padding:0px 0px 0px 0px;\" valign=\"top\" align=\"center\">\r\n                <img class=\"max-width\" border=\"0\" style=\"display:block; color:#000000; text-decoration:none; font-family:Helvetica, arial, sans-serif; font-size:16px; max-width:25% !important; width:25%; height:auto !important;\" width=\"240\" alt=\"\" data-proportionally-constrained=\"true\" data-responsive=\"true\" src=\"http://cdn.mcauto-images-production.sendgrid.net/ba5ff8c16d24e60e/55096f2c-876a-4cc2-8e2e-6832cbb37c14/500x500.png\">\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>" +
                      $"<table data-type=\"text\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"50%\" style=\"table-layout: fixed; margin-right:auto; margin-left:auto\" data-muid=\"d2b3a335-427c-4e78-b490-99e4b86ff853\" data-mc-module-version=\"2019-10-22\">\r\n    <tbody>\r\n        <tr>\r\n            <td style=\"padding:18px 0px 18px 0px; line-height:22px; text-align:inherit;\" height=\"100%\" valign=\"top\" bgcolor=\"\" role=\"module-content\">\r\n                <div>\r\n                    <h2>\r\n                        <span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 24px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial;text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing:0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top; line-height: 1.5\">Greetings and welcome to PReMaSys!</span>\r\n                        <div style=padding:5px></div>\r\n                        <div style=\"font-family: inherit; text-align: inherit; margin-left: 0px\"><span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing: 0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top\">  We’re so happy that you’re here and can’t wait to help you reach your goals when it comes to managing your sales employees, tracking their performance, boosting their morale, and recognizing their efforts. We can’t wait to finally have you in our community, but first, there are a few pre-screening steps you must complete to get started with the web application. You can start by doing this task./span>&nbsp;</div><div></div>\r\n                        <br>\r\n                        <span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 24px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing: 0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top; line-height: 1.5\">Let's confirm your email address.</span>\r\n                    </h2>\r\n                    <div style=\"font-family: inherit; text-align: inherit; margin-left: 0px\"><span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: start; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; box-sizing: border-box; width: 518px; -webkit-border-horizontal-spacing: 0px; -webkit-border-vertical-spacing: 0px; border-collapse: separate; margin-top: 0px; margin-right: 0px; margin-bottom: 30px; margin-left: 0px; padding-top: 0px; padding-right: 0px; padding-bottom: 0px; padding-left: 0px; vertical-align: top\">By clicking on the following link, you are confirming your email address.</span>&nbsp;</div><div></div>\r\n                </div>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"module\" data-role=\"module-button\" data-type=\"button\" role=\"module\" style=\"table-layout:fixed;\" width=\"100%\" data-muid=\"1c7c9076-cfcb-4233-a2e4-731694c53ed8\">\r\n    <tbody>\r\n        <tr>\r\n            <td align=\"center\" bgcolor=\"\" class=\"outer-td\" style=\"padding:0px 0px 0px 0px;\">\r\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"wrapper-mobile\" style=\"text-align:center;\">\r\n                    <tbody>\r\n                        <tr>\r\n                            <td align=\"center\" bgcolor=\"#55d97e\" class=\"inner-td\" style=\"border-radius:6px; font-size:16px; text-align:center; background-color:inherit;\">\r\n" +
                      $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style=\"background-color:#55d97e; border:1px solid #09d463; border-color:#09d463; border-radius:6px; border-width:1px; color:#ffffff; display:inline-block; font-size:14px; font-weight:normal; letter-spacing:0px; line-height:normal; padding:12px 18px 12px 18px; text-align:center; text-decoration:none; border-style:solid;\" target=\"_blank\">Click Here</a>\r\n                            </td>\r\n                        </tr>\r\n                    </tbody>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n<table class=\"module\" role=\"module\" data-type=\"text\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"table-layout: fixed;\" data-muid=\"952ac095-dfb9-4cd3-85ba-08f4779d7c91\" data-mc-module-version=\"2019-10-22\">\r\n    <tbody>\r\n        <tr>\r\n            <td style=\"padding:18px 0px 18px 0px; line-height:22px; text-align:inherit;\" height=\"100%\" valign=\"top\" bgcolor=\"\" role=\"module-content\"><div><div style=\"font-family: inherit; text-align: center\"><span style=\"color: #294661; font-family: &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, Helvetica, Helvetica, Arial, sans-serif; font-size: 12px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 300; letter-spacing: normal; orphans: 2; text-align: center; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(253, 253, 253); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; float: none; display: inline\">© PreMaSys Inc. 2544 Taft Ave, Malate, Manila, 1004 Metro Manila</span></div><div></div></div></td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n\r\n<table class=\"module\" role=\"module\" data-type=\"social\" align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"table-layout: fixed;\" data-muid=\"ea5afea2-fe0a-4d3d-9aaa-bc20bdd0375c\">\r\n    <tbody>\r\n        <tr>\r\n            <td valign=\"top\" style=\"padding:0px 0px 0px 0px; font-size:6px; line-height:10px;\" align=\"center\">\r\n                <table align=\"center\" style=\"-webkit-margin-start:auto;-webkit-margin-end:auto;\">\r\n                    <tbody>\r\n                        <tr align=\"center\">\r\n                            <td style=\"padding: 0px 5px;\" class=\"social-icon-column\">\r\n                                <a role=\"social-icon-link\" href=\"https://www.facebook.com/ylananjr.michael/\" target=\"_blank\" alt=\"Facebook\" title=\"Facebook\" style=\"display:inline-block; background-color:#3B579D; height:21px; width:21px;\">\r\n                                    <img role=\"social-icon\" alt=\"Facebook\" title=\"Facebook\" src=\"https://mc.sendgrid.com/assets/social/white/facebook.png\" style=\"height:21px; width:21px;\" height=\"21\" width=\"21\">\r\n                                </a>\r\n                            </td>\r\n                            <td style=\"padding: 0px 5px;\" class=\"social-icon-column\">\r\n                                <a role=\"social-icon-link\" href=\"https://twitter.com/mbyjr15\" target=\"_blank\" alt=\"Twitter\" title=\"Twitter\" style=\"display:inline-block; background-color:#7AC4F7; height:21px; width:21px;\">\r\n                                    <img role=\"social-icon\" alt=\"Twitter\" title=\"Twitter\" src=\"https://mc.sendgrid.com/assets/social/white/twitter.png\" style=\"height:21px; width:21px;\" height=\"21\" width=\"21\">\r\n                                </a>\r\n                            </td>\r\n                            <td style=\"padding: 0px 5px;\" class=\"social-icon-column\">\r\n                                <a role=\"social-icon-link\" href=\"https://www.linkedin.com/in/michael-jr-ylanan-98043623a/\" target=\"_blank\" alt=\"LinkedIn\" title=\"LinkedIn\" style=\"display:inline-block; background-color:#0077B5; height:21px; width:21px;\">\r\n                                    <img role=\"social-icon\" alt=\"LinkedIn\" title=\"LinkedIn\" src=\"https://mc.sendgrid.com/assets/social/white/linkedin.png\" style=\"height:21px; width:21px;\" height=\"21\" width=\"21\">\r\n                                </a>\r\n                            </td>\r\n                        </tr>\r\n                    </tbody>                  \r\n                </table>\r\n                </td>\r\n                </tr>");
                     /* $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");*/

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    { 
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
