using ProductsService.BusinessLogicLayer.DTO;
using ProductsService.BusinessLogicLayer.ServiceContracts;
using ProductsService.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProductsService.BusinessLogicLayer.ServiceContracts
{
    public interface IProductsService : IGenericService<Product, Guid, ProductResponse, ProductAddRequest, ProductUpdateRequest>
    {
        // implementar métodos específicos más adelante
    }
}
