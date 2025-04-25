using ProductsService.DataAccessLayer.EntitiesContracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsService.DataAccessLayer.Entities
{
    public class Product : IEntity<Guid>
    {  
        [Key] // Esto le dice a EF Core que esta es la clave primaria
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public double? UnitPrice { get; set; }
        public int? QuantityInStock { get; set; }
    }
}
