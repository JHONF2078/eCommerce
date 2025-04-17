using ProductsService.BusinessLogicLayer.DTO;
using ProductsService.BusinessLogicLayer.ServiceContracts;
using ProductsService.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServiceContracts
{
    public interface IProductsService : IGenericService<Product, ProductResponse, ProductAddRequest, ProductUpdateRequest>
    {
        // implementar métodos específicos más adelante
    }
}
