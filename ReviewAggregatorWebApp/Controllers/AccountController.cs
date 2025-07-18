﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReviewAggregatorWebApp.Model;
using ReviewAggregatorWebApp.ViewModel;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email, Login = model.Login };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    await AddClaimsAsync(user);
                    return Redirect(model.ReturnUrl ?? Url.Action("Index", "Genres"));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {            
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    await AddClaimsAsync(user);
                    return Redirect(model.ReturnUrl ?? Url.Action("Index", "Genres"));
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl ?? Url.Action("Index", "Genres"));
        }

        private async Task AddClaimsAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Login", user.Login),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Login");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
        }
    }
}