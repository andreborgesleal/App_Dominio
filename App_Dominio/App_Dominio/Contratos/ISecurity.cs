using App_Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Dominio.Contratos
{
    public interface ISecurity
    {
        string Criptografar(string value);
        Validate Autenticar(string login, string senha);
        Validate Autorizar(string usuario, string senha, int sistemaId, params object[] param);
        bool ValidarSessao(string sessionId);
        Validate AtualizarSessao(string sessionId);
        void EncerrarSessao(string sessionId);
        Sessao getSessaoCorrente();
    }
}
