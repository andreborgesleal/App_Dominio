using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using App_Dominio.Enumeracoes;
using App_Dominio.Repositories;
using App_Dominio.Security;
using App_Dominio.Negocio;


namespace App_Dominio.Negocio
{
    public class AlterarSenhaModel : UsuarioModel //CrudContext<Usuario, AlterarSenhaRepository, SecurityContext>
    {
        #region Métodos da classe CrudContext
       
        public override Validate Validate(UsuarioRepository value, Crud operation)
        {
            value.mensagem = base.Validate(value, operation);

            if (value.mensagem.Code == 0)
            {
                #region verifica se a senha atual está correta
                EmpresaSecurity<SecurityContext> security = new EmpresaSecurity<SecurityContext>();

                sessaoCorrente = security.getSessaoCorrente();

                value.mensagem = security.Autenticar(sessaoCorrente.login, ((AlterarSenhaRepository)value).senhaAtual, sessaoCorrente.sistemaId);
                #endregion
            }

            return value.mensagem;
        }

        public override UsuarioRepository CreateRepository(System.Web.HttpRequestBase Request)
        {
            return new AlterarSenhaRepository();
        }
        #endregion
    }
}