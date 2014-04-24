using App_Dominio.Component;
using App_Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Repositories
{
    public class UsuarioRepository : Repository
    {
        [DisplayName("ID")]
        public int usuarioId { get; set; }

        [Required(ErrorMessage="Informe o login de acesso")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Informe um e-mail válido")]
        [EmailAddress]
        [Display(Name = "Login")]
        public string login { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "Informe o nome do usuário")]
        [StringLength(40, ErrorMessage="O nome do usuário deve ter no máximo 40 caracteres" )]
        public string nome { get; set; }

        public DateTime dt_cadastro { get; set; }

        [Required(ErrorMessage = "Informe a Situação do cadastro")]
        [DisplayName("Situação")]
        public string situacao { get; set; }

        [Required(ErrorMessage = "Informe se o usuário é Administrador ou não")]
        [DisplayName("Administrador (S/N")]
        public string isAdmin { get; set; }

        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage="A senha deve possuir no mínimo 6 dígitos e no máximo 20 dígitos", MinimumLength = 6)]
        public string senha { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirmar Senha")]
        [Compare("senha", ErrorMessage = "As senhas não conferem.")]
        public string confirmacaoSenha { get; set; }

        public virtual string keyword { get; set; }

        public Nullable<DateTime> dt_keyword { get; set; }

        public string nome_grupo { get; set; }

        public string nome_sistema { get; set; }

    }

    public class AlterarSenhaRepository : UsuarioRepository
    {
        [DisplayName("Senha Atual")]
        [Required(ErrorMessage="Senha atual deve ser informada")]
        public string senhaAtual { get; set; }
    }

    public class EsqueciMinhaSenhaRepository : Repository
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Informe um e-mail válido")]
        [EmailAddress]
        [Display(Name = "Login")]
        public string login { get; set; }

        [Required]
        [DisplayName("Código de validação")]
        public string keyword { get; set; }
    }
}
