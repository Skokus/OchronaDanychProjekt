using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OchronaDanych.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Nick { get; set; }
        public string MasterPassword { get; set; }
        public List<DomainPassword> Passwords { get; set; }
        public bool IsLocked { get; set; }
    }
}
