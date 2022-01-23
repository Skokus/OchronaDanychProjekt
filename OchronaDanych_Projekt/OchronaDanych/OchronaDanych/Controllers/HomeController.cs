using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OchronaDanych.Areas.Identity.Data;
using OchronaDanych.Data;
using OchronaDanych.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OchronaDanych.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OchronaDanychDbContext ochronaDanychDb;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, OchronaDanychDbContext ochronaDanychDb, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this.ochronaDanychDb = ochronaDanychDb;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            List<DomainPassword> passwords = ochronaDanychDb.DomainPasswords.Where(d => d.User.Id == user.Id).ToList();
            return View(passwords);
        }

        public async Task<IActionResult> Create()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            DomainPassword password = new DomainPassword();
            return View(password);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DomainPassword s)
        {

            if (ModelState.IsValid)
            {
                s.Password = Encryption.Base64Encode(s.Password);
                s.User = await _userManager.GetUserAsync(User);
                ochronaDanychDb.DomainPasswords.Add(s);
                ochronaDanychDb.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(s);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckMaster(string MasterPassword)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (MasterPassword != null && _userManager.PasswordHasher.VerifyHashedPassword(user, user.MasterPassword, MasterPassword).ToString() == "Success")
            {
                user.IsLocked = false;
                ochronaDanychDb.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Lock()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            user.IsLocked = true;
            ochronaDanychDb.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
