using App_Dominio.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Dominio.Repositories
{
    public class ButtonRepository : Repository
    {
        public string linkText { get; set; }
        public string actionName { get; set; }
        public string controllerName { get; set; }
        /// <summary>
        /// "link" ou "submit"
        /// </summary>
        public string buttonType { get; set; }
        public string javaScriptFunction { get; set; }
        public string icon { get; set; }
        public string size { get; set; }
    }
}