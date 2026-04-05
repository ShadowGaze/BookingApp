using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingApp.Data;
using BookingApp.Models;

namespace BookingApp.Controllers
{
    public class RegistersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View("~/Views/Registers/Register.cshtml");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                
               
                var existingEmail = await _context.Register
                    .FirstOrDefaultAsync(r => r.EmailId == model.EmailId);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("EmailId", "This email is already registered.");
                    return View(model);
                }

                
                var existingPhone = await _context.Register
                    .FirstOrDefaultAsync(r => r.Number == model.Number);
                if (existingPhone != null)
                {
                    ModelState.AddModelError("Number", "This phone number is already registered.");
                    return View(model);
                }

                
                var register = new Register
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CompanyName = model.CompanyName,
                    Number = model.Number,
                    EmailId = model.EmailId,
                    Address = model.Address,
                    Username = model.Username,
                    Password = model.Password
                };

                _context.Register.Add(register);
                await _context.SaveChangesAsync();

                
                var login = new Login
                {
                    Username = model.Username,
                    Password = model.Password
                };

                _context.Login.Add(login);
                await _context.SaveChangesAsync();

                
                ViewBag.SuccessMessage = "Registration Successful! You can now login.";

                return View("Register", model);
            }

            return View(model);
        }
    }

}

