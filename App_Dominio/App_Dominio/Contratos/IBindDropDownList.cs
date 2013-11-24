using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Linq;
using App_Dominio.Entidades;

namespace App_Dominio.Contratos
{
    public interface IBindDropDownList
    {
        IEnumerable<SelectListItem> List(App_DominioContext app_dominio_db, params object[] param);
    }

}
