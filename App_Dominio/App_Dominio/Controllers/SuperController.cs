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

    public abstract class SuperController : MasterController
    {
        protected System.Data.Common.DbTransaction trans = null;
        public abstract int _sistema_id();

        #region CRUD
        #region Create

        [AuthorizeFilter]
        public ActionResult _Create(Repository value, ICrud model, ISuperController s = null)
        {
            if (ViewBag.ValidateRequest)
            {
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
            else
                return null;
        }
        #endregion

        #region Edit
        [AuthorizeFilter]
        public ActionResult _Edit(Repository value, ICrud model, ISuperController s = null)
        {
            if (ViewBag.ValidateRequest)
            {
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
            else
                return null;

        }
        #endregion

        #region Delete
        [AuthorizeFilter]
        public ActionResult _Delete(Repository value, ICrud model, ISuperController s = null)
        {
            if (ViewBag.ValidateRequest)
            {
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
            else
                return null;
        }
        #endregion
        #endregion

        #region Formulário Modal
        [AuthorizeFilter]
        public ActionResult ListModal(int? index, int? pageSize, IListRepository model, string header, params object[] param)
        {
            if (ViewBag.ValidateRequest)
            {
                IPagedList pagedList = model.getPagedList(index, pageSize.Value, param);

                ViewBag.Header = header;

                if (param != null && param.Count() > 0)
                    return View(pagedList);
                else
                    return View("LOVModal", pagedList);
            }
            else
                return null;
        }

        [AuthorizeFilter]
        public ActionResult ListModal(int? index, int? pageSize, IListRepository model, string header, string report, string controller, string action, params object[] param)
        {
            if (ViewBag.ValidateRequest)
            {
                IPagedList pagedList = model.getPagedList(index, report, controller, action, pageSize.Value, param);

                ViewBag.Header = header;

                if (param != null && param.Count() > 0)
                    return View(pagedList);
                else
                    return View("LOVModal", pagedList);
            }
            else
                return null;
        }

        [AuthorizeFilter]
        public ActionResult ListLovModal(int? index, int? pageSize, IListRepository model, string header, params object[] param)
        {
            if (ViewBag.ValidateRequest)
            {
                IPagedList pagedList = model.getPagedList(index, pageSize.Value, param);

                ViewBag.Header = header;

                return View("LOVModal", pagedList);               
            }
            else
                return null;
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
        [AuthorizeFilter]
        public ActionResult UpdateMiniCrud(MiniCrud miniCrud, string value, string text, string action, string DivId, Enumeradores.MiniCrudOperacao operation )
        {
            if (ViewBag.ValidateRequest)
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
            else
                return null;
        }
        #endregion
    }
}