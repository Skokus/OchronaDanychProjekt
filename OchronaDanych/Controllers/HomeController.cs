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
using System.Security.Cryptography;
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
            DomainPasswordVM password = new DomainPasswordVM();
            return View(password);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DomainPasswordVM s, string MasterPassword)
        {

            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                if (MasterPassword != null && _userManager.PasswordHasher.VerifyHashedPassword(user, user.MasterPassword, MasterPassword).ToString() == "Success")
                {
                    byte[] key = Encryption.getKeyFromString(MasterPassword, user.salt);
                    byte[] iv = new byte[16];
                    using (var random = new RNGCryptoServiceProvider())
                    {
                        random.GetNonZeroBytes(iv);
                    }
                    ochronaDanychDb.DomainPasswords.Add(new DomainPassword()
                    {
                        User = user,
                        Domain = s.Domain,
                        IV = iv,
                        Password = Encryption.EncryptStringToBytes_Aes(s.Password, key, iv)
                    });
                    ochronaDanychDb.SaveChanges();
                    return RedirectToAction("Index");
                } else
                {
                    return View(s);
                }
            }
            else
            {
                return View(s);
            }
        }

        public async Task<IActionResult> Open(int id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            DomainPassword domainPassword = ochronaDanychDb.DomainPasswords.FirstOrDefault(x => x.ID == id);
            try
            {
                if (user.Id == domainPassword.User.Id)
                {
                    DomainPasswordVM password = new DomainPasswordVM()
                    {
                        ID = id,
                        Password = "********",
                        Domain = domainPassword.Domain
                    };
                    return View(password);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            } catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Open(DomainPasswordVM s, string MasterPassword)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            DomainPassword domainPassword = ochronaDanychDb.DomainPasswords.FirstOrDefault(x => x.ID == s.ID);            
            if (ModelState.IsValid)
            {

                if (MasterPassword != null && _userManager.PasswordHasher.VerifyHashedPassword(user, user.MasterPassword, MasterPassword).ToString() == "Success")
                {
                    byte[] key = Encryption.getKeyFromString(MasterPassword, user.salt);
                    DomainPasswordVM domainPasswordVM = new DomainPasswordVM() {
                        ID = domainPassword.ID,
                        Domain = domainPassword.Domain,
                        Password = Encryption.DecryptStringFromBytes_Aes(domainPassword.Password, key, domainPassword.IV)
                    };
                    return View(domainPasswordVM);
                }
                else
                {
                    DomainPasswordVM password = new DomainPasswordVM()
                    {
                        ID = domainPassword.ID,
                        Password = "********",
                        Domain = domainPassword.Domain
                    };
                    return View(password);
                }
            }
            else
            {
                return View(s);
            }
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
