﻿using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TinyPlants.Models;
using TinyPlants.Models.Interfaces;

namespace TinyPlants.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IShopManager _shopManager;
        private IEmailSender _emailSender;

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        /// <summary>
        /// A property to be available on the Model property in the Razor Page
        /// It uses BindProperty attribute to access the values outside of the handler method
        /// </summary>
        [BindProperty]
        public RegisterInput Input { get; set; }

        /// <summary>
        /// A property that brings in SignInManager depdency to be used in the class
        /// </summary>
        /// <param name="userManager">UserManager context</param>
        /// <param name="signInManager">SignInManager context</param>
        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IShopManager shopManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            Configuration = configuration;
            _shopManager = shopManager;
            _emailSender = emailSender;
            WebHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// A handler method to process the default GET request
        /// </summary>
        public void OnGet()
        {

        }

        /// <summary>
        /// A handler method to process a POST request after a user's registration information has been entered
        /// </summary>
        /// <returns>Home page upon successful registration</returns>
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Address = Input.Address,
                    Address2 = Input.Address2,
                    City = Input.City,
                    State = Input.State,
                    Zip = Input.Zip
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    Claim name = new Claim("FullName", $"{Input.FirstName} {Input.LastName}");
                    Claim email = new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.Email);

                    List<Claim> claims = new List<Claim> { name, email };
                    await _userManager.AddClaimsAsync(user, claims);

                    string adminEmail = WebHostEnvironment.IsDevelopment()
                        ? Configuration["ADMIN_EMAIL"]
                        : Environment.GetEnvironmentVariable("ADMIN_EMAIL");

                    if (Input.Email == adminEmail)
                    {
                        await _userManager.AddToRoleAsync(user, ApplicationRoles.Admin);
                    }

                    await _userManager.AddToRoleAsync(user, ApplicationRoles.Member);

                    var cart = new Cart
                    {
                        UserId = user.Id
                    };

                    await _shopManager.CreateCartAsync(cart);

                    string subject = "Welcome to Tiny Plants!";
                    string message = 
                        $"<p>Hello {user.FirstName} {user.LastName},</p>" +
                        $"<p>&nbsp;</p>" +
                        $"<p>Welcome to Tiny Plants! You have successfully created a new account.</p>" +
                        $"<p>At Tiny Plants, we provide numerous unique and beautiful tiny plants for you to choose from!\n</p>" + "<a href=\"https://dotnet-ecommerce-tiny-plants.azurewebsites.net\">Start shoping now!<a>";

                    await _emailSender.SendEmailAsync(user.Email, subject, message);

                    await _signInManager.SignInAsync(user, false);

                    Response.Redirect("/");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }

        /// <summary>
        /// A class to define the RegisterInput
        /// </summary>
        public class RegisterInput
        {
            [Required]
            [Display(Name = "First Name:")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name:")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email Address:")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long", MinimumLength = 6)]
            [Display(Name = "Password:")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
            [Display(Name = "Confirm Password:")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Address { get; set; }

            [Display(Name = "Address 2:")]
            public string Address2 { get; set; }

            [Required]
            public string City { get; set; }

            [Required]
            public string State { get; set; }

            [Required]
            [DataType(DataType.PostalCode)]
            [Compare("Zip", ErrorMessage = "The is an invalid zip code")]
            public string Zip { get; set; }
        }
    }
}
