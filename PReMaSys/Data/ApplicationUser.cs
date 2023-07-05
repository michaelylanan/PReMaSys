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

        public byte[]? Pic { get; set; }//check
        public string? CompanyName { get; set; }//check
       
        public string? CompanyAddress { get; set; } // check
        public string? CompanyWebsite { get; set; }//check
        public string? CompanyEmail { get; set; }//check
        public DateTime CompanyBday { get; set; } // check


        public string? CompanyAffiliation { get; set; } // check
        public int? NumberOfEmployees { get; set; } // check
        public string? NatureOfBusiness { get; set; } // check
        public string? BusinessType { get; set; } //Must have an enum (Sole Proprietorship, Parnetrship, Corporation)

        public string? SECNumber { get; set; }
        public string? BusinessPNumber { get; set; }
        public string? BIRTIN { get; set; }


        public DateTime DateAdded { get; set; } = DateTime.Now;
        public virtual ApplicationUser? user { get; set; }

        public bool IsChecked { get; set; }

        public string? Role { get; set; }

        public DateTime? IsArchived { get; set; }

        public string? AddedBy { get; set; }
    }

    public enum BusinessType
    {
        Sole_Proprietorship,
        Parnetrship,
        Corporation,
        Limited_Liability_Company,
        Franchising,
        Cooperative,
        Company,
        Limited_Company,
        Others
    }
}
