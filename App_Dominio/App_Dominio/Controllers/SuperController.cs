using App_Dominio.Contratos;
using App_Dominio.Component;
using App_Dominio.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Negocio;
using App_Dominio.Repositories;
using App_Dominio.Security;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

            if (pagedList.TotalCount == 0)
                Attention("Não há registros a serem exibidos");

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

    public abstract class ReportController<R> : SuperController where R : Repository
    {
        #region Exportar para PDF (Report Server)
        public FileResult _PDF(string export, string fileName, ReportRepository<R, App_DominioContext> report, ReportParameter[] p,
                                string PageWidth = "21cm", string PageHeight = "29,7cm", params object[] param)
        {
            p[0] = new ReportParameter("empresa", new EmpresaSecurity<App_DominioContext>().getEmpresa().nome, false);

            LocalReport relatorio = new LocalReport();
            relatorio.ReportPath = Server.MapPath("~/App_Data/rdlc/" + fileName + ".rdlc");
            IEnumerable<IReportRepository<R>> r = (IEnumerable<IReportRepository<R>>)report.ListReportRepository(param);
            relatorio.DataSources.Add(new ReportDataSource("DataSet1", r));

            relatorio.SetParameters(p);
            relatorio.Refresh();

            string reportType = "PDF";
            string reportFile = fileName + ".pdf";

            if (export == "png")
            {
                reportType = "Image";
                reportFile = fileName + ".png";
            }
            else if (export == "excel")
            {
                reportType = "Excel";
                reportFile = fileName + ".xls";
            }
            else if (export == "word")
            {
                reportType = "Word";
                reportFile = fileName + ".doc";
            }

            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
             "<DeviceInfo>" +
             " <OutputFormat>PDF</OutputFormat>" +
             " <PageWidth>" + PageWidth + "</PageWidth>" +
             " <PageHeight>" + PageHeight + "</PageHeight>" +
             " <MarginTop>0.5cm</MarginTop>" +
             " <MarginLeft>0.5cm</MarginLeft>" +
             " <MarginRight>0.5cm</MarginRight>" +
             " <MarginBottom>0.5cm</MarginBottom>" +
             "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] bytes;

            //Renderiza o relatório em bytes
            bytes = relatorio.Render(
            reportType,
            deviceInfo,
            out mimeType,
            out encoding,
            out fileNameExtension,
            out streams,
            out warnings);

            if (export != "view")
                return File(bytes, mimeType, reportFile);
            else
                return File(bytes, mimeType);
        }
        #endregion
    }

    public abstract class RootController<R, T> : SuperController
        where R : Repository
        where T : ICrudContext<R>
    {
        protected T getModel()
        {
            Type typeInstance = typeof(T);
            return (T)Activator.CreateInstance(typeInstance);
        }

        protected string getBreadCrumbText(string breadCrumbText = null, IDictionary<string, string> text = null)
        {
            if (breadCrumbText == null)
            {
                if (text == null)
                {
                    text = new Dictionary<string, string>();
                    text.Add("edit", "Edição");
                    text.Add("detail", "Detalhe");
                    text.Add("delete", "Exclusão");
                }
                breadCrumbText = text[this.ControllerContext.RouteData.Values["action"].ToString().ToLower()];
            }

            return breadCrumbText;
        }

        #region CRUD
        #region Browse


        //public ActionResult _List(int? index, int? pageSize, string action, IListRepository model, params object[] param)
        //{
        //    if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
        //        return RedirectToAction("Index", "Home");

        //    IPagedList pagedList = model.getPagedList(index, this.ControllerContext.RouteData.Values["controller"].ToString(), action, pageSize.Value, param);

        //    if (pagedList.TotalCount == 0)
        //        Attention("Não há registros a serem exibidos");

        //    UpdateBreadCrumb(this.ControllerContext.RouteData.Values["controller"].ToString(), action);

        //    return View(pagedList);
        //}


        #endregion

        #region Create
        [AuthorizeFilter]
        public virtual ActionResult Create(R value = null)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            GetCreate();

            return View(getModel().CreateRepository(value));
        }

        [ValidateInput(false)]
        [HttpPost]
        [AuthorizeFilter]
        public virtual ActionResult Create(R value, FormCollection collection)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            R ret = SetCreate(value, getModel(), collection);

            if (ret.mensagem.Code == 0)
                return RedirectToAction("Create");
            else
                return View(ret);
        }

        public virtual void GetCreate(string breadCrumbText = "Inclusão")
        {
            BindBreadCrumb(breadCrumbText);
        }

        public virtual R SetCreate(R value, ICrudContext<R> model, FormCollection collection, string breadCrumbText = "Inclusão", IBaseController<R> s = null)
        {
            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeCreate(ref value, model, collection);

                    value = model.Insert(value);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro incluído com sucesso");
                }
                catch (App_DominioException ex)
                {
                    OnCreateError(ref value, model, collection);
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    OnCreateError(ref value, model, collection);
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                finally
                {
                    BindBreadCrumb(breadCrumbText);
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

            return value;
        }

        public virtual void OnCreateError(ref R value, ICrudContext<R> model, FormCollection collection) {}
        #endregion

        #region Edit
        [AuthorizeFilter]
        public virtual ActionResult _Edit(R value, string breadCrumbText = null, IDictionary<string, string> text = null)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            return View(GetEdit(value, breadCrumbText, text));
        }

        [ValidateInput(false)]
        [HttpPost]
        [AuthorizeFilter]
        public virtual ActionResult Edit(R value, FormCollection collection)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            R ret = SetEdit(value, getModel(), collection);
            
            if (ret.mensagem.Code == 0)
            {
                BreadCrumb b = (BreadCrumb)ViewBag.BreadCrumb;
                if (b.items.Count > 1)
                {
                    string[] split = b.items[b.items.Count - 2].queryString.Split('&');
                    string _index = split[0].Replace("?index=", "");
                    return RedirectToAction(b.items[b.items.Count - 2].actionName, b.items[b.items.Count - 2].controllerName, new { index = _index });
                }
                else
                    return RedirectToAction("Principal", "Home");
            }
            else
                return View(ret);
        }

        public virtual R GetEdit(R key, string breadCrumbText = null, IDictionary<string, string> text = null)
        {
            BindBreadCrumb(getBreadCrumbText(breadCrumbText, text));

            return getModel().getObject(key);
        }

        public virtual R SetEdit(R value, ICrudContext<R> model, FormCollection collection, string breadCrumbText = null, IDictionary<string, string> text = null, IRootController<R> s = null)
        {
            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeEdit(ref value, model);

                    value = model.Update(value);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro alterado com sucesso");
                }
                catch (App_DominioException ex)
                {
                    OnEditError(ref value, model, collection);
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    OnEditError(ref value, model, collection);
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                finally
                {
                    BindBreadCrumb(getBreadCrumbText(breadCrumbText, text));
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

            return value;

        }
        public virtual void OnEditError(ref R value, ICrudContext<R> model, FormCollection collection) { }
        #endregion

        #region Delete
        [HttpPost]
        [AuthorizeFilter]
        public virtual ActionResult Delete(R value, FormCollection collection)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            R ret = SetDelete(value, getModel(), collection);

            if (ret.mensagem.Code == 0)
            {
                BreadCrumb b = (BreadCrumb)ViewBag.BreadCrumb;
                if (b.items.Count > 1)
                {
                    string[] split = b.items[b.items.Count - 2].queryString.Split('&');
                    string _index = split[0].Replace("?index=", "");
                    return RedirectToAction(b.items[b.items.Count - 2].actionName, b.items[b.items.Count - 2].controllerName, new { index = _index });
                }
                else
                    return RedirectToAction("Principal", "Home");
            }
            else
                return View(ret);
        }

        public virtual R SetDelete(R value, ICrudContext<R> model, FormCollection collection, string breadCrumbText = null, IDictionary<string, string> text = null, IRootController<R> s = null)
        {
            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeDelete(ref value, model);

                    value = model.Delete(value);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro excluído com sucesso");
                }
                catch (App_DominioException ex)
                {
                    OnDeleteError(ref value, model, collection);
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    OnDeleteError(ref value, model, collection);
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                finally
                {
                    BindBreadCrumb(getBreadCrumbText(breadCrumbText, text));
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

            return value;
        }
        public virtual void OnDeleteError(ref R value, ICrudContext<R> model, FormCollection collection) { }
        #endregion
        #endregion

        #region Typeahead
        public JsonResult JSonTypeahead(string term, IListRepository model)
        {
            var results = model.ListRepository(0, 100, term).ToList();
            return new JsonResult()
            {
                Data = results,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult JSonCrud(R value, IRootController<R> s = null)
        {
            T model = getModel();
            R result = CreateModal(value, model, s);

            return new JsonResult()
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        protected R CreateModal(R value, ICrudContext<R> model, IRootController<R> s = null)
        {
            try
            {
                if (s != null)
                    s.beforeCreate(ref value, model);

                value = model.Insert(value);
                if (value.mensagem.Code > 0)
                    throw new App_DominioException(value.mensagem);
            }
            catch (App_DominioException ex)
            {
                ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
            }
            catch (Exception ex)
            {
                App_DominioException.saveError(ex, GetType().FullName);
                ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
            }

            return (R)value;
        }
        #endregion
    }

    public abstract class ProcessController<R,T> : RootController<R,T>
        where R : Repository
        where T : IProcessContext<R>
    {
        #region CRUD
        #region Create
        public override R SetCreate(R value, ICrudContext<R> model, FormCollection collection, string breadCrumbText = "Inclusão", IBaseController<R> s = null)
        {
            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeCreate(ref value, model, collection);

                    value = ((IProcessContext<R>)model).SaveAll(value);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro incluído com sucesso");
                }
                catch (App_DominioException ex)
                {
                    OnCreateError(ref value, model, collection);
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    
                    value.mensagem = new Validate()
                    {
                        Code = ex.Result.Code,
                        Message = ex.Result.Message,
                        MessageBase = ex.Result.MessageBase
                    };
                }
                catch (Exception ex)
                {
                    OnCreateError(ref value, model, collection);
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    value.mensagem = new Validate()
                    {
                        Code = 999,
                        Message = MensagemPadrao.Message(999).ToString(),
                        MessageBase = ModelState.Values.Where(erro => erro.Errors.Count > 0).First().Errors[0].ErrorMessage
                    };
                }
                finally
                {
                    BindBreadCrumb(breadCrumbText);
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

            return value;
        }
        #endregion

        #region Edit
        public override R SetEdit(R value, ICrudContext<R> model, FormCollection collection, string breadCrumbText = null, IDictionary<string, string> text = null, IRootController<R> s = null)
        {
            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeEdit(ref value, model);

                    value = ((IProcessContext<R>)model).SaveAll(value);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro alterado com sucesso");
                }
                catch (App_DominioException ex)
                {
                    OnEditError(ref value, model, collection);
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    OnEditError(ref value, model, collection);
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                finally
                {
                    BindBreadCrumb(getBreadCrumbText(breadCrumbText, text));
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

            return value;

        }
        #endregion

        #region Delete
        public override R SetDelete(R value, ICrudContext<R> model, FormCollection collection, string breadCrumbText = null, IDictionary<string, string> text = null, IRootController<R> s = null)
        {
            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeDelete(ref value, model);

                    value = ((IProcessContext<R>)model).SaveAll(value);
                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Registro excluído com sucesso");
                }
                catch (App_DominioException ex)
                {
                    OnDeleteError(ref value, model, collection);
                    ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                    if (ex.Result.MessageType == MsgType.ERROR)
                        Error(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                    else
                        Attention(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                catch (Exception ex)
                {
                    OnDeleteError(ref value, model, collection);
                    App_DominioException.saveError(ex, GetType().FullName);
                    ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                    Error(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
                }
                finally
                {
                    BindBreadCrumb(getBreadCrumbText(breadCrumbText, text));
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

            return value;
        }
        #endregion
        #endregion
    }


    public abstract class RootItemController<M, T, I, P> : RootController<M, T>
        where M : Repository
        where T : ICrudContext<M>
        where I : Repository
        where P : ICrudItemContext<I>
    {
        #region Virtual methods
        public abstract string getName();

        public abstract M setRepositoryAfterError(M value, FormCollection collection);
        #endregion

        #region Master
        [AuthorizeFilter]
        [HttpPost]
        public override ActionResult Create(M value, FormCollection collection)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            IMasterRepository<I> x = GetMaster((IMasterRepository<I>)value);
            ((IMasterRepository<I>)value).SetItems(x.GetItems());

            M ret = SetCreate(value, getModel(), collection);

            if (ret.mensagem.Code == 0)
                return RedirectToAction("Create");
            else
            {
                value = (M)GetMaster((IMasterRepository<I>)value); // recupera os valores da sessão
                value = setRepositoryAfterError(value, collection); // preenche os lookups, dropdownslists etc 

                ((IMasterRepository<I>)ret).SetItem(((IMasterRepository<I>)value).GetItem());

                return View(ret);
            }
        }

        [AuthorizeFilter]
        public override ActionResult _Edit(M value, string breadCrumbText = null, IDictionary<string, string> text = null)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            value = getModel().getObject(value);
            if (value == null)
            {
                Attention("ID não encontrado");
                return RedirectToAction("Create");
            }

            if (TempData.Peek(getName()) != null)
                TempData.Remove(getName());
            TempData.Add(getName(), value);

            return View(GetEdit(value, breadCrumbText, text));
        }

        public ActionResult _Detail(M value, string breadCrumbText = null, IDictionary<string, string> text = null)
        {
            TempData.Add("NoInput", true);

            return _Edit(value, breadCrumbText, text);
        }


        [AuthorizeFilter]
        [HttpPost]
        public override ActionResult Edit(M value, FormCollection collection)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            IMasterRepository<I> x = GetMaster((IMasterRepository<I>)value);
            ((IMasterRepository<I>)value).SetItems(x.GetItems());

            M ret = SetEdit(value, getModel(), collection);

            if (ret.mensagem.Code == 0)
            {
                BreadCrumb b = (BreadCrumb)ViewBag.BreadCrumb;
                if (b.items.Count > 1)
                {
                    string[] split = b.items[b.items.Count - 2].queryString.Split('&');
                    string _index = split[0].Replace("?index=", "");
                    return RedirectToAction(b.items[b.items.Count - 2].actionName, b.items[b.items.Count - 2].controllerName, new { index = _index });
                }
                else
                    return RedirectToAction("Principal", "Home");
            }
            else
            {
                value = (M)GetMaster((IMasterRepository<I>)value); // recupera os valores da sessão
                value = setRepositoryAfterError(value, collection); // preenche os lookups, dropdownslists etc 

                ((IMasterRepository<I>)ret).SetItem(((IMasterRepository<I>)value).GetItem());

                return View(ret);
            }
        }

        [AuthorizeFilter]
        [HttpPost]
        public override ActionResult Delete(M value, FormCollection collection)
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            IMasterRepository<I> x = GetMaster((IMasterRepository<I>)value);
            ((IMasterRepository<I>)value).SetItems(x.GetItems());

            M ret = SetDelete(value, getModel(), collection);

            if (ret.mensagem.Code == 0)
            {
                BreadCrumb b = (BreadCrumb)ViewBag.BreadCrumb;
                if (b.items.Count > 1)
                {
                    string[] split = b.items[b.items.Count - 2].queryString.Split('&');
                    string _index = split[0].Replace("?index=", "");
                    return RedirectToAction(b.items[b.items.Count - 2].actionName, b.items[b.items.Count - 2].controllerName, new { index = _index });
                }
                else
                    return RedirectToAction("Principal", "Home");
            }
            else
            {
                value = (M)GetMaster((IMasterRepository<I>)value); // recupera os valores da sessão
                value = setRepositoryAfterError(value, collection); // preenche os lookups, dropdownslists etc 

                ((IMasterRepository<I>)ret).SetItem(((IMasterRepository<I>)value).GetItem());

                return View(ret);
            }
        }

        #endregion

        #region CrudItem
        protected P getModel(IList<I> list)
        {
            Type typeInstance = typeof(P);
            P instance = (P)Activator.CreateInstance(typeInstance);

            instance.SetListItem(list);
            return instance;
        }

        public IMasterRepository<I> GetMaster(IMasterRepository<I> value)
        {
            IList<I> list = new List<I>();

            if (TempData.Peek(getName()) != null)
            {
                value = (IMasterRepository<I>)TempData.Peek(getName());
                foreach (I x in value.GetItems())
                    list.Add(x);
            }

            value.SetItems(list);

            return value;
        }

        public IEnumerable<I> GetItems(IMasterRepository<I> value)
        {
            IList<I> list = new List<I>();

            if (TempData.Peek(getName()) != null)
            {
                value = (IMasterRepository<I>)TempData.Peek(getName());
                foreach (I x in value.GetItems())
                    list.Add(x);
            }

            value.SetItems(list);

            return value.GetItems();
        }

        public virtual ActionResult GetItem(Func<I, bool> key, IMasterRepository<I> master)
        {
            master = this.GetMaster(master);
            master.SetItem(master.GetItems().Where(key).First());

            return View((M)master);
        }

        public virtual I AddItem(I value, P model, IRootController<I> s = null)
        {
            model.SetKey(value); // obtêm e atribui a chave primária para o item 

            if (s != null)
                s.beforeCreate(ref value, model);

            return model.Insert(value);
        }

        public virtual I SaveItem(I value, P model, IRootController<I> s = null)
        {
            if (s != null)
                s.beforeEdit(ref value, model);

            return model.Update(value);
        }

        public virtual I DelItem(I value, P model, IRootController<I> s = null)
        {
            if (s != null)
                s.beforeDelete(ref value, model);

            return model.Delete(value);
        }

        [AuthorizeFilter]
        public ActionResult _NewItem(IMasterRepository<I> master, string createItemAction = "CreateItem")
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            master = GetMaster(master); // recupera da sessao o repository master
            master.CreateItem();

            return View(createItemAction, master);
        }

        [AuthorizeFilter]
        public virtual ActionResult UpdateItem(I value, IMasterRepository<I> master, string operacao, IRootController<I> s = null, string actions = "EditItem|DeleteItem")
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            master = GetMaster(master); // recupera da sessao o repository master
            P model = getModel(master.GetItems().ToList()); // cria uma instância do model e já atribui a ele o ListItem que estava na sessao
            string _defaultErrorRoute = "";

            try
            {
                switch (operacao)
                {
                    case "I":
                        value = AddItem(value, model, s);
                        break;
                    case "A":
                        _defaultErrorRoute = actions.Split('|')[0];
                        value = SaveItem(value, model, s);
                        break;
                    case "E":
                        _defaultErrorRoute = actions.Split('|')[1];
                        value = DelItem(value, model, s);
                        break;
                }

                if (value.mensagem.Code > 0)
                    throw new App_DominioException(value.mensagem);

                master.SetItems(model.ListAll());

                TempData.Remove(getName());
                TempData.Add(getName(), master);

                master.CreateItem(); // cria uma instância do ItemRepository para ser exibida na tela para uma nova inclusão

                return View(master);
            }
            catch (App_DominioException ex)
            {
                ModelState.AddModelError(ex.Result.Field, ex.Result.Message); // mensagem amigável ao usuário
                Information(ex.Result.MessageBase); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
            }
            catch (Exception ex)
            {
                App_DominioException.saveError(ex, GetType().FullName);
                ModelState.AddModelError("", MensagemPadrao.Message(17).ToString()); // mensagem amigável ao usuário
                Information(ex.Message); // Mensagem em inglês com a descrição detalhada do erro e fica no topo da tela
            }

            master.SetItem(value);

            if (operacao != "I")
            {
                TempData.Add("master", master);
                return RedirectToAction(_defaultErrorRoute, new { sequencial = 0 });
            }
            else
                return View(master);
        }
        #endregion
    }
}