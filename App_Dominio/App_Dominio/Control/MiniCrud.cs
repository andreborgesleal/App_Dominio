using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App_Dominio.Control
{
    public abstract class MiniCrud
    {
        private IList<SelectListItem> Items { get; set; }
        public System.Web.HttpContext sessao = System.Web.HttpContext.Current;
        public abstract String getId();

        public static IList<SelectListItem> getItemsBySession(string id)
        {
            if (System.Web.HttpContext.Current.Session[id] != null)
                return (IList<SelectListItem>)System.Web.HttpContext.Current.Session[id];
            else
                return new List<SelectListItem>();
        }

        public virtual void Add(string value, string text)
        {
            Items = getItems();

            if (Items.Where(info => info.Value == value).Count() == 0)
            {
                Items.Add(new SelectListItem() { Value = value, Text = text });
                Remove();
                sessao.Session.Add(getId(), Items);
            }
        }

        public virtual void Del(string value, string text)
        {
            Items = new List<SelectListItem>();
            foreach (SelectListItem i in getItems())
            {
                if (i.Value != value)
                    Items.Add(i);
            }
            Remove();
            sessao.Session.Add(getId(), Items);
        }

        public IList<SelectListItem> getItems()
        {
            if (sessao.Session[getId()] != null)
                return (IList<SelectListItem>)sessao.Session[getId()];
            else
                return new List<SelectListItem>();
        }


        public void Remove()
        {
            sessao.Session.Remove(getId());
        }

    }
}