using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProductsService.DataAccessLayer.EntitiesContracts
{
    public interface IEntity<TId>
    {
        TId Id { get;}
    }
}
