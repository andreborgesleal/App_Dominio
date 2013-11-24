using App_Dominio.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Dominio.Contratos
{
    public interface IMasterRepository<R> where R : Repository
    {
        void CreateItem();
        IEnumerable<R> GetItems();
        R GetItem();
        void SetItems(IEnumerable<R> value);
        void SetItem(R value);        
    }
}
