using App_Dominio.Component;
using App_Dominio.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App_Dominio.Contratos
{
    public interface ISuperController
    {
        void beforeCreate(ref Repository value, ICrud model);
        void beforeEdit(ref Repository value, ICrud model);
        void beforeDelete(ref Repository value, ICrud model);
    }

    public interface IRootController<R> where R : Repository
    {
        void beforeCreate(ref R value, ICrudContext<R> model);
        void beforeEdit(ref R value, ICrudContext<R> model);
        void beforeDelete(ref R value, ICrudContext<R> model);
    }

    public interface IMiniCrud
    {
        void Create(IEnumerable<FiltroRepository> Filtros);
        IList<SelectListItem> getItems();
    }

}
