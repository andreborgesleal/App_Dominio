using App_Dominio.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Dominio.Contratos
{
    public interface IReportRepository<R> where R : Repository
    {
        object getValueColumn1();
        object getValueColumn2();

        void ClearColumn1();
        void ClearColumn2();

        R getKey(object group = null, object subGroup = null);

        R Create(R key, IEnumerable<R> list) ;

    }
}
