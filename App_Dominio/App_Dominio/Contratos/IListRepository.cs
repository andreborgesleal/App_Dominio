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
    public interface IListRepository 
    {
        IPagedList getPagedList(int? index, string report, string controllerName, string actionName, int pageSize = 50, params object[] param);
        IPagedList getPagedList(int? index, int pageSize = 50, params object[] param);
        IEnumerable<Repository> ListRepository(int? index, int pageSize = 50, params object[] param);
        Repository getRepository(Object id);
        string action();
        string DivId();
        IEnumerable<FiltroRepository> getFiltros(string report, string controllerName, string actionName, int pageSize = 50);
    }

    public interface IListReportRepository<R> where R : Repository
    {
        IPagedList getPagedList(int? index, string report, string controllerName, string actionName, int pageSize = 50, params object[] param);
        IPagedList getPagedList(int? index, int pageSize = 50, params object[] param);
        IEnumerable<Repository> ListRepository(int? index, int pageSize = 50, params object[] param);
        Repository getRepository(Object id);
        string action();
        string DivId();
        IEnumerable<FiltroRepository> getFiltros(string report, string controllerName, string actionName, int pageSize = 50);
        IEnumerable<R> ListReportRepository(params object[] param);
        IEnumerable<R> LineBreak(IEnumerable<R> repository);
    }

}
