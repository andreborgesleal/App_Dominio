using App_Dominio.Contratos;
using App_Dominio.Component;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace App_Dominio.Entidades
{
    public abstract class Context<D> where D : DbContext
    {
        protected D getContextInstance()
        {
            Type typeInstance = typeof(D);
            return (D)Activator.CreateInstance(typeInstance);
        }

        public D db { get; set; }
        public SecurityContext seguranca_db { get; set; }
        public D Create(D value)
        {
            this.db = value;

            return db;
        }

        public SecurityContext Create(SecurityContext value)
        {
            this.seguranca_db = value;

            return seguranca_db;
        }

        public D Create()
        {
            db = getContextInstance();
            seguranca_db = new SecurityContext();
            sessaoCorrente = seguranca_db.Sessaos.Find(System.Web.HttpContext.Current.Session.SessionID);
            return db;
        }

        public Sessao sessaoCorrente { get; set; }

    }

    public abstract class CrudContext<E, R, D> : Context<D>, ICrudContext<R>
        where E : class
        where R : Repository
        where D : DbContext
    {
        public LogAuditoria logAuditoria { get; set; }

        protected E getEntityInstance()
        {
            Type typeInstance = typeof(E);
            return (E)Activator.CreateInstance(typeInstance);
        }

        #region Métodos virtuais
        public abstract E MapToEntity(R value);

        public abstract R MapToRepository(E entity);

        public abstract E Find(R key);

        public abstract Validate Validate(R value, Crud operation);

        public virtual R CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            Type typeInstance = typeof(R);
            R Instance = (R)Activator.CreateInstance(typeInstance);

            return Instance;
        }

        public virtual void SaveLog(E entity, string url, string operacao = "Atualizar", string tag = null)
        {
            #region Log de Auditoria
            System.Reflection.PropertyInfo[] atributes = entity.GetType().GetProperties();

            string displayName = "";
            string notacao = "<?xml version=\"1.0\"?>\r\n";
            notacao += "<" + entity.GetType().Name + ">\r\n";
            notacao += "<Context name=\"Operação\" value=\"" + operacao + "\"></Context>\r\n";

            for (int i = 0; i <= atributes.Count() - 1; i++)
            {
                if (atributes[i].CustomAttributes.Count() > 0 && atributes[i].CustomAttributes.LastOrDefault().ToString().Contains("DisplayNameAttribute"))
                    displayName = atributes[i].CustomAttributes.LastOrDefault().ToString().Replace("[System.ComponentModel.DisplayNameAttribute(\"", "").Replace("\")]", "");
                else
                    displayName = atributes[i].Name;

                if (displayName.ToLower() != "xml")
                {
                    if (atributes[i].GetValue(entity) != null)
                        notacao += "<Context name=\"" + displayName + "\" value=\"" + atributes[i].GetValue(entity).ToString().Replace("\"", "'").Replace("<", "[").Replace(">", "]") + "\"></Context>\r\n";
                    else
                        notacao += "<Context name=\"" + displayName + "\" value=\"\"></Context>\r\n";
                }
            }

            if (tag != null && tag != "")
                notacao += "<Context name=\"tag\" value=\"" + tag + "\"></Context>\r\n";

            notacao += "</" + entity.GetType().Name + ">\r\n";

            int _sistemaId = 0;
            int _usuarioId = 0;
            int _empresaId = 0;
            string _ip = "";
            if (sessaoCorrente == null)
            {
                _sistemaId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["sistemaId"]);
                _empresaId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["empresaId"]);
                _usuarioId = (from usu in seguranca_db.Usuarios
                              where usu.empresaId == _empresaId && usu.isAdmin == "S" && usu.situacao == "A"
                              select usu.usuarioId).FirstOrDefault();
                _ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                _sistemaId = sessaoCorrente.sistemaId;
                _usuarioId = sessaoCorrente.usuarioId;
                _empresaId = sessaoCorrente.empresaId;
                _ip = sessaoCorrente.ip;
            }


            int _transacaoId = (from t in seguranca_db.Transacaos where t.url.ToLower() == url.ToLower() && t.sistemaId == _sistemaId select t.transacaoId).FirstOrDefault();

            logAuditoria = new LogAuditoria()
            {
                dt_log = DateTime.Now,
                empresaId = _empresaId,
                usuarioId = _usuarioId,
                ip = _ip,
                transacaoId = _transacaoId,
                notacao = notacao
            };
            this.seguranca_db.Set<LogAuditoria>().Add(logAuditoria);

            #endregion
        }
        #endregion

        #region getObject
        /// <summary>
        /// Recebe o repository com as chaves primárias
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna uma instância do objeto repository a partir da chave primária</returns>
        public R getObject(R key)
        {
            using (db = getContextInstance())
            {
                key.empresaId = new EmpresaSecurity<App_DominioContext>().getSessaoCorrente().empresaId;

                E entity = Find(key);

                if (entity != null)
                {
                    R value = MapToRepository(entity);
                    return value;
                }
                else
                    return null;
            }
        }
        #endregion

        #region Search
        public IQueryable<E> Search(Expression<Func<E, bool>> where)
        {
            IQueryable<E> entities = null;

            using (db = getContextInstance())
            {
                entities = this.db.Set<E>().Where(where).AsQueryable();
            }

            return entities;
        }

        public IQueryable<E> Search(Expression<Func<E, bool>> where, DbContext db)
        {
            IQueryable<E> entities = null;

            entities = this.db.Set<E>().Where(where).AsQueryable();

            return entities;
        }
        #endregion

        #region Insert
        public R Insert(R value)
        {
            using (db = getContextInstance())
            {
                using (seguranca_db = new SecurityContext())
                {
                    try
                    {
                        System.Web.HttpContext web = System.Web.HttpContext.Current;
                        sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                        if (sessaoCorrente != null)
                            value.empresaId = sessaoCorrente.empresaId;

                        #region validar inclusão
                        value.mensagem = this.Validate(value, Crud.INCLUIR);
                        #endregion

                        #region insere o registro
                        if (value.mensagem.Code == 0)
                        {
                            string _url = value.uri;
                            #region Mapear repository para entity
                            E entity = MapToEntity(value);
                            #endregion

                            this.db.Set<E>().Add(entity);
                            db.SaveChanges();
                            value = MapToRepository(entity);

                            #region Log de Auditoria
                            SaveLog(entity, _url, "Inclusão");
                            seguranca_db.SaveChanges();
                            #endregion
                        }
                        #endregion

                    }
                    catch (ArgumentException ex)
                    {
                        value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                    }
                    catch (DbUpdateException ex)
                    {
                        value.mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                        if (value.mensagem.MessageBase.ToUpper().Contains("REFERENCE"))
                        {
                            value.mensagem.Code = 45;
                            value.mensagem.Message = MensagemPadrao.Message(28).ToString();
                            value.mensagem.MessageType = MsgType.ERROR;
                        }
                        else if (value.mensagem.MessageBase.ToUpper().Contains("PRIMARY"))
                        {
                            value.mensagem.Code = 37;
                            value.mensagem.Message = MensagemPadrao.Message(37).ToString();
                            value.mensagem.MessageType = MsgType.WARNING;
                        }
                        else if (value.mensagem.MessageBase.ToUpper().Contains("UNIQUE KEY"))
                        {
                            value.mensagem.Code = 54;
                            value.mensagem.Message = MensagemPadrao.Message(54).ToString();
                            value.mensagem.MessageType = MsgType.WARNING;
                        }
                        else
                        {
                            value.mensagem.Code = 44;
                            value.mensagem.Message = MensagemPadrao.Message(44).ToString();
                            value.mensagem.MessageType = MsgType.ERROR;
                        }

                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        value.mensagem = new Validate() { Code = 42, Message = MensagemPadrao.Message(42).ToString(), MessageBase = ex.EntityValidationErrors.Select(m => m.ValidationErrors.First().ErrorMessage).First() };
                    }
                    catch (Exception ex)
                    {
                        value.mensagem.Code = 17;
                        value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                        value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                        value.mensagem.MessageType = MsgType.ERROR;
                    }
                }

            }

            return value;

        }
        #endregion

        #region Update
        public R Update(R value)
        {
            using (db = getContextInstance())
            {
                using (seguranca_db = new SecurityContext())
                {
                    try
                    {
                        System.Web.HttpContext web = System.Web.HttpContext.Current;
                        sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                        if (sessaoCorrente != null)
                            value.empresaId = sessaoCorrente.empresaId;

                        #region validar alteração
                        value.mensagem = this.Validate(value, Crud.ALTERAR);
                        #endregion

                        #region altera o registro
                        if (value.mensagem.Code == 0)
                        {
                            string _url = value.uri;
                            #region Mapear repository para entity
                            E entity = MapToEntity(value);
                            #endregion

                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                            #region Log de Auditoria
                            SaveLog(entity, _url, "Alteração");
                            seguranca_db.SaveChanges();
                            #endregion
                        }
                        #endregion
                    }
                    catch (ArgumentException ex)
                    {
                        value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                    }
                    catch (DbUpdateException ex)
                    {
                        value.mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                        if (value.mensagem.MessageBase.ToUpper().Contains("REFERENCE"))
                        {
                            value.mensagem.Code = 28;
                            value.mensagem.Message = MensagemPadrao.Message(28).ToString();
                            value.mensagem.MessageType = MsgType.ERROR;
                        }
                        else if (value.mensagem.MessageBase.ToUpper().Contains("PRIMARY"))
                        {
                            value.mensagem.Code = 37;
                            value.mensagem.Message = MensagemPadrao.Message(37).ToString();
                            value.mensagem.MessageType = MsgType.WARNING;
                        }
                        else
                        {
                            value.mensagem.Code = 43;
                            value.mensagem.Message = MensagemPadrao.Message(42).ToString();
                            value.mensagem.MessageType = MsgType.ERROR;
                        }

                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        value.mensagem = new Validate() { Code = 43, Message = MensagemPadrao.Message(43).ToString(), MessageBase = ex.EntityValidationErrors.Select(m => m.ValidationErrors.First().ErrorMessage).First() };
                    }
                    catch (Exception ex)
                    {
                        value.mensagem.Code = 17;
                        value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                        value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                        value.mensagem.MessageType = MsgType.ERROR;
                    }

                }
            }

            return value;
        }
        #endregion

        #region Delete
        public R Delete(R value)
        {
            using (db = getContextInstance())
            {
                using (seguranca_db = new SecurityContext())
                {
                    try
                    {
                        System.Web.HttpContext web = System.Web.HttpContext.Current;
                        sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                        value.empresaId = sessaoCorrente.empresaId;

                        #region validar exclusão
                        value.mensagem = this.Validate(value, Crud.EXCLUIR);
                        #endregion

                        #region excluir o registro
                        if (value.mensagem.Code == 0)
                        {
                            string _url = value.uri;

                            E entity = this.Find(value);
                            if (entity == null)
                                throw new ArgumentException("Objeto não identificado para exclusão");
                            this.db.Set<E>().Remove(entity);
                            db.SaveChanges();

                            #region Log de Auditoria
                            SaveLog(entity, _url, "Exclusão");
                            seguranca_db.SaveChanges();
                            #endregion
                        }
                        #endregion
                    }
                    catch (ArgumentException ex)
                    {
                        value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                    }
                    catch (DbUpdateException ex)
                    {
                        value.mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                        if (value.mensagem.MessageBase.ToUpper().Contains("REFERENCE"))
                        {
                            value.mensagem.Code = 16;
                            value.mensagem.Message = MensagemPadrao.Message(16).ToString();
                        }
                        else
                        {
                            value.mensagem.Code = 42;
                            value.mensagem.Message = MensagemPadrao.Message(42).ToString();
                        }
                        value.mensagem.MessageType = MsgType.ERROR;
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        value.mensagem = new Validate() { Code = 44, Message = MensagemPadrao.Message(44).ToString(), MessageBase = ex.EntityValidationErrors.Select(m => m.ValidationErrors.First().ErrorMessage).First() };
                    }
                    catch (Exception ex)
                    {
                        value.mensagem.Code = 17;
                        value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                        value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                        value.mensagem.MessageType = MsgType.ERROR;
                    }
                }
            }

            return value;
        }
        #endregion

        #region Save
        public R Save(R value, Expression<Func<E, bool>> where)
        {
            using (db = getContextInstance())
            {
                try
                {
                    value.empresaId = new EmpresaSecurity<App_DominioContext>().getSessaoCorrente().empresaId;
                    Crud op = Crud.INCLUIR;

                    if (Search(where, db) != null)
                        op = Crud.ALTERAR;

                    #region validar alteração
                    value.mensagem = this.Validate(value, op);
                    #endregion

                    #region altera o registro
                    if (value.mensagem.Code == 0)
                    {
                        #region Mapear repository para entity
                        E entity = MapToEntity(value);
                        #endregion

                        if (op == Crud.ALTERAR)
                            db.Entry(entity).State = EntityState.Modified;
                        else
                            this.db.Set<E>().Add(entity);
                        db.SaveChanges();
                        value = MapToRepository(entity);
                    }
                    #endregion
                }
                catch (ArgumentException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (DbUpdateException ex)
                {
                    value.mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                    if (value.mensagem.MessageBase.ToUpper().Contains("REFERENCE"))
                    {
                        value.mensagem.Code = 28;
                        value.mensagem.Message = MensagemPadrao.Message(28).ToString();
                        value.mensagem.MessageType = MsgType.ERROR;
                    }
                    else if (value.mensagem.MessageBase.ToUpper().Contains("PRIMARY"))
                    {
                        value.mensagem.Code = 37;
                        value.mensagem.Message = MensagemPadrao.Message(37).ToString();
                        value.mensagem.MessageType = MsgType.WARNING;
                    }
                    else
                    {
                        value.mensagem.Code = 43;
                        value.mensagem.Message = MensagemPadrao.Message(42).ToString();
                        value.mensagem.MessageType = MsgType.ERROR;
                    }

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    value.mensagem = new Validate() { Code = 43, Message = MensagemPadrao.Message(43).ToString(), MessageBase = ex.EntityValidationErrors.Select(m => m.ValidationErrors.First().ErrorMessage).First() };
                }
                catch (Exception ex)
                {
                    value.mensagem.Code = 17;
                    value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                    value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                    value.mensagem.MessageType = MsgType.ERROR;
                }
            }

            return value;
        }
        #endregion

        #region Save
        public Validate SaveCollection(IEnumerable<R> values, Expression<Func<E, bool>> where)
        {
            Validate mensagem = new Validate();

            using (db = getContextInstance())
            {
                using (seguranca_db = new SecurityContext())
                {
                    try
                    {
                        // exclui todo mundo para incluir novamente
                        IEnumerable<E> entities = db.Set<E>().Where(where);

                        foreach (E entity in db.Set<E>().Where(where))
                        {
                            this.db.Set<E>().Remove(entity);

                            #region Log de Auditoria
                            SaveLog(entity, values.FirstOrDefault().uri, "Exclusão");
                            #endregion
                        }

                        db.SaveChanges();
                        seguranca_db.SaveChanges();

                        // Inclui novamente
                        foreach (R value in values)
                        {
                            value.empresaId = new EmpresaSecurity<App_DominioContext>().getSessaoCorrente().empresaId;
                            Crud op = Crud.INCLUIR;

                            if (Find(value) != null)
                                op = Crud.ALTERAR;

                            #region validar alteração
                            value.mensagem = this.Validate(value, op);
                            #endregion

                            #region inclui/altera o registro
                            if (value.mensagem.Code == 0)
                            {
                                #region Mapear repository para entity
                                E entity = MapToEntity(value);
                                #endregion

                                if (op == Crud.ALTERAR)
                                    db.Entry(entity).State = EntityState.Modified;
                                else
                                    this.db.Set<E>().Add(entity);

                                #region Log de Auditoria
                                SaveLog(entity, value.uri, "Inclusão");
                                #endregion
                            }
                            #endregion
                        }

                        db.SaveChanges();
                        seguranca_db.SaveChanges();

                        mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

                    }
                    catch (ArgumentException ex)
                    {
                        return new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                    }
                    catch (DbUpdateException ex)
                    {

                        mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                        if (mensagem.MessageBase.ToUpper().Contains("REFERENCE"))
                        {
                            mensagem.Code = 28;
                            mensagem.Message = MensagemPadrao.Message(28).ToString();
                            mensagem.MessageType = MsgType.ERROR;
                        }
                        else if (mensagem.MessageBase.ToUpper().Contains("PRIMARY"))
                        {
                            mensagem.Code = 37;
                            mensagem.Message = MensagemPadrao.Message(37).ToString();
                            mensagem.MessageType = MsgType.WARNING;
                        }
                        else
                        {
                            mensagem.Code = 43;
                            mensagem.Message = MensagemPadrao.Message(42).ToString();
                            mensagem.MessageType = MsgType.ERROR;
                        }

                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        return new Validate() { Code = 43, Message = MensagemPadrao.Message(43).ToString(), MessageBase = ex.EntityValidationErrors.Select(m => m.ValidationErrors.First().ErrorMessage).First() };
                    }
                    catch (Exception ex)
                    {
                        mensagem.Code = 17;
                        mensagem.Message = MensagemPadrao.Message(17).ToString();
                        mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                        mensagem.MessageType = MsgType.ERROR;
                    }
                }
            }

            return mensagem;
        }
        #endregion

        public IEnumerable<R> ListAll()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class CrudItem<R, D> : Context<D>, ICrudItemContext<R>
        where R : Repository
        where D : DbContext
    {
        private IList<R> ListItem { get; set; }

        public CrudItem()
        {
            ListItem = new List<R>();
        }

        public CrudItem(IList<R> list)
        {
            SetListItem(list);
        }

        public void SetListItem(IList<R> list)
        {
            ListItem = list;
        }

        #region Métodos virtuais
        public abstract R Find(R key);

        public abstract int Indexof(R key);

        public virtual R CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            Type typeInstance = typeof(R);
            R Instance = (R)Activator.CreateInstance(typeInstance);

            return Instance;
        }

        public abstract R SetKey(R r);

        public abstract Validate Validate(R value, Crud operation);

        public abstract void AfterDelete();
        #endregion

        #region getObject
        public R getObject(R key)
        {
            return Find(key);
        }
        #endregion

        #region Search
        public IList<R> Search(Func<R, bool> where)
        {
            IList<R> repositories = ListItem.Where(where).ToList();
            return repositories.ToList();
        }
        #endregion

        #region Insert
        public R Insert(R value)
        {
            try
            {
                #region validar inclusão
                value.mensagem = this.Validate(value, Crud.INCLUIR);
                #endregion

                #region insere o item
                if (value.mensagem.Code == 0)
                    this.ListItem.Add(value);
                #endregion
            }
            catch (ArgumentException ex)
            {
                value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
            }
            catch (Exception ex)
            {
                value.mensagem.Code = 17;
                value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                value.mensagem.MessageType = MsgType.ERROR;
            }

            return value;

        }
        #endregion

        #region Update
        public R Update(R value)
        {
            try
            {
                #region validar alteração
                value.mensagem = this.Validate(value, Crud.ALTERAR);
                #endregion

                #region altera o registro
                if (value.mensagem.Code == 0)
                    ListItem[this.Indexof(value)] = value;
                #endregion
            }
            catch (ArgumentException ex)
            {
                value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
            }
            catch (Exception ex)
            {
                value.mensagem.Code = 17;
                value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                value.mensagem.MessageType = MsgType.ERROR;
            }

            return value;
        }
        #endregion

        #region Delete
        public R Delete(R value)
        {
            try
            {
                #region validar exclusão
                value.mensagem = this.Validate(value, Crud.EXCLUIR);
                #endregion

                #region excluir o exercicio
                if (value.mensagem.Code == 0)
                {
                    ListItem.RemoveAt(this.Indexof(value));
                    AfterDelete();
                }
                #endregion
            }
            catch (ArgumentException ex)
            {
                value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
            }
            catch (Exception ex)
            {
                value.mensagem.Code = 17;
                value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                value.mensagem.MessageType = MsgType.ERROR;
            }

            return value;
        }
        #endregion

        public IEnumerable<R> ListAll()
        {
            return this.ListItem;
        }
    }

    public abstract class ProcessContext<E, R, D> : CrudContext<E, R, D>, IProcessContext<R>
        where E : class
        where R : Repository
        where D : DbContext
    {
        public abstract E ExecProcess(R value, Crud operation = Crud.INCLUIR);

        #region Save All
        public R SaveAll(R value, Crud operation)
        {
            using (db = getContextInstance())
            {
                using (seguranca_db = new SecurityContext())
                {
                    try
                    {
                        System.Web.HttpContext web = System.Web.HttpContext.Current;
                        sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                        if (sessaoCorrente != null)
                            value.empresaId = sessaoCorrente.empresaId;

                        #region validar processamento
                        value.mensagem = this.Validate(value, operation);
                        #endregion

                        #region processar os registros
                        if (value.mensagem.Code == 0)
                        {
                            string _url = value.uri;
                            E entity = ExecProcess(value, operation);
                            db.SaveChanges();
                            seguranca_db.SaveChanges();
                            if (sessaoCorrente != null)
                                value.empresaId = sessaoCorrente.empresaId;
                            value = MapToRepository(entity);
                            value.uri = _url;
                            // só deverá ser implementado se não for executar operações na conexão atual.
                            // caso contrário deverá ser feito dentro do método ExecProcess
                            if (operation == Crud.INCLUIR)
                                value.mensagem = AfterInsert(value);
                            else if (operation == Crud.ALTERAR)
                                value.mensagem = AfterUpdate(value);
                            else
                                value.mensagem = AfterDelete(value);

                            if (value.mensagem.Code > 0)
                                throw new DbUpdateException(value.mensagem.MessageBase);

                            #region Log de Auditoria
                            SaveLog(entity, _url);
                            #endregion
                        }
                        #endregion
                    }
                    catch (App_DominioException ex)
                    {
                        value.mensagem = ex.Result;
                    }
                    catch (ArgumentException ex)
                    {
                        value.mensagem = new Validate() { Code = 999, Message = MensagemPadrao.Message(999).ToString(), MessageBase = ex.Message };
                    }
                    catch (DbUpdateException ex)
                    {
                        value.mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                        if (value.mensagem.MessageBase.ToUpper().Contains("REFERENCE"))
                        {
                            value.mensagem.Code = 45;
                            value.mensagem.Message = MensagemPadrao.Message(28).ToString();
                            value.mensagem.MessageType = MsgType.ERROR;
                        }
                        else if (value.mensagem.MessageBase.ToUpper().Contains("PRIMARY"))
                        {
                            value.mensagem.Code = 37;
                            value.mensagem.Message = MensagemPadrao.Message(37).ToString();
                            value.mensagem.MessageType = MsgType.WARNING;
                        }
                        else
                        {
                            value.mensagem.Code = 44;
                            value.mensagem.Message = MensagemPadrao.Message(44).ToString();
                            value.mensagem.MessageType = MsgType.ERROR;
                        }

                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        value.mensagem = new Validate() { Code = 42, Message = MensagemPadrao.Message(42).ToString(), MessageBase = ex.EntityValidationErrors.Select(m => m.ValidationErrors.First().ErrorMessage).First() };
                    }
                    catch (Exception ex)
                    {
                        value.mensagem.Code = 17;
                        value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                        value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                        value.mensagem.MessageType = MsgType.ERROR;
                    }
                }
            }

            return value;

        }
        #endregion

        public virtual Validate AfterInsert(R value)
        {
            return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
        }
        public virtual Validate AfterUpdate(R value)
        {
            return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
        }
        public virtual Validate AfterDelete(R value)
        {
            return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
        }
    }


    public abstract class ExecContext<R, D> : Context<D>, IExecContext<R>
        where R : Repository
        where D : DbContext
    {
        public abstract R ExecProcess(R value, Crud operation);

        #region Métodos da interface
        #region Run
        public R Run(R value, Crud operation)
        {
            using (db = getContextInstance())
            {
                using (seguranca_db = new SecurityContext())
                {
                    try
                    {
                        System.Web.HttpContext web = System.Web.HttpContext.Current;
                        sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                        if (sessaoCorrente != null)
                            value.empresaId = sessaoCorrente.empresaId;

                        #region validar processamento
                        value.mensagem = this.Validate(value, operation);
                        #endregion

                        #region processamento
                        if (value.mensagem.Code == 0)
                        {
                            string _url = value.uri;
                            value = ExecProcess(value, operation);
                            value.uri = _url;
                            if (value.mensagem.Code > 0)
                                throw new DbUpdateException(value.mensagem.MessageBase);

                            // só deverá ser implementado se não for executar operações na conexão atual.
                            // caso contrário deverá ser feito dentro do método ExecProcess
                            value.mensagem = AfterRun(value, operation);
                            if (value.mensagem.Code > 0)
                                throw new DbUpdateException(value.mensagem.MessageBase);
                        }
                        #endregion
                    }
                    catch (ArgumentException ex)
                    {
                        value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                    }
                    catch (DbUpdateException ex)
                    {
                        value.mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                        if (value.mensagem.MessageBase.ToUpper().Contains("REFERENCE"))
                        {
                            value.mensagem.Code = 45;
                            value.mensagem.Message = MensagemPadrao.Message(28).ToString();
                            value.mensagem.MessageType = MsgType.ERROR;
                        }
                        else if (value.mensagem.MessageBase.ToUpper().Contains("PRIMARY"))
                        {
                            value.mensagem.Code = 37;
                            value.mensagem.Message = MensagemPadrao.Message(37).ToString();
                            value.mensagem.MessageType = MsgType.WARNING;
                        }
                        else
                        {
                            value.mensagem.Code = 44;
                            value.mensagem.Message = MensagemPadrao.Message(44).ToString();
                            value.mensagem.MessageType = MsgType.ERROR;
                        }

                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        value.mensagem = new Validate() { Code = 42, Message = MensagemPadrao.Message(42).ToString(), MessageBase = ex.EntityValidationErrors.Select(m => m.ValidationErrors.First().ErrorMessage).First() };
                    }
                    catch (Exception ex)
                    {
                        value.mensagem.Code = 17;
                        value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                        value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                        value.mensagem.MessageType = MsgType.ERROR;
                    }
                }
            }

            return value;

        }
        #endregion

        public virtual Validate Validate(R value, App_Dominio.Enumeracoes.Crud operation)
        {
            return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
        }

        public virtual Validate AfterRun(R value, Crud operation)
        {
            return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };
        }

        public virtual R CreateRepository(System.Web.HttpRequestBase Request = null)
        {
            Type typeInstance = typeof(R);
            R Instance = (R)Activator.CreateInstance(typeInstance);

            return Instance;
        }
        #endregion
    }
}