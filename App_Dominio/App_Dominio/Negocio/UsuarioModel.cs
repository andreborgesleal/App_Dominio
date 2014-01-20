using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using App_Dominio.Enumeracoes;
using App_Dominio.Repositories;
using App_Dominio.Security;

namespace App_Dominio.Negocio
{
    public class UsuarioModel : CrudContext<Usuario, UsuarioRepository, SecurityContext>
    {
        #region Métodos da classe CrudContext
        public override Usuario MapToEntity(UsuarioRepository value)
        {
            EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();

            return new Usuario()
            {
                usuarioId = value.usuarioId,
                empresaId = value.empresaId,
                login = value.login,
                nome = value.nome,
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

            if (value.nome.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "Nome").ToString();
                value.mensagem.MessageBase = "Campo Nome deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }
            else if ((value.usuarioId == null || value.usuarioId == 0) && operation != Crud.INCLUIR)
            {
                value.mensagem.Code = 5;
                value.mensagem.Message = MensagemPadrao.Message(5, "ID do usuário").ToString();
                value.mensagem.MessageBase = "Campo ID do Usuário deve ser informado";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            return value.mensagem;
        }

        public override UsuarioRepository CreateRepository()
        {
            return new UsuarioRepository();
        }
        #endregion
    }

    public class ListViewUsuario : ListViewRepository<UsuarioRepository, SecurityContext>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<UsuarioRepository> Bind(int? index, int pageSize = 50, params object[] param)
        {
            EmpresaSecurity<App_DominioContext> security = new EmpresaSecurity<App_DominioContext>();
            string _descricao = param != null && param.Count() > 0 && param[0] != null ? param[0].ToString() : null;
            int _sistemaId = param != null && param.Count() > 1 && param[1] != null ? (int)param[1] : 0;
            int _empresaId = security.getSessaoCorrente().empresaId;
            return (from usu in db.Usuarios join ugr in db.UsuarioGrupos on usu equals ugr.Usuario
                    join gru in db.Grupos on ugr.Grupo equals gru
                    where (_descricao == null || String.IsNullOrEmpty(_descricao) || usu.nome.StartsWith(_descricao.Trim())) 
                            && usu.empresaId == _empresaId
                            && gru.sistemaId == _sistemaId
                            && gru.situacao == "A"
                            && ugr.situacao == "A"
                            && usu.situacao == "A"
                    orderby usu.nome 
                    select new UsuarioRepository
                    {
                        usuarioId = usu.usuarioId,
                        nome = usu.nome,
                        login = usu.login,
                        empresaId = usu.empresaId,
                        dt_cadastro = usu.dt_cadastro,
                        situacao = usu.situacao,
                        isAdmin = usu.isAdmin,
                        senha = usu.senha,
                        nome_grupo = gru.descricao,
                        keyword = usu.keyword,
                        dt_keyword = usu.dt_keyword,
                        PageSize = pageSize,
                        TotalCount = (from usu1 in db.Usuarios
                                      join ugr1 in db.UsuarioGrupos on usu1 equals ugr1.Usuario
                                      join gru1 in db.Grupos on ugr1.Grupo equals gru1
                                      where (_descricao == null || String.IsNullOrEmpty(_descricao) || usu1.nome.StartsWith(_descricao.Trim()))
                                              && usu1.empresaId == _empresaId
                                              && gru1.sistemaId == _sistemaId
                                              && gru1.situacao == "A"
                                              && ugr1.situacao == "A"
                                              && usu1.situacao == "A"
                                      select usu1).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new UsuarioModel().getObject((UsuarioRepository)id);
        }
        #endregion
    }

}
