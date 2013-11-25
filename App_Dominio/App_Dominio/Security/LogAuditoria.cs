using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Dominio.Security
{
    /*
    #region Funcionalidade
    public class FuncionalidadeModel : Model, IPinheiroSereniCrud
    {
        #region Construtor
        public FuncionalidadeModel()
        {

        }

        public FuncionalidadeModel(DataContext db)
            : base(db)
        {

        }
        #endregion

        #region Métodos da Interface IFinamCrud
        /// <summary>
        /// Consulta uma determinada funcionalidade pela sua descrição
        /// </summary>
        /// <param name="id">Deve ser passado a descrição da funcionalidade</param>
        /// <returns>Retorna um Objeto FUNCIONALIDADE pela descricao. Se a descrição não existir, a procedure inclui</returns>
        public Object getObject(Object id)
        {
            //ObjectParameter id_funcionalidade = new ObjectParameter("pId_Funcionalidade", 0);
            //pontoeletronico_db.SP_CONSULTAR_FUNCIONALIDADE(id_funcionalidade, id.ToString(), codErro, desErro);
            return new FUNCIONALIDADE() { ID_FUNCIONALIDADE = (int)id_funcionalidade.Value, DESCRICAO = id.ToString() };
        }

        public Validate Validate(Object value, Finam.Dominio.Enumeracoes.Crud operation)
        {
            Validate result = new Validate() { Code = 0 };
            return result;
        }

        public IEnumerable<Object> List()
        {
            return from f in pontoeletronico_db.FUNCIONALIDADE select f;
        }

        public Validate Insert(Object value)
        {
            return new Validate();
        }

        public Validate Update(Object value)
        {
            return new Validate();
        }

        public Validate Delete(Object value)
        {
            return new Validate();
        }
        #endregion
    }
    #endregion

    #region LogAuditoria

    public class LogAuditoria: Model, IPinheiroSereniCrud
    {
        #region Construtor
        public LogAuditoria()
        {

        }

        public LogAuditoria(DataContext db)
            : base(db)
        {

        }
        #endregion

        #region Métodos da Interface IPinheiroSereniCrud
        public Object getObject(Object id)
        {
            return pinheiroSereni_db.LOG_FINAM.Where(m => m.ID_LOG.Equals(id)).FirstOrDefault();
        }

        public Validate Validate(Object value, Finam.Dominio.Enumeracoes.Crud operation)
        {
            Validate result = new Validate() { Code = 0 };
            return result;
        }

        public IEnumerable<Object> List()
        {
            return null;
        }

        public Validate Insert(Object value)
        {
            Validate result = new Validate();

            try
            {
                result = Validate(value, Crud.INCLUIR);

                if (result.Code > 0)
                    throw new ArgumentException();

                System.Web.HttpContext web = System.Web.HttpContext.Current;
                FuncionalidadeModel f = new FuncionalidadeModel(finam_db);
                LogAuditoriaRepository log = (LogAuditoriaRepository)value;

                log.ID_FUNCIONALIDADE = ((FUNCIONALIDADE)f.getObject(log.stored_procedure)).ID_FUNCIONALIDADE;
                log.ID_LOGIN = web.Request.LogonUserIdentity.Name;

                if (log.DESCRICAO == null)
                {
                    log.DESCRICAO = "Parametro de entrada";
                    log.RETORNO_SP = "Parametro de saida";
                }

                XDocument documento = null;
                object[] content = { new XAttribute("HostName", log.ID_LOGIN), new XAttribute("Data", DateTime.Now.ToString()), new XAttribute("Procedimento", log.stored_procedure) };
                XElement root = new XElement("LOG", content);
                foreach (Parameter p in log.parameters)
                {
                    string valor = p.Value != null ? p.Value.ToString() : "NULL";
                    object[] content2 = { new XAttribute("Name", p.Name) };
                    XElement root2 = new XElement("Parametros", content2); 
                    root2.Add(new XElement("Value", valor));
                    root2.Add(new XElement("DataType", p.DataType));
                    root2.Add(new XElement("Length", p.Length));
                    root2.Add(new XElement("Type", p.Type));
                    root.Add(root2);
                }

                documento = new XDocument(new XElement("Auditoria"));
                documento.Root.Add(root);

                log.DESCRICAO = documento.ToString(System.Xml.Linq.SaveOptions.DisableFormatting).Replace("\\\"","\"") ;
                //log.DESCRICAO = documento.ToString();

                //foreach (Parameter p in log.parameters)
                //{
                //    string valor = p.Value != null ? p.Value.ToString() : "NULL";
                //    if (p.Type.ToLower().Equals("inout") || p.Type.ToLower().Equals("in"))
                //        log.DESCRICAO += " >> name=[" + p.Name + "], value=[" + valor + "], DataType=[" + p.DataType + "], Length=[" + p.Length + "], Type=[" + p.Type + "] **** ";
                //    else if (p.Type.ToLower().Equals("out"))
                //        log.RETORNO_SP += " name=[" + p.Name + "], value=[" + valor + "], DataType=[" + p.DataType + "], Length=[" + p.Length + "], Type=[" + p.Type + "] **** ";
                //}

                pontoeletronico_db.SP_LOG_AUDITORIA(
                    log.ID_LOGIN,
                    log.ID_FUNCIONALIDADE,
                    log.ACAO,
                    log.DESCRICAO,
                    log.TRACE,
                    log.RETORNO_SP,
                    codErro,
                    desErro);

                if ((int)codErro.Value > 0)
                    throw new FinamException(desErro.Value.ToString(), GetType().FullName);

                result.Code = (int)codErro.Value;
                result.Message = MensagemPadrao.Message(0).ToString();
            }
            catch (ArgumentException ex)
            {
                return result;
            }
            catch (FinamException ex)
            {
                return new Validate() { Field = result.Field, Code = (int)codErro.Value, Message = MensagemPadrao.Message((int)codErro.Value).ToString() };
            }
            catch (Exception ex)
            {
                FinamException.Exception(ex, GetType().FullName, FinamException.ErrorType.InsertError);
                result.Code = 3;
                result.Message = MensagemPadrao.Message(result.Code.Value).ToString();
            }

            return result;
        }

        public Validate Update(Object value)
        {
            return new Validate();
        }

        public Validate Delete(Object value)
        {
            return new Validate();
        }
        #endregion

        public void Generator(string sp_name)
        {
            System.IO.StreamWriter s = new System.IO.StreamWriter("C:\\log_code.txt");
            

        }
    }
    #endregion
      */

}