using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App_Dominio.Controllers
{
    public static class Alerts
    {
        public const string SUCCESS = "success";
        public const string ATTENTION = "attention";
        public const string ERROR = "danger";
        public const string INFORMATION = "info";

        public static string[] ALL
        {
            get { return new[] { SUCCESS, ATTENTION, INFORMATION, ERROR }; }
        }
    }

    public abstract class MasterController : Controller
    {
        protected int PageSize = 50;
        protected Validate result { get; set; }

        #region Messages
        public void Attention(string message)
        {
            CleanAlerts();
            TempData.Add(Alerts.ATTENTION, message);
        }

        public void Success(string message)
        {
            CleanAlerts();
            TempData.Add(Alerts.SUCCESS, message);
        }

        public void Information(string message)
        {
            CleanAlerts();
            TempData.Add(Alerts.INFORMATION, message);
        }

        public void Error(string message)
        {
            CleanAlerts();
            TempData.Add(Alerts.ERROR, message);
        }

        private void CleanAlerts()
        {
            TempData.Remove(Alerts.ATTENTION);
            TempData.Remove(Alerts.SUCCESS);
            TempData.Remove(Alerts.ERROR);
            TempData.Remove(Alerts.INFORMATION);
        }
        #endregion

        #region BreadCrumb
        public void BindBreadCrumb(string text, bool clear = false, FormCollection collection = null)
        {
            BreadCrumb caminhoPao = BreadCrumb.Create(TempData);
            if (clear)
                caminhoPao.Clear();
            else
                caminhoPao.RemoveRest(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

            if (Request.QueryString.Count > 0 && collection == null)
                collection = new FormCollection();

            for (int i = 0; i <= Request.QueryString.Count - 1; i++)
                collection.Add(Request.QueryString.AllKeys[i], Request.QueryString[i]);

            BreadCrumbItem item = new BreadCrumbItem()
            {
                text = text,
                controllerName = this.ControllerContext.RouteData.Values["controller"].ToString(),
                actionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                queryString = Request.QueryString.ToString() != "" ? "?" + Request.QueryString.ToString() : "",
                collection = collection
            };

            caminhoPao.Add(item);
            TempData.Remove("breadcrumb");
            TempData.Add("breadcrumb", caminhoPao);
            ViewBag.BreadCrumb = (BreadCrumb)TempData.Peek("breadcrumb");
        }

        public void UpdateBreadCrumb(string controller, string action, FormCollection collection = null)
        {
            BreadCrumb caminhoPao = BreadCrumb.Create(TempData);
            if (Request.QueryString.Count > 0 && collection == null)
                collection = new FormCollection();

            for (int i = 0; i <= Request.QueryString.Count - 1; i++)
                collection.Add(Request.QueryString.AllKeys[i], Request.QueryString[i]);

            caminhoPao.setCollection(controller, action, collection, Request.QueryString.ToString() != "" ? "?" + Request.QueryString.ToString() : "");
            TempData.Remove("breadcrumb");
            TempData.Add("breadcrumb", caminhoPao);
            ViewBag.BreadCrumb = (BreadCrumb)TempData.Peek("breadcrumb");
        }
        #endregion

        #region Browse
        #region abstract methods
        public abstract string getListName();

        public abstract ActionResult List(int? index, int? PageSize, string descricao = null);
        #endregion

        public virtual bool mustListOnLoad()
        {
            return true;
        }

        [AuthorizeFilter]
        public virtual ActionResult Browse(int? index = 0, int pageSize = 50, string descricao = null)
        {
            if (ViewBag.ValidateRequest)
            {
                BindBreadCrumb(getListName(), true);

                TempData.Remove("Controller");
                TempData.Add("Controller", this.ControllerContext.RouteData.Values["controller"].ToString());

                if (mustListOnLoad())
                    return List(index, this.PageSize, descricao);
                else
                    return View();
            }
            else
                return null;
        }

        [AuthorizeFilter]
        public ActionResult _List(int? index, int? pageSize, string action, IListRepository model, params object[] param)
        {
            if (ViewBag.ValidateRequest)
            {
                IPagedList pagedList = model.getPagedList(index, pageSize.Value, param);
                UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);
                return View(pagedList);
            }
            else
                return null;
        }

        [AuthorizeFilter]
        public ActionResult _List(int? index, int? pageSize, string report, string action, IListRepository model, params object[] param)
        {
            if (ViewBag.ValidateRequest)
            {
                IPagedList pagedList = model.getPagedList(index, report, this.ControllerContext.RouteData.Values["controller"].ToString(), action, pageSize.Value, param);
                UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);
                return View(pagedList);
            }
            else
                return null;
        }

        [AuthorizeFilter]
        public ActionResult _List(int? index, int? pageSize, string report, string action, IListRepository model, IMiniCrud miniCrud, params object[] param)
        {
            if (ViewBag.ValidateRequest)
            {
                IPagedList pagedList = model.getPagedList(index, report, this.ControllerContext.RouteData.Values["controller"].ToString(), action, pageSize.Value, param);
                miniCrud.Create(pagedList.Filtros);
                UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);
                return View(pagedList);
            }
            else
                return null;
        }

        public IPagedList PagedList(int? index, int? pageSize, string report, string action, IListRepository model, params object[] param)
        {
            IPagedList pagedList = model.getPagedList(index, report, this.ControllerContext.RouteData.Values["controller"].ToString(), action, pageSize.Value, param);
            return pagedList;
        }
        #endregion

    }
}
