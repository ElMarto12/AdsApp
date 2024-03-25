using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace AdsApp.Controllers;

public class LoginController : Controller
{
    public async Task Login()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
    }
    
    public async Task<IActionResult> GoogleResponse()
    {
        await HttpContext.AuthenticateAsync((CookieAuthenticationDefaults.AuthenticationScheme));

        return RedirectToAction("Index", "Home", new { area = " " });
    }

    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}