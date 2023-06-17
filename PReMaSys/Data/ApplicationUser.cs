using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PReMaSys.Models;

namespace PReMaSys.Data
{
    public class ApplicationUser: IdentityUser
    {
        public string? CompanyName { get; set; }
        public byte[]? Pic { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyAffiliation { get; set; }
        public string? NatureOfBusiness { get; set; }
        public DateTime CompanyBday { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public virtual ApplicationUser? user { get; set; }
    }
}
