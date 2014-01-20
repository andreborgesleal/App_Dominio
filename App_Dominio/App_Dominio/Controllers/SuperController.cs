using App_Dominio.Contratos;
using App_Dominio.Component;
using App_Dominio.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Negocio;
using App_Dominio.Repositories;
using App_Dominio.Security;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public abstract class SuperController : Controller
    {
        protected System.Data.Common.DbTransaction trans = null;
        protected int PageSize = 50;
        protected Validate result { get; set; }

        public abstract int _sistema_id();

        #region Segurança
        protected bool AccessDenied(string url = null)
        {
            EmpresaSecurity<App_DominioContext> security = new EmpresaSecurity<App_DominioContext>();
            if (url != null)
                return security.AccessDenied(url);
            else
                return security.AccessDenied(this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString());
        }
        #endregion

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

        #region CRUD
        #region Browse
        #region abstract methods
        public abstract string getListName();

        public abstract ActionResult List(int? index, int? PageSize, string descricao = null);
        #endregion

        public string getReport(string report, string action)
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();

            FiltroModel f = new FiltroModel();
            return f.getReport(report, controller, action);
        }

        public virtual bool mustListOnLoad()
        {
            return true;
        }

        [AuthorizeFilter]
        public virtual ActionResult Browse(int? index = 0, int pageSize = 50, string descricao = null)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            BindBreadCrumb(getListName(), true);

            TempData.Remove("Controller");
            TempData.Add("Controller", this.ControllerContext.RouteData.Values["controller"].ToString());

            if (mustListOnLoad())
                return List(index, this.PageSize, descricao);
            else
                return View();
        }

        [AuthorizeFilter]
        public ActionResult _List(int? index, int? pageSize, string action, IListRepository model, params object[] param)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            IPagedList pagedList = model.getPagedList(index, pageSize.Value, param);

            UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);

            return View(pagedList);
        }

        [AuthorizeFilter]
        public ActionResult _List(int? index, int? pageSize, string report, string action, IListRepository model, params object[] param)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            IPagedList pagedList = model.getPagedList(index, report, this.ControllerContext.RouteData.Values["controller"].ToString(), action, pageSize.Value, param);

            if (pagedList.TotalCount == 0)
                Attention("Não há registros a serem exibidos");

            UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);

            return View(pagedList);
        }

        [AuthorizeFilter]
        public ActionResult _List(int? index, int? pageSize, string report, string action, IListRepository model, IMiniCrud miniCrud, params object[] param)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            IPagedList pagedList = model.getPagedList(index, report, this.ControllerContext.RouteData.Values["controller"].ToString(), action, pageSize.Value, param);

            if (pagedList.TotalCount == 0)
                Attention("Não há registros a serem exibidos");

            miniCrud.Create(pagedList.Filtros);

            UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);

            return View(pagedList);
        }

        public IPagedList PagedList(int? index, int? pageSize, string report, string action, IListRepository model, params object[] param)
        {
            IPagedList pagedList = model.getPagedList(index, report, this.ControllerContext.RouteData.Values["controller"].ToString(), action, pageSize.Value, param);
            return pagedList;
        }
        #endregion

        #region Create

        [AuthorizeFilter]
        public ActionResult _Create(Repository value, ICrud model, ISuperController s = null)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeCreate(ref value, model);

                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                    value = model.Insert(value);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro incluído com sucesso");
                    return RedirectToAction("Create");
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
            else
            {
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString(),
                    MessageBase = ModelState.Values.Where(erro => erro.Errors.Count > 0).First().Errors[0].ErrorMessage
                };
                ModelState.AddModelError("", value.mensagem.Message); // mensagem amigável ao usuário
                Attention(value.mensagem.MessageBase);
            }

            return View(value);
        }
        #endregion

        #region Edit
        [AuthorizeFilter]
        public ActionResult _Edit(Repository value, ICrud model, ISuperController s = null)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeEdit(ref value, model);

                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                    value = model.Update(value);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro alterado com sucesso");
                    return RedirectToAction("Edit", value);
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
            else
            {
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString(),
                    MessageBase = ModelState.Values.Where(erro => erro.Errors.Count > 0).First().Errors[0].ErrorMessage
                };
                ModelState.AddModelError("", value.mensagem.Message); // mensagem amigável ao usuário
                Attention(value.mensagem.MessageBase);
            }

            return View(value);

        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult _Delete(Repository value, ICrud model, ISuperController s = null)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeDelete(ref value, model);

                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                    value = model.Delete(value);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro excluído com sucesso");
                    return RedirectToAction("Browse");
                }
                catch (App_DominioException ex)
                {
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
            else
            {
                value.mensagem = new Validate()
                {
                    Code = 999,
                    Message = MensagemPadrao.Message(999).ToString(),
                    MessageBase = ModelState.Values.Where(erro => erro.Errors.Count > 0).First().Errors[0].ErrorMessage
                };
                ModelState.AddModelError("", value.mensagem.Message); // mensagem amigável ao usuário
                Attention(value.mensagem.MessageBase);
            }

            return View(value);
        }
        #endregion
        #endregion

        #region Formulário Modal
        public ActionResult ListModal(int? index, int? pageSize, IListRepository model, string header, params object[] param)
        {
            IPagedList pagedList = model.getPagedList(index, pageSize.Value, param);

            if (pagedList.TotalCount == 0)
                Attention("Não há registros a serem exibidos");

            ViewBag.Header = header;

            if (param != null && param.Count() > 0)
                return View(pagedList);
            else
                return View("LOVModal", pagedList);
        }

        public ActionResult ListModal(int? index, int? pageSize, IListRepository model, string header, string report, string controller, string action, params object[] param)
        {

            IPagedList pagedList = model.getPagedList(index, report, controller, action, pageSize.Value, param);

            if (pagedList.TotalCount == 0)
                Attention("Não há registros a serem exibidos");

            ViewBag.Header = header;

            if (param != null && param.Count() > 0)
                return View(pagedList);
            else
                return View("LOVModal", pagedList);
        }

        public ActionResult ListLovModal(int? index, int? pageSize, IListRepository model, string header, params object[] param)
        {

            IPagedList pagedList = model.getPagedList(index, pageSize.Value, param);

            if (pagedList.TotalCount == 0)
                Attention("Não há registros a serem exibidos");

            ViewBag.Header = header;

            return View("LOVModal", pagedList);               
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

        #region Parâmetros de pesquisa
        public virtual void SetEditParam(IEnumerable<FiltroRepository> values)
        {
            try
            {
                EmpresaSecurity<App_DominioContext> empresa = new EmpresaSecurity<App_DominioContext>();
                FiltroModel model = new FiltroModel();
                FiltroRepository f = values.First();
                int empresaId = empresa.getSessaoCorrente().empresaId;
                result = model.SaveCollection(values, info => info.report == f.report && info.controller == f.controller && info.action == f.action && info.empresaId == empresaId);
                if (result.Code > 0)
                    throw new App_DominioException(result);
            }
            catch (App_DominioException ex)
            {
                ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                if (ex.Result.MessageType == MsgType.ERROR)
                    Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                else
                    Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
            }
            catch (Exception ex)
            {
                App_DominioException.saveError(ex, GetType().FullName);
                ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
            }
        }
        #endregion

        #region Mini Crud
        public ActionResult UpdateMiniCrud(MiniCrud miniCrud, string value, string text, string action, string DivId, Enumeradores.MiniCrudOperacao operation )
        {
            switch (operation)
            {
                case Enumeradores.MiniCrudOperacao.ADD:
                    miniCrud.Add(value, text);
                    break;
                case Enumeradores.MiniCrudOperacao.DEL:
                    miniCrud.Del(value, text);
                    break;
                case Enumeradores.MiniCrudOperacao.CLEAR:
                    miniCrud.Remove();
                    break;
            }

            ViewBag.DivId = DivId;
            ViewBag.Action = action;
            ViewBag.Controller = this.ControllerContext.RouteData.Values["controller"].ToString();

            return View("_AddMiniCrud", miniCrud.getItems());
        }
        #endregion
    }
}