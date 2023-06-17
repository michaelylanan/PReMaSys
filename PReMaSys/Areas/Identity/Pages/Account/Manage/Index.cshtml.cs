#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PReMaSys.Data;

namespace PReMaSys.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }


        [BindProperty]
        public InputModel Input { get; set; }


        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            [RegularExpression(@"^(|\+639)\d{9}$", ErrorMessage = "Entered phone format is not valid. Must be 11 digits and numeric (+639 #### ####)")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Company Logo")]
            public byte[] Pic { get; set; }

            //[Required]
            [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
            [Display(Name = "Company Name")]
            public string CompanyName { get; set; }

            //[Required]
            [Display(Name = "Company Address")]
            public string CompanyAddress { get; set; }

            //[Required]
            [Display(Name = "Company Affiliation")]
            public string CompanyAffiliation { get; set; }

            //[Required]
            [Display(Name = "Nature of Business")]
            public string NatureOfBusiness { get; set; }

            //[Required]
            [Display(Name = "Company Foundation Day")]
            [DataType(DataType.Date)]
            public DateTime CompanyBday { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var companyName = user.CompanyName;
            var pic = user.Pic;
            var address = user.CompanyAddress;
            var companyAffiliation = user.CompanyAffiliation;
            var natureOfBusiness = user.NatureOfBusiness;
        /*    var companyBday = user.CompanyBday;*/

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                CompanyName = companyName,
                Pic = pic,
                CompanyAddress = address,
                CompanyAffiliation = companyAffiliation,
                NatureOfBusiness = natureOfBusiness,
                CompanyBday = user.CompanyBday
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            //Company Name
            var companyName = user.CompanyName;
            if(Input.CompanyName != companyName)
            {
                user.CompanyName = Input.CompanyName;
                await _userManager.UpdateAsync(user);
            }

            //Company Logo
            if(Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    user.Pic = dataStream.ToArray();
                }
                await _userManager.UpdateAsync(user);
            }

            //Company Contact Number
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            //Company Address
            var companyAddress = user.CompanyAddress;
            if (Input.CompanyAddress != companyAddress)
            {
                user.CompanyAddress = Input.CompanyAddress;
                await _userManager.UpdateAsync(user);
            }

            //Company Affiliation
            var companyAffiliation = user.CompanyAffiliation;
            if (Input.CompanyAffiliation != companyAffiliation)
            {
                user.CompanyAffiliation = Input.CompanyAffiliation;
                await _userManager.UpdateAsync(user);
            }

            //Company Nature of Business
            var natureOfBusiness = user.NatureOfBusiness;
            if (Input.NatureOfBusiness != natureOfBusiness)
            {
                user.NatureOfBusiness = Input.NatureOfBusiness;
                await _userManager.UpdateAsync(user);
            }

            //Company Foundation Day
            /*var companyBday = user.CompanyBday;*/
            if (Input.CompanyBday != user.CompanyBday)
            {
                user.CompanyBday = Input.CompanyBday;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
