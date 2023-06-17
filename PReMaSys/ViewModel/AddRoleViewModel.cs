using Microsoft.CodeAnalysis.Differencing;
using SendGrid.Helpers.Mail;
using System;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace PReMaSys.ViewModel
{
    public class AddRoleViewModel
    {
        [Required]
        [Display(Name="Role")]
        public string RoleName { get; set; }
    }
}


