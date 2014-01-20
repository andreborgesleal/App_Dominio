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
