using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NetCoreSeguridadEmpleados.Models;
using NetCoreSeguridadEmpleados.Repositories;
using System.Security.Claims;

namespace NetCoreSeguridadEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryHospital repo;
        public ManagedController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            int idempleado = int.Parse(password);
            Empleado empleado = await this.repo.LogInEmpleadoAsync(username, idempleado);
            if(empleado != null)
            {
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme,
                                                                ClaimTypes.Name, ClaimTypes.Role);
                //EMPLEADO ARROYO: 7499 SERA NUESTRO ADMINISTRADOR
                if(empleado.IdEmpleado == 7499)
                {
                    Claim claimAdmin =
                        new Claim("Admin", "Soy el amo de la empresa");
                    identity.AddClaim(claimAdmin);
                }
                Claim claimName =
                    new Claim(ClaimTypes.Name, username);
                identity.AddClaim(claimName);
                Claim claimid =
                    new Claim(ClaimTypes.NameIdentifier, empleado.IdEmpleado.ToString());
                identity.AddClaim(claimid);
                Claim claimrole =
                    new Claim(ClaimTypes.Role, empleado.Oficio);
                identity.AddClaim(claimrole);
                Claim claimsalario =
                    new Claim("Salario", empleado.Salario.ToString());
                identity.AddClaim(claimsalario);
                Claim claimdepartamento =
                    new Claim("Departamento", empleado.IdDepartamento.ToString());
                identity.AddClaim(claimdepartamento);
                //COMO POR AHORA NO VOY A UTILIZAR ROLES, NO LO INDICAMOS
                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                string controller = TempData["controller"].ToString();
                string action = TempData["action"].ToString();
                if (TempData["id"] != null)
                {
                    string id = TempData["id"].ToString();
                    return RedirectToAction(action, controller, new { id = id });
                } else
                {
                    //POR AHORA LO ENVIAMOS A UNA VISTA QUE HAREMOS EN BREVE
                    return RedirectToAction(action, controller);
                }
            } else
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
