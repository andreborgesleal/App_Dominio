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
    public abstract class ExecController<R, T> : MasterController
        where R : Repository
        where T : IExecContext<R>
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

        #region Create
        [AuthorizeFilter]
        public virtual ActionResult Create()
        {
            if (ViewBag.ValidateRequest)
            {
                GetCreate();
                return View(getModel().CreateRepository(Request));
            }
            else
                return null;
        }

        [ValidateInput(false)]
        [HttpPost]
        [AuthorizeFilter]
        public virtual ActionResult Create(R value, FormCollection collection)
        {
            if (ViewBag.ValidateRequest)
            {
                R ret = SetCreate(value, getModel(), collection);

                if (ret.mensagem.Code == 0)
                    return RedirectToAction("Create");
                else
                    return View(ret);
            }
            else
                return null;
        }

        public virtual void GetCreate(string breadCrumbText = "Inclusão")
        {
            BindBreadCrumb(breadCrumbText);
        }

        public virtual void BeforeCreate(ref R value, IExecContext<R> model, FormCollection collection)
        {

        }
        public virtual R SetCreate(R value, IExecContext<R> model, FormCollection collection, string breadCrumbText = "Inclusão")
        {
            if (ModelState.IsValid)
                try
                {
                    BeforeCreate(ref value, model, collection);
                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                    value = ((IExecContext<R>)model).Run(value, Crud.INCLUIR);

                    if (value.mensagem.Code > 0)
                        throw new App_DominioException(value.mensagem);

                    Success("Processamento executado com sucesso");
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

        public virtual void OnCreateError(ref R value, IExecContext<R> model, FormCollection collection) { }
        #endregion

        #region Save (JSon)
        public virtual JsonResult SaveJSon(R value, IExecContext<R> model, string breadCrumbText = "Edição")
        {
            IDictionary<int, string> result = new Dictionary<int, string>();

            try
            {
                BeforeCreate(ref value, model, null);
                value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

                value = ((IExecContext<R>)model).Run(value, Crud.INCLUIR);

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
    }
}
