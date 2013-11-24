using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Dominio.Contratos
{
    interface IValidate
    {
        int? Code { get; set; }
        string Field { get; set; }
        string Message { get; set; }
        string MessageBase { get; set; }
        Enumeracoes.MsgType MessageType { get; set; }
        string showMessage(IList<System.Web.Mvc.SelectListItem> param = null);
        string ToString();
    }

    public class Validate : IValidate
    {
        public int? Code { get; set; }
        public string Field { get; set; }
        public string Message { get; set; }
        public string MessageBase { get; set; }
        public Enumeracoes.MsgType MessageType { get; set; }
        public string Type { get; set; }

        public Validate(string _field, string _message)
        {
            Field = _field;
            Message = _message;
        }
        public Validate()
        {
            Field = "";
            Message = "";
            MessageType = App_Dominio.Enumeracoes.MsgType.SUCCESS;
        }

        public string showMessage(IList<System.Web.Mvc.SelectListItem> param = null)
        {
            if (Code == 0)
                return ShowMessageOk(param);
            else if (Code == 1)
                return ShowMessageAlert(param);
            return ShowMessageError(param);
        }

        private string ShowMessageError(IList<System.Web.Mvc.SelectListItem> param = null)
        {
            StringBuilder html = new StringBuilder();
            if (param != null)
            {
                html.Append("<div style=\"visibility: hidden; height: 0px\">");
                foreach (System.Web.Mvc.SelectListItem i in param)
                    html.Append("<input type=\"hidden\" id=\"" + i.Text + "\" value=\"" + i.Value + "\" />");
                html.Append("</div>");
            }
            html.Append("<div class=\"MensagemErro\">");
            html.Append("<div class=\"imagem\">");
            html.Append("           <img src=\"../Content/themes/base/images/Error.gif\" >");
            html.Append("       </div>");
            html.Append("       <div class=\"texto\">");
            html.Append("         " + Message);
            html.Append("       </div>");
            html.Append("</div>");

            return html.ToString();
        }

        private string ShowMessageOk(IList<System.Web.Mvc.SelectListItem> param = null)
        {
            StringBuilder html = new StringBuilder();

            if (param != null)
            {
                html.Append("<br>");
                html.Append("<div style=\"visibility: hidden; height: 0px\">");
                foreach (System.Web.Mvc.SelectListItem i in param)
                    html.Append("<input type=\"hidden\" id=\"" + i.Text + "\" value=\"" + i.Value + "\" />");
                html.Append("</div>");
            }

            html.Append("<div class=\"MensagemSucesso\">");
            html.Append("<div class=\"imagem\">");
            html.Append("           <img src=\"../Content/themes/base/images/Check.gif\" >");
            html.Append("       </div>");
            html.Append("       <div class=\"texto\">");
            html.Append("         " + Message);
            html.Append("       </div>");
            html.Append("</div>");

            return html.ToString();
        }

        private string ShowMessageAlert(IList<System.Web.Mvc.SelectListItem> param = null)
        {
            StringBuilder html = new StringBuilder();

            if (param != null)
            {
                html.Append("<br>");
                html.Append("<div style=\"visibility: hidden; height: 0px\">");
                foreach (System.Web.Mvc.SelectListItem i in param)
                    html.Append("<input type=\"hidden\" id=\"" + i.Text + "\" value=\"" + i.Value + "\" />");
                html.Append("</div>");
            }

            html.Append("<div class=\"MensagemAlerta\">");
            html.Append("<div class=\"imagem\">");
            html.Append("           <img src=\"../Content/themes/base/images/ico_alert2.gif\" >");
            html.Append("       </div>");
            html.Append("       <div class=\"texto\">");
            html.Append("         " + Message);
            html.Append("       </div>");
            html.Append("</div>");

            return html.ToString();
        }

        public override string ToString()
        {
            return Message;
        }
    }

}
