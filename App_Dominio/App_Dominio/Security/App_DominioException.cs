using App_Dominio.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace App_Dominio.Security
{
    public class App_DominioException : Exception
    {
        public static class ErrorMessage
        {
            public static string GenericMessage = "Ocorreu uma exceção no processamento requisitado. Favor informar à central de atendimento o código de erro ";
            public static string InsertMessage = "Ocorreu um erro na inclusão do registro. Favor informar à central de atendimento o código de erro ";
            public static string UpdateMessage = "Ocorreu um erro na alteração do registro. Favor informar à central de atendimento o código de erro ";
            public static string DeleteMessage = "Ocorreu um erro na exclusão do registro. Favor informar à central de atendimento o código de erro ";
            public static string ListMessage = "Ocorreu um erro na listagem dos registros para consulta. Favor informar à central de atendimento o código de erro ";
            public static string DisplayMessage = "Ocorreu um erro na consulta do registro. Favor informar à central de atendimento o código de erro ";
            public static string SaveMessage = "Ocorreu um erro na gravação do registro. Favor informar o à central de atendimento o código de erro ";
            public static string PaginationMessage = "Ocorreu um erro na mudança de página da listagem dos dados. Favor informar à central de atendimento o código de erro ";
            public static string DrowpDownListMessage = "Ocorreu um erro na costrução do objeto DropDownList. Favor informar à central de atendimento o código de erro ";
            public static string UrlNotFondMessage = "O endereço de acesso está incorreto.";

        };

        public enum ErrorType
        {
            InsertError = 1,
            UpdateError = 2,
            DeleteError = 3,
            ListError = 4,
            DisplayError = 5,
            PaginationError = 6,
            SaveError = 7,
            DropDownListError = 8,
            UrlNotFondMessage = 9,
            GenericError = 99
        }

        public Validate Result { get; set; }

        public App_DominioException(string message, string type)
            : base(message)
        {
            saveError(this, type);
        }

        public App_DominioException(Validate result)
            : base()
        {
            this.Result = result;
        }

        /// <summary>
        /// Levanta uma exceção gravando o erro na tabela LogSessao para auditoria
        /// </summary>
        /// <param name="message">Messagem de erro gerado pelo try-catch</param>
        /// <param name="type">Deve ser preenchido com o comando "GetType().FullName"</param>
        /// <param name="errorType">Tipo de Erro (Inclusão, Alteração, Exclusão, etc.)</param>
        /// <returns>Retorna a mensagem de erro tratada para ser mostrada ao usuário</returns>
        public static string Exception(Exception ex, string type, ErrorType errorType)
        {
            return TrataErro(saveError(ex, type), errorType, ex.Message);
        }

        /// <summary>
        /// Levanta uma exceção gravando o erro na tabela LogSessao para auditoria
        /// </summary>
        /// <param name="message">Messagem de erro gerado pelo try-catch</param>
        /// <param name="type">Deve ser preenchido com o comando "GetType().FullName"</param>
        /// <param name="errorType">Tipo de Erro (Inclusão, Alteração, Exclusão, etc.)</param>
        /// <param name="complement">Complemento da mensagem a ser exibida ao usuário</param>
        /// <returns>Retorna a mensagem de erro tratada para ser mostrada ao usuário</returns>
        public static string Exception(Exception ex, string type, ErrorType errorType, string complement)
        {
            return TrataErro(saveError(ex, type), errorType, ex.Message, complement);
        }

        /// <summary>
        /// Levanta uma exceção gravando o erro na tabela LogSessao para auditoria
        /// </summary>
        /// <param name="message">Messagem de erro gerado pelo try-catch</param>
        /// <param name="type">Deve ser preenchido com o comando "GetType().FullName"</param>
        /// <param name="errorType">Tipo de Erro (Inclusão, Alteração, Exclusão, etc.)</param>
        /// <param name="complement">Complemento da mensagem a ser exibida ao usuário</param>
        /// <param name="cod_erro">Variável que armazenará o código do erro quando da inclusão na tabela LogError</param>
        /// <returns>Retorna a mensagem de erro tratada para ser mostrada ao usuário</returns>
        public static string Exception(Exception ex, string type, ErrorType errorType, string complement, ref int cod_erro)
        {
            cod_erro = saveError(ex, type);
            if (!String.IsNullOrEmpty(complement))
                return TrataErro(cod_erro, errorType, ex.Message, complement);
            else
                return TrataErro(cod_erro, errorType, ex.Message);
        }

        public static int saveError(Exception ex, string type)
        {
            LogError error = new LogError();
            error.msgError = ex.InnerException != null ? ex.InnerException.Message + " *** " + ex.Message : ex.Message;
            error.nameSpace = type + " => " + LogError.getCurrentUrl();
            error.stackTrace = ex.StackTrace;

            try
            {
                error.SaveError();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return error.idLogError;
        }

        private static string TrataErro(int cod_erro, ErrorType type, string err)
        {
            string prefixo = "";
            string sufixo = "";

            if (err.ToUpper().Contains("PRIMARY KEY"))
                sufixo += " => Código já cadastrado. Registro já existente";
            else if (err.ToUpper().Contains("FOREIGN KEY"))
                sufixo += " => Erro de integridade relacional. O erro ocorreu ao tentar gravar um código que não existe na tabela origem.";

            //Pegando o código do erro.
            //Caso o erro seja do tipo HttpException vai pegar o código do erro, 
            //ou então definirá o código como 500
            //Exception error = Server.GetLastError();
            //int code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;

            switch (type)
            {
                case ErrorType.InsertError:
                    prefixo = ErrorMessage.InsertMessage + cod_erro.ToString();
                    break;
                case ErrorType.UpdateError:
                    prefixo = ErrorMessage.UpdateMessage + cod_erro.ToString();
                    break;
                case ErrorType.DeleteError:
                    prefixo = ErrorMessage.DeleteMessage + cod_erro.ToString();
                    break;
                case ErrorType.ListError:
                    prefixo = ErrorMessage.ListMessage + cod_erro.ToString();
                    break;
                case ErrorType.DisplayError:
                    prefixo = ErrorMessage.DisplayMessage + cod_erro.ToString();
                    break;
                case ErrorType.PaginationError:
                    prefixo = ErrorMessage.PaginationMessage + cod_erro.ToString();
                    break;
                case ErrorType.SaveError:
                    prefixo = ErrorMessage.SaveMessage + cod_erro.ToString();
                    break;
                case ErrorType.GenericError:
                    prefixo = ErrorMessage.GenericMessage + cod_erro.ToString();
                    break;
                case ErrorType.DropDownListError:
                    prefixo = ErrorMessage.DrowpDownListMessage + cod_erro.ToString();
                    break;
                case ErrorType.UrlNotFondMessage:
                    prefixo = ErrorMessage.UrlNotFondMessage;
                    break;
            }

            return prefixo + sufixo;
        }

        private static string TrataErro(int cod_erro, ErrorType type, string err, string complement)
        {
            return TrataErro(cod_erro, type, err) + " ==>> " + complement;
        }

    }

    public class LogError
    {
        public int idLogError { get; set; }
        public string msgError { get; set; }
        public string sessao_id { get; set; }
        public string endereco_ip { get; set; }
        public string nameSpace { get; set; }
        public string stackTrace { get; set; }
        public string context { get; set; }
        public string hostName { get; set; }
        public string fileSession { get; set; }
        public DateTime data_ocorrencia { get; set; }

        public LogError()
        {
            System.Web.HttpContext web = System.Web.HttpContext.Current;
            //IPHostEntry hostInfo = Dns.GetHostByAddress(web.Request.UserHostAddress);

            idLogError = Randomize();
            for (int i = 0; i <= web.Request.Form.Keys.Count - 1; i++)
                context += web.Request.Form.Keys[i] + "=\"" + web.Request.Form.GetValues(i)[0] + "\"" + (i < web.Request.Form.Keys.Count - 1 ? "," : "");
            data_ocorrencia = DateTime.Now;
            if (web.Session != null)
                sessao_id = web.Session.SessionID;
            else
                sessao_id = "";
            endereco_ip = web.Request.UserHostAddress;
            hostName = web.Request.LogonUserIdentity.Name;
            fileSession = web.Request.PhysicalApplicationPath + "\\App_Data\\LOGs\\log" + "-" + System.DateTime.Today.ToString("ddMMyyyy") + ".xml";
        }

        public static string getCurrentUrl()
        {
            System.Web.HttpContext web = System.Web.HttpContext.Current;
            int contadorSegmentos = web.Request.Url.Segments.Count();
            int contadorParametros = web.Request.QueryString.Count;
            contadorParametros = 0;
            string virtualPath = "";
            for (int i = 1; i < contadorSegmentos - contadorParametros; i++)
                virtualPath += web.Request.Url.Segments[i].ToString();

            return virtualPath.ToLower();
        }

        public int Randomize()
        {
            Random random = new Random();

            int i = random.Next();
            return i;
        }

        public void SaveError()
        {
            XDocument documento = null;
            object[] content = { new XAttribute("ID", this.idLogError), new XAttribute("Data", this.data_ocorrencia.ToString()) };
            XElement root = new XElement("LOG", content);

            root.Add(new XElement("Sessao", this.sessao_id));
            root.Add(new XElement("IP", this.endereco_ip));
            root.Add(new XElement("HostName", this.hostName));
            root.Add(new XElement("Mensagem", this.msgError));
            root.Add(new XElement("NameSpace", this.nameSpace));
            root.Add(new XElement("Context", this.context));
            root.Add(new XElement("StackTrace", this.stackTrace));


            if (System.IO.File.Exists(this.fileSession))
                documento = XDocument.Load(fileSession);
            else
                documento = new XDocument(new XElement("LOG"));

            documento.Root.Add(root);

            documento.Root.Save(fileSession);
        }

    }
}