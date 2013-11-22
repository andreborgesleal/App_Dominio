using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace App_Dominio.Enumeracoes
{
    public class MessageItem
    {
        public int key { get; set; }
        public string text { get; set; }
        public string type { get; set; }

        public override string ToString()
        {
            return text;
        }
    }

    public static class MensagemPadrao
    {
        const string PATH = "App_Data\\MensagemPadrao.xml";

        private static IEnumerable<MessageItem> ListaMensagens()
        {
            IList<MessageItem> list = new List<MessageItem>();

            System.Web.HttpContext web = System.Web.HttpContext.Current;
            string fileMessage = web.Request.PhysicalApplicationPath + PATH;
            if (!System.IO.File.Exists(fileMessage))
                return list;

            XDocument documento = XDocument.Load(fileMessage);

            IEnumerable<XElement> element = from c in documento.Descendants("Mensagem") select c;

            foreach (XElement e in element.Elements())
            {
                MessageItem item = new MessageItem()
                {
                    key = int.Parse(e.Attribute("key").Value),
                    text = e.Attribute("value").Value,
                    type = e.Attribute("type").Value
                };

                list.Add(item);
            }

            return list;
        }

        public static MessageItem Message(int key)
        {
            return ListaMensagens().Where(m => m.key.Equals(key)).FirstOrDefault();
        }

        public static MessageItem Message(int key, params string[] param)
        {
            MessageItem msg = ListaMensagens().Where(m => m.key.Equals(key)).FirstOrDefault();

            for (int i = 0; i <= param.Count() - 1; i++)
                msg.text = msg.text.Replace("{" + i.ToString().Trim() + "}", param[i]);

            return msg;
        }
    }

}