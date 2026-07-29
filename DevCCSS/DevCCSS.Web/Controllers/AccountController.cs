using DevCCSS.Web.Models;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevCCSS.Web.Controllers
{
    // El login NO toca SQL: le pregunta al WCF.
    public class AccountController : Controller
    {
        private readonly SeguridadClient _seguridad;

        public AccountController(SeguridadClient seguridad)
        {
            _seguridad = seguridad;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _seguridad.ValidarLoginAsync(model.Username, model.Password);

            if (response is null || !response.Success)
            {
                ModelState.AddModelError("", "Usuario o contrasenia incorrectos.");
                return View(model);
            }

            // Armar los claims con lo que devolvio el WCF
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, response.Username)
            };

            foreach (var role in response.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
