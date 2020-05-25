using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HarelTech.WMS.Common.Models;
using HarelTech.WMS.RestClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HarelTech.WMS.App.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IWmsClient _wmsClient;
        //private readonly IMemoryCache _cache;

        [BindProperty]
        public UserLoginModel UserLogin { get; set; }
        public LoginModel(IWmsClient wmsClient)
        {
            _wmsClient = wmsClient;
            //_cache = cache;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _wmsClient.GetSystemUserAsync(UserLogin);

            if (user.Id == 0)
                return RedirectToPage("ErrorLogin", new { error = user.Response.Error });


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, UserLogin.UserName),
                new Claim(ClaimTypes.Name, user.USERNAME),
                //new Claim(ClaimTypes.Email, "test@gmail.com"),
                new Claim(ClaimTypes.UserData, user.Id.ToString()),
                new Claim(ClaimTypes.Sid , UserLogin.Password),
                new Claim(ClaimTypes.Role, "User"),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                IssuedUtc = DateTime.Now,
                // The time at which the authentication ticket was issued.

                RedirectUri = "/Account/Login"
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            //make login to license service, if ok, go on
            return RedirectToPage("Company", new { companies = UserLogin.Companies });
            //return new JsonResult(new { success = true, message = "" });
        }
    }
}