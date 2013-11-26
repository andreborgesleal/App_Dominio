using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Enumeracoes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace App_Dominio.Security
{
    public class EmpresaSecurity : Context, ISecurity
    {
        private int Timeout = 20;
        public string Criptografar(string value)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] hashBytes;
            using (HashAlgorithm hash = SHA1.Create())
                hashBytes = hash.ComputeHash(encoding.GetBytes(value));

            StringBuilder hashValue = new StringBuilder(hashBytes.Length * 2);
            foreach (byte b in hashBytes)
            {
                hashValue.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
            }

            return hashValue.ToString();
        }
        
        #region Autenticar
        private Validate _Autenticar(string usuario, string senha)
        {
            Validate validate = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
            try
            {
                #region Recupera o usuário
                senha = Criptografar(senha);
                Usuario usu = (from u in seguranca_db.Usuarios where u.login == usuario && u.senha == senha && u.situacao == "A" select u).First();
                #endregion

                #region autenticar usuário
                if (usu == null)
                {
                    validate.Code = 36;
                    validate.Message = MensagemPadrao.Message(36).ToString();
                    validate.MessageBase = MensagemPadrao.Message(999).ToString();
                }
                else
                    validate.Field = usu.usuarioId.ToString();
                #endregion
            }
            catch (DbEntityValidationException ex)
            {
                throw new App_DominioException(ex.Message, GetType().FullName);
            }
            catch (Exception ex)
            {
                throw new App_DominioException(ex.Message, GetType().FullName);
            }
            return validate;
        }
        public Validate Autenticar(string usuario, string senha)
        {
            using (seguranca_db = new SecurityContext())
                return _Autenticar(usuario, senha);
        }
        #endregion
        
        public Validate Autorizar(string usuario, string senha, int sistemaId, params object[] param)
        {
            Validate validate = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
            try
            {
                string value1 = param != null && param.Count() > 0 ? (string)param[0] : null;
                string value2 = param != null && param.Count() > 1 ? (string)param[1] : null;
                string value3 = param != null && param.Count() > 2 ? (string)param[2] : null;
                string value4 = param != null && param.Count() > 3 ? (string)param[3] : null;
                
                using (seguranca_db = new SecurityContext())
                {
                    #region Autenticar usuário
                    validate = _Autenticar(usuario, senha);
                    if (validate.Code > 0)
                        return validate;
                    Usuario usu = seguranca_db.Usuarios.Find(int.Parse(validate.Field));
                    #endregion

                    #region insere a sessao
                    System.Web.HttpContext web = System.Web.HttpContext.Current;
                    Sessao s1 = seguranca_db.Sessaos.Find(web.Session.SessionID);

                    if (s1 == null)
                    {
                        Sessao sessao = new Sessao()
                        {
                            sessaoId = web.Session.SessionID,
                            sistemaId = sistemaId,
                            usuarioId = usu.usuarioId,
                            empresaId = usu.empresaId,
                            login = usu.login,
                            dt_criacao= DateTime.Now,
                            dt_atualizacao = DateTime.Now,
                            isOnline = "S",
                            value1 = value1,
                            value2 = value2,
                            value3 = value3,
                            value4 = value4                            
                        };

                        seguranca_db.Sessaos.Add(sessao);
                    }
                    else
                    {
                        Sessao sessao = seguranca_db.Sessaos.Find(web.Session.SessionID);

                        sessao.sistemaId = sistemaId;
                        sessao.usuarioId = usu.usuarioId;
                        sessao.empresaId = usu.empresaId;
                        sessao.login = usu.login;
                        sessao.dt_desativacao = null;
                        sessao.dt_atualizacao = DateTime.Now;
                        sessao.isOnline = "S";
                        sessao.value1 = value1;
                        sessao.value2 = value2;
                        sessao.value3 = value3;
                        sessao.value4 = value4;

                        db.Entry(sessao).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                    validate.Field = web.Session.SessionID;
                    #endregion
                }            
                
            }
            catch (DbEntityValidationException ex)
            {
                throw new App_DominioException(ex.Message, GetType().FullName);
            }
            catch (Exception ex)
            {
                throw new App_DominioException(ex.Message, GetType().FullName);
            }
            return validate;            
        }

        #region Validar Sessão
        public bool _ValidarSessao(string sessionId)
        {
            try
            {
                #region Validar Sessão do usuário
                Sessao s = seguranca_db.Sessaos.Find(sessionId);
                if (s == null || s.dt_desativacao != null || s.dt_atualizacao.AddMinutes(Timeout) < DateTime.Now)
                    return false;
                #endregion
            }
            catch (Exception ex)
            {
                App_DominioException.saveError(ex, GetType().FullName);
                return false;
            }

            return true;
        }

        public bool ValidarSessao(string sessionId)
        {
            using (seguranca_db = new SecurityContext())
                return _ValidarSessao(sessionId);
        }
        #endregion

        public Validate AtualizarSessao(string sessionId)
        {
            Validate validate = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
            try
            {
                using (seguranca_db = new SecurityContext())
                {
                    if (_ValidarSessao(sessionId))
                    {
                        #region Atualiza a sessão
                        sessaoCorrente = seguranca_db.Sessaos.Find(sessionId);
                        sessaoCorrente.dt_atualizacao = DateTime.Now;
                        db.Entry(sessaoCorrente).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        #endregion
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                throw new App_DominioException(ex.Message, GetType().FullName);
            }
            catch (Exception ex)
            {
                throw new App_DominioException(ex.Message, GetType().FullName);
            }
            return validate;
        }

        public void EncerrarSessao(string sessionId)
        {
            try
            {
                using (seguranca_db = new SecurityContext())
                {
                    #region Desativa a sessão
                    sessaoCorrente = seguranca_db.Sessaos.Find(sessionId);
                    if (sessaoCorrente != null)
                    {
                        sessaoCorrente.dt_atualizacao = DateTime.Now;
                        sessaoCorrente.dt_desativacao = DateTime.Now;
                        db.Entry(sessaoCorrente).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                App_DominioException.saveError(ex, GetType().FullName);
            }
        }

        public Sessao getSessaoCorrente()
        {
            using (seguranca_db = new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);
                return this.sessaoCorrente;
            }
        }

    }
}