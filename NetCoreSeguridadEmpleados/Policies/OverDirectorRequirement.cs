using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using NetCoreSeguridadEmpleados.Models;
using NetCoreSeguridadEmpleados.Repositories;
using System.Security.Claims;

namespace NetCoreSeguridadEmpleados.Policies
{
    public class OverDirectorRequirement : AuthorizationHandler<OverDirectorRequirement>, IAuthorizationRequirement
    {
        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, OverDirectorRequirement requirement)
        {
            if(context.User.HasClaim(x => x.Type == ClaimTypes.NameIdentifier) == false)
            {
                context.Fail();
            } else
            {
                var filterContext = context.Resource as AuthorizationFilterContext;
                var httpContext = filterContext.HttpContext;

                var repo = httpContext.RequestServices.GetService<RepositoryHospital>();
                string dato = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int id = int.Parse(dato);
                List<Empleado> subordinados = await repo.GetEmpleadosSubordinadosAsync(id); 
                if(subordinados.Count == 0)
                {
                    context.Fail();
                } else
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
