using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Repositories;
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
        private Validate _Autenticar(string usuario, string senha, int sistemaId)
        {
            Validate validate = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
            try
            {
                #region Recupera o usuário
                senha = Criptografar(senha);
                Usuario usu = (from u in seguranca_db.Usuarios join usg in seguranca_db.UsuarioGrupos on u equals usg.Usuario
                               join g in seguranca_db.Grupos on usg.Grupo equals g
                               where u.login == usuario && u.senha == senha && u.situacao == "A" && g.sistemaId == sistemaId
                               select u).FirstOrDefault();
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
        public Validate Autenticar(string usuario, string senha, int sistemaId)
        {
            using (seguranca_db = new SecurityContext())
                return _Autenticar(usuario, senha, sistemaId);
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
                    validate = _Autenticar(usuario, senha, sistemaId);
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

                        seguranca_db.Entry(sessao).State = System.Data.Entity.EntityState.Modified;
                    }
                    seguranca_db.SaveChanges();
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

        #region Atualizar Sessão
        public Validate _AtualizarSessao(string sessionId)
        {
            Validate validate = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
            try
            {
                if (_ValidarSessao(sessionId))
                {
                    #region Atualiza a sessão
                    sessaoCorrente = seguranca_db.Sessaos.Find(sessionId);
                    sessaoCorrente.dt_atualizacao = DateTime.Now;
                    seguranca_db.Entry(sessaoCorrente).State = System.Data.Entity.EntityState.Modified;
                    seguranca_db.SaveChanges();
                    #endregion
                }
                else
                {
                    validate.Code = 200;
                    validate.Message = MensagemPadrao.Message(200).ToString();
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
        public Validate AtualizarSessao(string sessionId)
        {
            using (seguranca_db = new SecurityContext())
                return _AtualizarSessao(sessionId);
        }
        #endregion

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
                        seguranca_db.Entry(sessaoCorrente).State = System.Data.Entity.EntityState.Modified;
                        seguranca_db.SaveChanges();
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                App_DominioException.saveError(ex, GetType().FullName);
            }
        }

        #region getSessaoCorrente
        public Sessao _getSessaoCorrente()
        {
            System.Web.HttpContext web = System.Web.HttpContext.Current;
            if (_ValidarSessao(web.Session.SessionID))
                return seguranca_db.Sessaos.Find(web.Session.SessionID);
            else
                return null;
        }
        public Sessao getSessaoCorrente()
        {
            using (seguranca_db = new SecurityContext())
                return _getSessaoCorrente();
        }
        #endregion

        #region Retorna a empresa do usuário da sessão corrente
        public Empresa getEmpresa()
        {
            using (seguranca_db = seguranca_db ?? new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                if (_ValidarSessao(web.Session.SessionID))
                {
                    sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);
                    return seguranca_db.Empresas.Find(this.sessaoCorrente.empresaId);
                }
                else
                    return null;
            }
        }
        #endregion

        #region retorna o Usuário da sessão corrente
        public Usuario getUsuario()
        {
            using (seguranca_db = seguranca_db ?? new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                if (_ValidarSessao(web.Session.SessionID))
                {
                    sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);
                    return seguranca_db.Usuarios.Find(this.sessaoCorrente.usuarioId);
                }
                else
                    return null;
            }
        }
        #endregion

        #region Retorna os Alertas não lidos do usuário da sessão corrente
        public IEnumerable<Alerta> getAlertasNaoLidos()
        {
            using (seguranca_db = seguranca_db ?? new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                if (ValidarSessao(web.Session.SessionID))
                {
                    sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                    IEnumerable<Alerta> q = from a in seguranca_db.Alertas
                                            where a.usuarioId == sessaoCorrente.usuarioId
                                                    && a.dt_leitura == null
                                            select a;
                    return q;
                }
                else
                    return null;
            }
        }
        #endregion

        #region Retorna os grupos de um dado usuário
        public IEnumerable<Grupo> getUsuarioGrupo(decimal? usuarioId = null)
        {
            using (seguranca_db = seguranca_db ?? new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                if (ValidarSessao(web.Session.SessionID))
                {
                    if (!usuarioId.HasValue)
                    {
                        sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);
                        usuarioId = sessaoCorrente.usuarioId;
                    }

                    IEnumerable<Grupo> q = from a in seguranca_db.UsuarioGrupos
                                           where a.usuarioId == usuarioId && a.situacao == "A"
                                           select a.Grupo;
                    return q;
                }
                else
                    return null;
            }
        }
        #endregion

        #region Retorna as transações de um dado usuário
        public IEnumerable<TransacaoRepository> getUsuarioTransacao(decimal? usuarioId = null)
        {
            using (seguranca_db = new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                if (_ValidarSessao(web.Session.SessionID))
                {
                    sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                    if (!usuarioId.HasValue)
                        usuarioId = sessaoCorrente.usuarioId;

                    try
                    {
                        IEnumerable<TransacaoRepository> t = (from a in seguranca_db.UsuarioGrupos
                                                              join b in seguranca_db.GrupoTransacaos on a.grupoId equals b.grupoId
                                                              join c in seguranca_db.Transacaos on b.transacaoId equals c.transacaoId
                                                              join d in seguranca_db.Grupos on a.grupoId equals d.grupoId
                                                              where a.usuarioId == usuarioId && a.situacao == "A" && b.situacao == "A" && d.sistemaId == sessaoCorrente.sistemaId
                                                              orderby c.transacaoId_pai, c.posicao
                                                              select new TransacaoRepository()
                                                              {
                                                                  transacaoId = c.transacaoId,
                                                                  transacaoId_pai = c.transacaoId_pai,
                                                                  sistemaId = c.sistemaId,
                                                                  nomeCurto = c.nomeCurto,
                                                                  nome = c.nome,
                                                                  descricao = c.descricao,
                                                                  referencia = c.referencia,
                                                                  exibir = c.exibir,
                                                                  posicao = c.posicao,
                                                                  url = c.url,
                                                                  glyph = c.glyph
                                                              }).Distinct();
                        return t.ToList();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        throw new App_DominioException(ex.Message, GetType().FullName);
                    }
                    catch (Exception ex)
                    {
                        throw new App_DominioException(ex.Message, GetType().FullName);
                    }
                }
                else
                    return null;
            }
        }
        #endregion

        #region Retorna os Grupos que podem acessar uma determinada transação
        public string getGruposByTransacao(string url)
        {
            using (seguranca_db = new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                if (_ValidarSessao(web.Session.SessionID))
                {
                    sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                    try
                    {
                        var x = (from a in seguranca_db.GrupoTransacaos
                                 join b in seguranca_db.Transacaos on a.Transacao equals b
                                 join c in seguranca_db.Grupos on a.Grupo equals c
                                 where b.url == url && b.sistemaId == sessaoCorrente.sistemaId && c.empresaId == sessaoCorrente.empresaId
                                 select c.descricao).ToList().ToArray();
                        
                        var resultado = "";

                        if (x.Count() > 0)
                            resultado = x[0];

                        for (int i = 1; i <= x.Count() - 1; i++)
                        {
                            resultado += ", " + x[i];
                        }

                        return resultado;
                    }
                    catch (DbEntityValidationException ex)
                    {
                        throw new App_DominioException(ex.Message, GetType().FullName);
                    }
                    catch (Exception ex)
                    {
                        throw new App_DominioException(ex.Message, GetType().FullName);
                    }
                }
                else
                    return "";
            }
        }

        public string[] getGruposByCurrentUser()
        {
            using (seguranca_db = new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                if (_ValidarSessao(web.Session.SessionID))
                {
                    sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                    try
                    {
                        return (from a in seguranca_db.UsuarioGrupos
                                join b in seguranca_db.Grupos on a.Grupo equals b
                                join c in seguranca_db.Usuarios on a.Usuario equals c
                                where c.login == sessaoCorrente.login && b.sistemaId == sessaoCorrente.sistemaId && c.empresaId == sessaoCorrente.empresaId && b.empresaId == sessaoCorrente.empresaId
                                select b.descricao).ToList().ToArray();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        throw new App_DominioException(ex.Message, GetType().FullName);
                    }
                    catch (Exception ex)
                    {
                        throw new App_DominioException(ex.Message, GetType().FullName);
                    }
                }
                else
                    return new string[] { };
            }
        }
        #endregion

        #region Retorna se o usuário corrente está autorizado a acessar uma dada URL
        public bool AccessDenied(string url)
        {
            using (seguranca_db = new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                if (_ValidarSessao(web.Session.SessionID))
                {
                    sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                    try
                    {
                        if (_AtualizarSessao(web.Session.SessionID).Code > 0)
                            return true;

                        var x = (from gtr in seguranca_db.GrupoTransacaos
                                 join tra in seguranca_db.Transacaos on gtr.Transacao equals tra
                                 join gru in seguranca_db.Grupos on gtr.Grupo equals gru
                                 join ugr in seguranca_db.UsuarioGrupos on gru equals ugr.Grupo
                                 join usu in seguranca_db.Usuarios on ugr.Usuario equals usu
                                 where tra.url == url && tra.sistemaId == sessaoCorrente.sistemaId && gru.empresaId == sessaoCorrente.empresaId && usu.usuarioId == sessaoCorrente.usuarioId
                                 select usu).ToList();

                        return x == null;
                    }
                    catch (DbEntityValidationException ex)
                    {
                        throw new App_DominioException(ex.Message, GetType().FullName);
                    }
                    catch (Exception ex)
                    {
                        throw new App_DominioException(ex.Message, GetType().FullName);
                    }
                }
                else
                    return true;
            }

        }

        #endregion

    }
}