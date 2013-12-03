using App_Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App_Dominio.Security
{
    public class AuthorizeFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                Roles = filterContext.Controller.ControllerContext.RouteData.Values["controller"].ToString() + "/" + filterContext.Controller.ControllerContext.RouteData.Values["action"].ToString();
                if (new EmpresaSecurity<App_DominioContext>().AccessDenied(Roles))
                    filterContext.HttpContext.Response.Redirect("/Account/Login/");
            }
            catch (DbEntityValidationException ex)
            {
                filterContext.HttpContext.Response.Redirect("Error");
            }
            catch (Exception ex)
            {
                filterContext.HttpContext.Response.Redirect("Error");
            }

        }
    }
}
