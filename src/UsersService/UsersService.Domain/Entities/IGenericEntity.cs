using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Domain.Entities
{
    public interface IGenericEntity<TKey>
    {
        TKey GetId();
        void SetId(TKey id);

        string GetTableName();
    }
}
