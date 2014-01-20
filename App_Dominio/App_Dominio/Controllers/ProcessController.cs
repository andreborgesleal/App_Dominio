using App_Dominio.Contratos;
using App_Dominio.Component;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace App_Dominio.Controllers
{
    public abstract class ProcessController<R, T> : RootController<R, T>
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
                    //if (s != null)
                    //    s.beforeCreate(ref value, model, collection);

                    BeforeCreate(ref value, model, collection);

                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

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

                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

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

                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

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

        #region Save (JSon)
        public virtual JsonResult SaveJSon(R value, ICrudContext<R> model, string breadCrumbText = "Edição")
        {
            IDictionary<int, string> result = new Dictionary<int, string>();

            try
            {
                BeforeCreate(ref value, model, null);

                value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                value = ((IProcessContext<R>)model).SaveAll(value);
                if (value.mensagem.Code > 0)
                    throw new App_DominioException(value.mensagem);
            }
            catch (App_DominioException ex)
            {
                OnCreateError(ref value, model, null);
            }
            catch (Exception ex)
            {
                OnCreateError(ref value, model, null);
                App_DominioException.saveError(ex, GetType().FullName);
                value.mensagem = new Validate()
                {
                    Code = 17,
                    Message = MensagemPadrao.Message(17).ToString(),
                };
            }

            result.Add(value.mensagem.Code.Value, value.mensagem.ToString());

            return new JsonResult()
            {
                Data = result.ToArray(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region Delete (JSon)
        public virtual JsonResult DeleteJSon(R value, ICrudContext<R> model, string breadCrumbText = "Exclusão")
        {
            IDictionary<int, string> result = new Dictionary<int, string>();

            try
            {
                BeforeCreate(ref value, model, null);

                value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                value = ((IProcessContext<R>)model).Delete(value);
                if (value.mensagem.Code > 0)
                    throw new App_DominioException(value.mensagem);
            }
            catch (App_DominioException ex)
            {
                OnCreateError(ref value, model, null);
            }
            catch (Exception ex)
            {
                OnCreateError(ref value, model, null);
                App_DominioException.saveError(ex, GetType().FullName);
                value.mensagem = new Validate()
                {
                    Code = 17,
                    Message = MensagemPadrao.Message(17).ToString()
                };
            }

            result.Add(value.mensagem.Code.Value, value.mensagem.ToString());

            return new JsonResult()
            {
                Data = result.ToArray(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #endregion
    }
}
