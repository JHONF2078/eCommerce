using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsService.DataAccessLayer.Entities
{    public class Product : BaseEntity<Guid>
    {
        //De esta forma EF Core solo mapea Id, pero otros componentes siguen usando ProductID
        [NotMapped]
        public Guid ProductID
        {
            get => Id;     // reenvía a Id
            set => Id = value;
        }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public double? UnitPrice { get; set; }
        public int? QuantityInStock { get; set; }
    }
}
