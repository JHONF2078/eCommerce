using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsService.DataAccessLayer.RepositoryContracts
{
    public interface IEntity<TKey>
    {
        TKey GetId();
        void SetId(TKey id);

        string GetTableName();
    }
}
