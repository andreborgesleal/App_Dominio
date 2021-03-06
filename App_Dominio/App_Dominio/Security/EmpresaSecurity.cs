﻿using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Models;
using App_Dominio.Negocio;
using App_Dominio.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace App_Dominio.Security
{
    public class EmpresaSecurity<D> : Context<D>, ISecurity where D : DbContext
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
                Usuario usu = (from u in seguranca_db.Usuarios join usg in seguranca_db.UsuarioGrupos on u.usuarioId equals usg.usuarioId
                               join g in seguranca_db.Grupos on usg.grupoId equals g.grupoId
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

                    System.Web.HttpContext web = System.Web.HttpContext.Current;

                    #region Validar Sessão
                    Sessao s = seguranca_db.Sessaos.Find(web.Session.SessionID);
                    if (s != null && s.dt_desativacao == null && s.dt_atualizacao.AddMinutes(Timeout) >= Funcoes.Brasilia() && s.usuarioId != usu.usuarioId)
                    {
                        validate.Code = 201;
                        validate.Message = MensagemPadrao.Message(201).text;
                        validate.MessageBase = "Sessão já está em uso. Tente novamente mais tarde ou contate o Administrador do sistema.";
                        return validate;                        
                    }
                    #endregion

                    #region insere a sessao
                    //Sessao s1 = seguranca_db.Sessaos.Find(web.Session.SessionID);

                    if (s == null)
                    {
                        Sessao sessao = new Sessao()
                        {
                            sessaoId = web.Session.SessionID,
                            sistemaId = sistemaId,
                            usuarioId = usu.usuarioId,
                            empresaId = usu.empresaId,
                            login = usu.login,
                            dt_criacao = Funcoes.Brasilia(),
                            dt_atualizacao = Funcoes.Brasilia(),
                            isOnline = "S",
                            ip = web.Request.UserHostAddress,
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
                        sessao.dt_atualizacao = Funcoes.Brasilia();
                        sessao.isOnline = "S";
                        sessao.ip = web.Request.UserHostAddress;
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
                if (s == null || s.dt_desativacao != null || s.dt_atualizacao.AddMinutes(Timeout) < Funcoes.Brasilia())
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
                    sessaoCorrente.dt_atualizacao = Funcoes.Brasilia();
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

        #region Alterar Sessão
        public Validate AlterarSessao(params object[] param)
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
                    System.Web.HttpContext web = System.Web.HttpContext.Current;

                    #region Validar Sessão
                    Sessao s = seguranca_db.Sessaos.Find(web.Session.SessionID);
                    if (s == null || s.dt_desativacao != null || s.dt_atualizacao.AddMinutes(Timeout) < Funcoes.Brasilia())
                    {
                        validate.Code = 201;
                        validate.Message = MensagemPadrao.Message(201).text;
                        validate.MessageBase = "Sessão Expirada.";
                        return validate;
                    }
                    #endregion

                    #region altera a sessao
                    Sessao sessao = seguranca_db.Sessaos.Find(web.Session.SessionID);

                    sessao.dt_desativacao = null;
                    sessao.dt_atualizacao = Funcoes.Brasilia();
                    sessao.value1 = value1;
                    sessao.value2 = value2;
                    sessao.value3 = value3;
                    sessao.value4 = value4;

                    seguranca_db.Entry(sessao).State = System.Data.Entity.EntityState.Modified;

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
                        sessaoCorrente.dt_atualizacao = Funcoes.Brasilia();
                        sessaoCorrente.dt_desativacao = Funcoes.Brasilia();
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
        public Sessao _getSessaoCorrente(SecurityContext seguranca_db)
        {
            this.seguranca_db = seguranca_db;
            return _getSessaoCorrente();
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
            using (seguranca_db = new SecurityContext())
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
        /// <summary>
        /// retorna o Usuário da sessão corrente
        /// </summary>
        /// <returns></returns>
        public Usuario getUsuario()
        {
            using (seguranca_db = new SecurityContext())
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

        #region retorna o Usuário a partir de um ID
        public Usuario getUsuarioById(int usuarioId)
        {
            using (seguranca_db = new SecurityContext())
                return seguranca_db.Usuarios.Find(usuarioId);
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
                                                    && a.sistemaId == sessaoCorrente.sistemaId 
                                                    && a.dt_leitura == null
                                            select a;
                    return q;
                }
                else
                    return null;
            }
        }
        #endregion

        #region Retorna os últimos 6 Alertas do usuário da sessão corrente
        public IEnumerable<AlertaRepository> getLast6Alertas()
        {
            using (seguranca_db = new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                if (_ValidarSessao(web.Session.SessionID))
                {
                    sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                    DateTime seisDias = DateTime.Today.AddDays(-6);

                    IEnumerable<AlertaRepository> q = (from a in seguranca_db.Alertas
                                                       where a.usuarioId == sessaoCorrente.usuarioId
                                                               && a.sistemaId == sessaoCorrente.sistemaId 
                                                               && a.dt_emissao >= seisDias
                                                       orderby a.dt_emissao descending
                                                       select new AlertaRepository()
                                                       {
                                                           alertaId = a.alertaId,
                                                           dt_emissao = a.dt_emissao,
                                                           dt_leitura = a.dt_leitura,
                                                           usuarioId = a.usuarioId,
                                                           sistemaId = a.sistemaId,
                                                           linkText = a.linkText,
                                                           url = a.url,
                                                           mensagemAlerta = a.mensagem
                                                       }
                                            ).Take(6).ToList(); // ultimos 6 alertas
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
                                                              where a.usuarioId == usuarioId && 
                                                                    a.situacao == "A" && 
                                                                    b.situacao == "A" && 
                                                                    d.sistemaId == sessaoCorrente.sistemaId && 
                                                                    d.empresaId == sessaoCorrente.empresaId
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
        public int AccessDenied(string url)
        {
            // valores de retorno:
            // 0-Acesso autorizado
            // 1-Redireciona para o login (usuário sem sessão ou sessão expirada)
            // 2-Exibir mensagem de erro (usuário tem sessão ativa, mas não tem autorização de usar a funcionalidade)

            using (seguranca_db = new SecurityContext())
            {
                System.Web.HttpContext web = System.Web.HttpContext.Current;
                if (_ValidarSessao(web.Session.SessionID))
                {
                    sessaoCorrente = seguranca_db.Sessaos.Find(web.Session.SessionID);

                    try
                    {
                        if (_AtualizarSessao(web.Session.SessionID).Code > 0)
                            return 1;

                        var x = (from gtr in seguranca_db.GrupoTransacaos
                                 join tra in seguranca_db.Transacaos on gtr.Transacao equals tra
                                 join gru in seguranca_db.Grupos on gtr.Grupo equals gru
                                 join ugr in seguranca_db.UsuarioGrupos on gru equals ugr.Grupo
                                 join usu in seguranca_db.Usuarios on ugr.Usuario equals usu
                                 where tra.url == url && tra.sistemaId == sessaoCorrente.sistemaId && gru.empresaId == sessaoCorrente.empresaId && ugr.usuarioId == sessaoCorrente.usuarioId && gtr.situacao == "A"
                                 select usu).ToList();
                        if (x == null || x.Count == 0)
                            return 2;
                        else
                            return 0;
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
                    return 1; // redireciona para o login
            }

        }

        #endregion

        #region Retorna os usuários de uma empresa e sistema 
        public IEnumerable<UsuarioRepository> getUsuarios(int sistemaId, int empresaId)
        {
            using (seguranca_db = new SecurityContext())
            {
                IEnumerable<UsuarioRepository> q = from usu in seguranca_db.Usuarios
                                                   join ugr in seguranca_db.UsuarioGrupos on usu equals ugr.Usuario
                                                   join gru in seguranca_db.Grupos on ugr.Grupo equals gru
                                                   where usu.empresaId == empresaId
                                                            && gru.sistemaId == sistemaId
                                                            && usu.situacao == "A"
                                                            && ugr.situacao == "A"
                                                            && gru.situacao == "A"
                                                   select new UsuarioRepository()
                                                   {
                                                       usuarioId = usu.usuarioId,
                                                       nome = usu.nome,
                                                       login = usu.login,
                                                       situacao = usu.situacao,
                                                       dt_cadastro = usu.dt_cadastro,
                                                       isAdmin = usu.isAdmin
                                                   };
                return q.ToList();
            }

        }

        #endregion

        #region Retorna um usuário pelo Login
        public UsuarioRepository getUsuarioByLogin(string login, int empresaId)
        {
            using (seguranca_db = new SecurityContext())
            {
                return (from usu in seguranca_db.Usuarios
                        where usu.login == login && usu.empresaId == empresaId
                        select new UsuarioRepository()
                        {
                            empresaId = usu.empresaId,
                            usuarioId = usu.usuarioId,
                            login = usu.login,
                            nome = usu.nome,
                            dt_cadastro = usu.dt_cadastro,
                            situacao = usu.situacao,
                            isAdmin = usu.isAdmin,
                            senha = usu.senha,
                            keyword = usu.keyword,
                            dt_keyword = usu.dt_keyword
                        }).FirstOrDefault();
            }
        }

        #endregion

        #region Incluir um usuário
        public UsuarioRepository SetUsuario(UsuarioRepository value)
        {
            UsuarioModel model = new UsuarioModel();
            return model.Insert(value);
        }

        #endregion

        #region Retorna o Log de Auditoria a partid do ID
        public LogAuditoriaRepository getLogAuditoriaById(int logId)
        {
            using (seguranca_db = new SecurityContext())
            {
                int empresaId = _getSessaoCorrente().empresaId;

                LogAuditoriaRepository log = (from l in seguranca_db.LogAuditorias
                                              join t in seguranca_db.Transacaos on l.transacaoId equals t.transacaoId
                                              join u in seguranca_db.Usuarios on l.usuarioId equals u.usuarioId
                                              where l.logId == logId && l.empresaId == empresaId
                                              select new LogAuditoriaRepository()
                                              {
                                                  logId = l.logId,
                                                  transacaoId = l.transacaoId,
                                                  nomeCurto = t.nomeCurto,
                                                  nome_funcionalidade = t.nome,
                                                  empresaId = l.empresaId,
                                                  usuarioId = l.usuarioId,
                                                  login = u.login,
                                                  nome_usuario = u.nome,
                                                  dt_log = l.dt_log,
                                                  ip = l.ip,
                                                  notacao = l.notacao
                                              }).FirstOrDefault();

                IList<SelectListItem> list = new List<SelectListItem>();

                XDocument documento = XDocument.Parse(log.notacao);

                string root = documento.Root.Name.LocalName;

                SelectListItem nomeCurto = new SelectListItem() { Text = "Funcionalidade", Value = log.nomeCurto };
                list.Add(nomeCurto);

                SelectListItem funcionalidade = new SelectListItem() { Text = "Descrição", Value = log.nome_funcionalidade };
                list.Add(funcionalidade);

                SelectListItem entity = new SelectListItem() { Text = "Entidade", Value = root };
                list.Add(entity);

                IEnumerable<XElement> element = from c in documento.Descendants(root) select c;

                foreach (XElement e in element.Elements())
                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = e.Attribute("name").Value,
                        Value = e.Attribute("value").Value
                    };

                    list.Add(item);
                }

                log.Notacaos = list.AsEnumerable().ToList();

                return log;
            }
        }

        #endregion

        #region Alerta
        public AlertaRepository InsertAlerta(AlertaRepository value)
        {
            try
            {
                value = new AlertaModel().Insert(value);
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

        public void ReadAlert(int alertaId)
        {
            AlertaModel model = new AlertaModel();
            AlertaRepository value = new AlertaRepository();

            try
            {
                value.alertaId = alertaId;
                value = model.getObject(value);

                value.dt_leitura = Funcoes.Brasilia();

                value = model.Update(value);
            }
            catch (Exception ex)
            {
                value.mensagem.Code = 17;
                value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                value.mensagem.MessageBase = new App_DominioException(ex.InnerException.InnerException.Message ?? ex.Message, GetType().FullName).Message;
                value.mensagem.MessageType = MsgType.ERROR;
            }
        }

        #endregion

    }
}