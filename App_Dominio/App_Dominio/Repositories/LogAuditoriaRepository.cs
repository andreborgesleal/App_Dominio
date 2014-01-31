using App_Dominio.Component;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Repositories
{
    public class LogAuditoriaRepository : Repository
    {
        [DisplayName("ID Log")]
        public decimal logId { get; set; }
        [DisplayName("ID Transação")]
        public int transacaoId { get; set; }
        [DisplayName("Funcionalidade")]
        public string nomeCurto { get; set; }
        [DisplayName("Descrição")]
        public string nome_funcionalidade { get; set; }
        [DisplayName("ID Empresa")]
        public int empresaId { get; set; }
        [DisplayName("Empresa")]
        public string nome_empresa { get; set; }
        [DisplayName("ID Usuário")]
        public int usuarioId { get; set; }
        [DisplayName("Login")]
        public string nome_usuario { get; set; }
        [DisplayName("Login")]
        public string login { get; set; }
        [DisplayName("Dt.Log")]
        public DateTime dt_log { get; set; }
        [DisplayName("IP")]
        public string ip { get; set; }
        [DisplayName("Notação")]
        public string notacao { get; set; }
    }
}
