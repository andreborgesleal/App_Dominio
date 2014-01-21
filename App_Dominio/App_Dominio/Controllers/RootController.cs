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
        #region Create
        [AuthorizeFilter]
        public virtual ActionResult Create()
        {
            //if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
            //    return RedirectToAction("Index", "Home");

            if (ValidateRequest)
            {
                GetCreate();
                return View(getModel().CreateRepository());
            }
            else
            {
                return null;
            }
                
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

        public virtual void BeforeCreate(ref R value, ICrudContext<R> model, FormCollection collection)
        {

        }
        public virtual R SetCreate(R value, ICrudContext<R> model, FormCollection collection, string breadCrumbText = "Inclusão", IBaseController<R> s = null)
        {
            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeCreate(ref value, model, collection);
                    else
                        BeforeCreate(ref value, model, collection);

                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

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

        public virtual void OnCreateError(ref R value, ICrudContext<R> model, FormCollection collection) { }
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

        public virtual void BeforeEdit(ref R value, ICrudContext<R> model, FormCollection collection) { }

        public virtual R SetEdit(R value, ICrudContext<R> model, FormCollection collection, string breadCrumbText = null, IDictionary<string, string> text = null, IRootController<R> s = null)
        {
            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeEdit(ref value, model);
                    else
                        BeforeEdit(ref value, model, collection);

                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

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
        [ValidateInput(false)]
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
        public virtual void BeforeDelete(ref R value, ICrudContext<R> model, FormCollection collection) { }
        public virtual R SetDelete(R value, ICrudContext<R> model, FormCollection collection, string breadCrumbText = null, IDictionary<string, string> text = null, IRootController<R> s = null)
        {
            if (ModelState.IsValid)
                try
                {
                    if (s != null)
                        s.beforeDelete(ref value, model);
                    else
                        BeforeDelete(ref value, model, collection);

                    value.uri = this.ControllerContext.Controller.GetType().Name.Replace("Controller", "") + "/" + this.ControllerContext.RouteData.Values["action"].ToString();

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

        public JsonResult JSonSelectListItem(string term, IListSelectItem model)
        {
            var results = model.getListItems(term).ToList();
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
}
