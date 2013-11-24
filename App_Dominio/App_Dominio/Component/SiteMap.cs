using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace App_Dominio.Component
{
    public class BreadCrumbItem
    {
        public string text { get; set; }    
        public string link
        {
            get
            {
                var _link = actionName + queryString;
                return _link;
            }
        }
        public string controllerName { get; set; }
        public string actionName { get; set; }
        public FormCollection collection { get; set; }
        public string queryString { get; set; }
    }

    public class BreadCrumb
    {
        public IList<BreadCrumbItem> items { get; set; }

        public BreadCrumb()
        {
            items = new List<BreadCrumbItem>();
        }

        public static BreadCrumb Create(TempDataDictionary TempData)
        {
            if (TempData.Peek("breadcrumb") == null)
                return new BreadCrumb();
            else
                return (BreadCrumb)TempData.Peek("breadcrumb");


            //if (System.Web.HttpContext.Current.Session["breadcrumb"] == null)
            //    return new BreadCrumb();
            //else
            //    return (BreadCrumb)System.Web.HttpContext.Current.Session["breadcrumb"];
        }

        public void Add(BreadCrumbItem item)
        {
            items.Add(item);            
        }

        public void Clear() 
        {
            items.Clear();
        }

        public void Remove(int index) 
        {
            if (index >= 0)
            {
                int total = items.Count - 1;
                for (int i = index; i <= total; i++)
                    items.RemoveAt(index);
            }
        }

        public void RemoveRest(string _controller, string _action)
        {
            Remove(getIndex(_controller, _action));
        }

        public int getIndex(string controller, string action)
        {
            int index = -1;
            int i = 0;
            foreach (BreadCrumbItem item in items)
            {
                if (item.controllerName == controller && item.actionName == action)
                {
                    index = i;
                    break;
                }
                i++;
            }

            return index;
        }

        public void setCollection(string controller, string action, FormCollection collection, string queryString)
        {
            items.ElementAt(getIndex(controller, action)).queryString = queryString;
            items.ElementAt(getIndex(controller, action)).collection = collection;
        }

        public BreadCrumbItem getItem(string controller, string action)
        {
            BreadCrumbItem breadCrumbItem = null;
            foreach (BreadCrumbItem item in items)
            {
                if (item.controllerName == controller && item.actionName == action)
                {
                    breadCrumbItem = item;
                    break;
                }
            }

            return breadCrumbItem;
        }

    }
}