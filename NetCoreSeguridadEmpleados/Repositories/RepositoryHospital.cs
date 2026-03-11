using Microsoft.EntityFrameworkCore;
using NetCoreSeguridadEmpleados.Data;
using NetCoreSeguridadEmpleados.Models;

namespace NetCoreSeguridadEmpleados.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;
        public RepositoryHospital(HospitalContext context) {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            var consulta = from datos in this.context.Empleados
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<Empleado> FindEmpleadoAsync(int idempleado)
        {
            return await this.context.Empleados.FirstOrDefaultAsync(x => x.IdEmpleado == idempleado);
        }

        public async Task<List<Empleado>> GetEmpleadosDepartamentoAsync(int iddepartamento)
        {
            return await this.context.Empleados.Where(x => x.IdDepartamento == iddepartamento).ToListAsync();
        }

        public async Task UpdateSalarioEmpleadosAsync(int iddepartamento, int incremento)
        {
            List<Empleado> empleados = await GetEmpleadosDepartamentoAsync(iddepartamento);
            foreach(Empleado emp in empleados)
            {
                emp.Salario += incremento;
            }
            await this.context.SaveChangesAsync();
        }

        public async Task<Empleado> LogInEmpleadoAsync
            (string apellido, int idempleado)
        {
            Empleado empleado = await
                this.context.Empleados.FirstOrDefaultAsync(z => z.Apellido == apellido && z.IdEmpleado == idempleado);
            return empleado;
        }
    }
}
