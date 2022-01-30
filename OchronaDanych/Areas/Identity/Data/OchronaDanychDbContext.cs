using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OchronaDanych.Areas.Identity.Data;

namespace OchronaDanych.Data
{
    public class OchronaDanychDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<DomainPassword> DomainPasswords { get; set; }

        public OchronaDanychDbContext()
        {
        }

        public OchronaDanychDbContext(DbContextOptions<OchronaDanychDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
