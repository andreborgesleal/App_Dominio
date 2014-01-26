using App_Dominio.Entidades;
using System;
using System.Data.Entity.Validation;
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
                int value = new EmpresaSecurity<App_DominioContext>().AccessDenied(Roles);
                filterContext.Controller.ViewBag.ValidateRequest = true;
                if (value > 0)
                {
                    filterContext.Controller.ViewBag.ValidateRequest = false;
                    if (Order < 999 && !Roles.ToLower().Contains("modal") && !filterContext.Controller.ControllerContext.RouteData.Values["action"].ToString().StartsWith("List"))
                    {
                        if (value == 1)
                            filterContext.HttpContext.Response.Redirect("/Account/Login/");
                        else if (value == 2)
                            filterContext.HttpContext.Response.Redirect("/Home/_Error");
                    }
                }                    
            }
            catch (DbEntityValidationException ex)
            {
                filterContext.HttpContext.Response.Redirect("/Home/_Error");
            }
            catch (Exception ex)
            {
                filterContext.HttpContext.Response.Redirect("/Home/_Error");
            }

        }
    }
}
