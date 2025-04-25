using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using ProductsService.BusinessLogicLayer.DTO;
using ProductsService.BusinessLogicLayer.RabbitMQ;
using ProductsService.BusinessLogicLayer.ServiceContracts;
using ProductsService.DataAccessLayer.Entities;
using ProductsService.DataAccessLayer.RepositoryContracts;
using System.Linq.Expressions;

namespace ProductsService.BusinessLogicLayer.Services
{
    public class ProductService : IProductService
    {        
        private readonly IValidator<ProductAddRequest> _genericRequestAddValidator;
        private readonly IValidator<ProductUpdateRequest> _genericRequestUpdateValidator;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Product, Guid> _genericRepository;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;

        public ProductService(
            IValidator<ProductAddRequest> genericRequestAddValidator,
            IValidator<ProductUpdateRequest> genericRequestUpdateValidator,
            IMapper mapper,
            IGenericRepository<Product, Guid> genericRepository,
            IRabbitMQPublisher rabbitMQPublisher
            )
        {
            _genericRequestAddValidator = genericRequestAddValidator;
            _genericRequestUpdateValidator = genericRequestUpdateValidator;
            _mapper = mapper;
            _genericRepository = genericRepository;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public async Task<ProductResponse?> AddAsync(ProductAddRequest productUpdateRequest)
        {
            if (productUpdateRequest == null) {
                throw new ArgumentNullException(nameof(productUpdateRequest), "product request cannot be null");
            }

            //Vlidate the product using Fluent Validation
            ValidationResult validationResult = await _genericRequestAddValidator.ValidateAsync(productUpdateRequest);

            //chech the validation result
            if (!validationResult.IsValid)
            {
                // this return Error1, Error2
                string errors =  string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            //attempt to add product
            Product productInput = _mapper.Map<Product>(productUpdateRequest);

            Product? productAdded =  await _genericRepository.AddAsync(productInput);

            if (productAdded == null)
            {
                return default;
            }

            ProductResponse productAddedRResponse = _mapper.Map<ProductResponse>(productAdded);

            return productAddedRResponse;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // 2.  Buscar la entidad (FindAsync → GetByIdAsync)
            Product? existingProduct = await _genericRepository.GetByIdAsync(id);
            if (existingProduct is null)
                throw new ArgumentException("Invalid product ID");

            //attempt to delete product
            bool isDeleted = await _genericRepository.DeleteAsync(id);

            //TO DO: Add code for posting a message to the message queue that announces the consumers about the deleted product details

            //Publish message of product.delete
            if (isDeleted)
            {
                ProductDeletionMessage message = new ProductDeletionMessage(existingProduct.Id, existingProduct.ProductName);
                string routingKey = "product.delete";

                var  headers = new Dictionary<string, object>()
                {
                    { "event", "product.delete" },
                    { "RowCount", 1 }
                };

                _rabbitMQPublisher.Publish(headers, message);
            }

            return isDeleted;
        }

        public async Task<List<ProductResponse?>> GetAllAsync()
        {
            IEnumerable<Product>? entities = await _genericRepository.GetAllAsync();
            //for example invokes ProductToProducProductResponseMappingProfile
            IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(entities);

            return productResponses.ToList();
        }

        public async Task<List<ProductResponse?>> GetAllByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            IEnumerable<Product?> entities = await _genericRepository.GetAllByConditionsAsync(conditionExpression);
            //Invokes ProductToProducProductResponseMappingProfile
            IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(entities); 
            return productResponses.ToList();
        }

        public async Task<ProductResponse?> GetSingleByCondition(Expression<Func<Product, bool>> conditionExpresion)
        {
           Product? product = await _genericRepository.GetSingleByConditionAsync(conditionExpresion);

            if(product == null)
            {
                return default;
            }

            //for example invokes ProductToProducProductResponseMappingProfile
            ProductResponse productResponse = _mapper.Map<ProductResponse>(product);

            return productResponse;

        }

        public async Task<ProductResponse> UpdateAsync(ProductUpdateRequest productUpdateRequest)
        {
            // 1.  Obtener el ID con el tipo correcto
            Guid id = productUpdateRequest.Id;

            //Verificar que la entidad exista (usa la caché con FindAsync)
            Product? existingProduct = await _genericRepository.GetByIdAsync(id);

            if (existingProduct == null)
            {
                throw new ArgumentException("Invalid Product ID");
            }

            //Validate the product using Fluent Validation
            ValidationResult validationResult = await _genericRequestUpdateValidator.ValidateAsync(productUpdateRequest);

            // Check the validation result
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage)); //Error1, Error2, ...
                throw new ArgumentException(errors);
            }

            //usa la sobrecarga Map<TDestination>(source), que:
            //1.Crea una instancia nueva de Product.
            // 2.Copia las propiedades del DTO en esa instancia.
            //Si luego intentas adjuntar ese objeto al DbContext mientras ya existe otra instancia con la
            //misma PK(la que recuperaste con GetByIdAsync), EF Core lanza la excepción de tracking duplicado.
            //la otra opcion seria no verificar que existe antes de mapear, es decir no hacer nada de existingProduct
            //Product product = _mapper.Map<Product>(productUpdateRequest);


            // Check if product name is changed
            //se hace esta linea antes de maperar ya que si al mapear se copian los datos de productUpdateRequest
            //a existingProduct por lo tanto  productUpdateRequest.ProductName simpre seria a
            //existingProduct.ProductName  por lo tanto ProductName siempre seria igual en los dos
            // bool isProductNameChanged = productUpdateRequest.ProductName != existingProduct.ProductName;


            //Map from ProductUpdateRequest to Product product
            //Invokes ProductUpdateRequestToProductMappingProfile
            // ← NO se crea una entidad nueva           
            Product product =  _mapper.Map(productUpdateRequest, existingProduct);                      

            Product? updatedProduct = await _genericRepository.UpdateAsync(product);

            //Publish product.update.name message to the exchange
                       
            //string routingKey = "product.update.name";
            //var message = new ProductNameUpdateMessage(product.Id, product.ProductName);

            var headers = new Dictionary<string, object>()
            {
                { "event", "product.update" },              
                { "RowCount", 1 }
            };

            ProductDTO productDTO = _mapper.Map<ProductDTO>(product);

            _rabbitMQPublisher.Publish<ProductDTO>(headers, productDTO);                          

            ProductResponse? updatedproductResponse = _mapper.Map<ProductResponse>(updatedProduct);

            return updatedproductResponse;
        }
    }
}
