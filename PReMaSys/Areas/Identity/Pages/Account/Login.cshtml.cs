#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PReMaSys.Controllers;
using PReMaSys.Data;
using System.ComponentModel.DataAnnotations;

namespace PReMaSys.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        //AuditLogs
        private readonly AuditLogController _auditLogController;

        public LoginModel(
         SignInManager<ApplicationUser> signInManager,
         UserManager<ApplicationUser> userManager,
         ILogger<LoginModel> logger,
         AuditLogController auditLogController)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _auditLogController = auditLogController;
        }

        [BindProperty]
        public InputModel Input { get; set; }


        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }


        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {

                    //AuditLogs
                    _auditLogController.LogLoginEvent(Input.Email, "Successful");


                    _logger.LogInformation("User logged in.");

                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Domain"))
                    {
                        return Redirect("~/Domain/ReportsPage");
                    }
                    else if (roles.Contains("Support"))
                    {
                        return Redirect("~/SupportAdmin/SupportPage");
                    }
                    else if (roles.Contains("Sales"))
                    {
                        return Redirect("~/Employee/EmployeeHomePage");
                    }
                    else if (roles.Contains("Admin"))
                    {
                        return Redirect("~/Admin/ReportsPage");
                    }
                    else if (roles.Contains("Super"))
                    {
                        return Redirect("~/Manage/ListAllRoles");
                    }
                    else
                    {
                        return Redirect("~/Home/FirstLanding");
                    }
                    /* else if (roles.Contains("Admin"))
                     {
                         return Redirect("~/Admin/AdminDashboard");
                     }
                     else if (roles.Contains("Sales"))
                     {
                         return Redirect("~/Employee/EmployeeHomePage");
                     }*/
                    // Log failed login event
                    
                    return LocalRedirect(returnUrl);//redirect to this page returnUrl = ("~/")


                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    _auditLogController.LogLoginEvent(Input.Email, "Account Locked");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    _auditLogController.LogLoginEvent(Input.Email, "Failed");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
