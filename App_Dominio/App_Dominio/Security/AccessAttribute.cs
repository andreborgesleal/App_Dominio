using App_Dominio.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public class AccessAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            System.Web.HttpContext web = System.Web.HttpContext.Current;
            string sessionId = web.Session.SessionID;

            EmpresaSecurity security = new EmpresaSecurity();



            return !security.ValidarSessao(sessionId);


        }
    }
}
