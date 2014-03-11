using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using App_Dominio.Enumeracoes;
using App_Dominio.Security;
using App_Dominio.Repositories;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Net.Mail;

namespace App_Dominio.Negocio
{
    public class EsqueciMinhaSenhaModel : ProcessContext<Usuario, UsuarioRepository, SecurityContext>
    {
        private string newPassword { get; set; }
        
        public  EsqueciMinhaSenhaModel() : base()
        {
        }

        public EsqueciMinhaSenhaModel(SecurityContext _db)
        {
            this.db = _db;
        }

        #region Métodos da classe CrudContext
        public override Usuario ExecProcess(UsuarioRepository value, Crud operation)
        {
            Usuario entity = MapToEntity(value);
            db.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public override Validate AfterInsert(UsuarioRepository value)
        {
            Validate result = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            Empresa empresa = db.Empresas.Find(value.empresaId);
            Sistema sistema = db.Sistemas.Find(int.Parse(System.Configuration.ConfigurationManager.AppSettings["sistemaId"]));

            #region Enviar e-mail ao usuário
            SendEmail sendMail = new SendEmail();

            MailAddress sender = new MailAddress(empresa.nome + " <" + empresa.email + ">");
            List<string> recipients = new List<string>();
            recipients.Add(value.nome + "<" + value.login + ">");
            string Subject = "Esqueci minha senha";
            string Text = "<p>Encaminhamento de nova senha</p>";
            string Html = "<p><span style=\"font-family: Verdana; font-size: larger; color: #656464\">Conta do " + sistema.descricao + "</span></p>" +
                          "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #0094ff\">Nova Senha</span></p>" +
                          "<p></p>" +
                          "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Aqui está a sua nova senha: <b>" + newPassword + "</b></span></p>" +
                          "<p></p>" +
                          "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Esta é a sua nova senha gerada automaticamente pelo sistema. Se preferir, faça seu acesso e escolha a opção <b>Alterar Senha</b> no seu perfil de usuário para escolher uma nova senha.</span></p>" +
                          "<p></p>" +
                          "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Obrigado,</span></p>" +
                          "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Suporte " + empresa.nome + "</span></p>";

            result = sendMail.Send(sender, recipients, Html, Subject, Text);
            #endregion

            return result;
        }

        public override Usuario MapToEntity(UsuarioRepository value)
        {
            Prepare(ref value);

            EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();

            return new Usuario()
            {
                usuarioId = value.usuarioId,
                empresaId = value.empresaId,
                login = value.login,
                nome = value.nome.ToUpper(),
                dt_cadastro = DateTime.Now,
                situacao = value.situacao,
                isAdmin = value.isAdmin,
                senha = security.Criptografar(value.senha),
                keyword = value.keyword,
                dt_keyword = value.dt_keyword
            };
        }

        public override UsuarioRepository MapToRepository(Usuario entity)
        {
            return new UsuarioRepository()
            {
                usuarioId = entity.usuarioId,
                empresaId = entity.empresaId,
                login = entity.login,
                nome = entity.nome,
                dt_cadastro = entity.dt_cadastro,
                situacao = entity.situacao,
                isAdmin = entity.isAdmin,
                keyword = entity.keyword,
                dt_keyword = entity.dt_keyword,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Usuario Find(UsuarioRepository key)
        {
            return db.Usuarios.Find(key.usuarioId);
        }

        public override Validate Validate(UsuarioRepository value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            #region Validar código de validação e data de expiração do código

            UsuarioRepository usu = (from k in seguranca_db.Usuarios.AsEnumerable() where k.usuarioId == value.usuarioId select new UsuarioRepository() { keyword = k.keyword, dt_keyword = k.dt_keyword }).FirstOrDefault();
            
            if (value.keyword != usu.keyword)
            {
                value.mensagem.Code = 4;
                value.mensagem.Message = MensagemPadrao.Message(4, "Código de validação", "Valor incorreto").ToString();
                value.mensagem.MessageBase = "Código de validação informado nao corresponde ao código de validação esperado.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            else if (usu.dt_keyword < DateTime.Now)
            {
                value.mensagem.Code = 50;
                value.mensagem.Message = MensagemPadrao.Message(50).ToString();
                value.mensagem.MessageBase = "Código de validação já expirou. Favor solicitar novo código e repetir a operação.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            #endregion

            return value.mensagem;
        }
        #endregion

        #region Métodos Customizados
        public virtual void Prepare(ref UsuarioRepository value)
        {
            value.senha = new Random().Next(0, 99999999).ToString();
            newPassword = value.senha;
        }
        #endregion
    }

    public class CodigoSegurancaModel : EsqueciMinhaSenhaModel
    {
        private string newKeyword { get; set; }

        #region Métodos da classe CrudContext
        public override Validate AfterInsert(UsuarioRepository value)
        {
            Validate result = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            Empresa empresa = db.Empresas.Find(value.empresaId);
            Sistema sistema = db.Sistemas.Find(int.Parse(System.Configuration.ConfigurationManager.AppSettings["sistemaId"]));

            #region Enviar e-mail ao usuário
            SendEmail sendMail = new SendEmail();

            MailAddress sender = new MailAddress(empresa.nome + " <" + empresa.email + ">");
            List<string> recipients = new List<string>();
            recipients.Add(value.nome + "<" + value.login + ">");
            string Subject = "Código de Segurança da conta do " + sistema.descricao;
            string Text = "<p>Encaminhamento de código de segurança</p>";
            string Html = "<p><span style=\"font-family: Verdana; font-size: larger; color: #656464\">Conta do " + sistema.descricao + "</span></p>" +
                          "<p><span style=\"font-family: Verdana; font-size: xx-large; color: #0094ff\">Código de Segurança</span></p>" +
                          "<p></p>" +
                          "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Aqui está o seu código: <b>" + newKeyword + "</b></span></p>" +
                          "<p></p>" +
                          "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Esse é um código de validação, não uma senha. Se você não solicitou esse código, outra pessoa pode saber sua senha e ter acesso à sua conta.</span></p>" +
                          "<p></p>" +
                          "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Obrigado,</span></p>" +
                          "<p><span style=\"font-family: Verdana; font-size: small; color: #000\">Suporte " + empresa.nome + "</span></p>";

            result = sendMail.Send(sender, recipients, Html, Subject, Text);
            #endregion

            return result;
        }

        public override Validate Validate(UsuarioRepository value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            return value.mensagem;
        }
        #endregion

        #region Métodos Customizados
        public override void Prepare(ref UsuarioRepository value)
        {
            value.keyword = new Random().Next(0, 99999999).ToString();
            value.dt_keyword = DateTime.Now.AddHours(2);

            newKeyword = value.keyword;
        }
        #endregion

    }

}


