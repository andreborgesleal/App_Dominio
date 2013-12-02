using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Dominio.Repositories
{
    public class TransacaoRepository
    {
        public int transacaoId { get; set; }
        public int sistemaId { get; set; }
        public Nullable<int> transacaoId_pai { get; set; }
        public string nomeCurto { get; set; }
        public string nome { get; set; }
        public string descricao { get; set; }
        public string referencia { get; set; }
        public string exibir { get; set; }
        public Nullable<int> posicao { get; set; }
        public string url { get; set; }
        public string glyph { get; set; }
    }
}
