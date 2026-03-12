using Microsoft.AspNetCore.Mvc;
using NetCoreSeguridadEmpleados.Filters;
using NetCoreSeguridadEmpleados.Models;
using NetCoreSeguridadEmpleados.Repositories;
using System.Security.Claims;

namespace NetCoreSeguridadEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryHospital repo;
        public EmpleadosController(RepositoryHospital repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Details(int id)
        {
            Empleado empleado = await this.repo.FindEmpleadoAsync(id);
            return View(empleado);
        }

        [AuthorizeEmpleados(Policy = "SUBORDINADOS")]
        public async Task<IActionResult> Delete(int id)
        {
            await this.repo.DeleteEmpleadoAsync(id);
            return RedirectToAction("Index", "Empleados");
        }

        [AuthorizeEmpleados]
        public IActionResult PerfilEmpleado()
        {
            return View();
        }

        [AuthorizeEmpleados(Policy = "SOLOJEFES")]
        public async Task<IActionResult> Compis()
        {
            //RECUPERAMOS EL CLAIM DEL USUARIO VALIDADO
            string dato = HttpContext.User.FindFirstValue("Departamento");
            int iddepartamento = int.Parse(dato);
            List<Empleado> empleados = await this.repo.GetEmpleadosDepartamentoAsync(iddepartamento);
            return View(empleados);
        }
        [AuthorizeEmpleados]
        [HttpPost]
        public async Task<IActionResult> Compis(int incremento)
        {
            string dato = HttpContext.User.FindFirstValue("Departamento");
            int iddepartamento = int.Parse(dato);
            await this.repo.UpdateSalarioEmpleadosAsync(iddepartamento, incremento);
            List<Empleado> empleados = await this.repo.GetEmpleadosDepartamentoAsync(iddepartamento);
            return View(empleados);
        }
        [AuthorizeEmpleados(Policy = "AdminOnly")]
        public IActionResult AdminEmpleados()
        {
            return View();
        }

        [AuthorizeEmpleados(Policy = "SoloRicos")]
        public IActionResult ZonaNoble()
        {
            return View();
        }
    }
}
