using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using ProductsService.BusinessLogicLayer.ServiceContracts;
using ProductsService.DataAccessLayer.Entities;
using ProductsService.DataAccessLayer.RepositoryContracts;
using System;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;

namespace ProductsService.BusinessLogicLayer.Services
{
    public class GenericService<TEntity, TId, TResponse, TRequestAdd, TRequestUpdate> : IGenericService<TEntity, TId, TResponse, TRequestAdd, TRequestUpdate>
          where TEntity : BaseEntity<TId>
          where TRequestUpdate : IRequestWithId<TId>
    {        
        private readonly IValidator<TRequestAdd> _genericRequestAddValidator;
        private readonly IValidator<TRequestUpdate> _genericRequestUpdateValidator;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<TEntity, TId> _genericRepository;

        public GenericService(
            IValidator<TRequestAdd> genericRequestAddValidator,
            IValidator<TRequestUpdate> genericRequestUpdateValidator,
            IMapper mapper,
            IGenericRepository<TEntity, TId> genericRepository
            )
        {
            _genericRequestAddValidator = genericRequestAddValidator;
            _genericRequestUpdateValidator = genericRequestUpdateValidator;
            _mapper = mapper;
            _genericRepository = genericRepository;
        }

        public async Task<TResponse?> AddAsync(TRequestAdd entityRequest)
        {
            if (entityRequest == null) {
                throw new ArgumentNullException(nameof(entityRequest), "Entity request cannot be null");
            }

            //Vlidate the product using Fluent Validation
            ValidationResult validationResult = await _genericRequestAddValidator.ValidateAsync(entityRequest);

            //chech the validation result
            if (!validationResult.IsValid)
            {
                // this return Error1, Error2
                string errors =  string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            //attempt to add entity
            TEntity entityInput = _mapper.Map<TEntity>(entityRequest);

            TEntity? entityAdded =  await _genericRepository.AddAsync(entityInput);

            if (entityAdded == null)
            {
                return default;
            }

            TResponse entityAddedRResponse = _mapper.Map<TResponse>(entityAdded);

            return entityAddedRResponse;
        }

        public async Task<bool> DeleteAsync(TId id)
        {
            // 2.  Buscar la entidad (FindAsync → GetByIdAsync)
            TEntity? existing = await _genericRepository.GetByIdAsync(id);
            if (existing is null)
                throw new ArgumentException("Invalid entity ID");

            //attempt to delete entity
            bool isDeleted = await _genericRepository.DeleteAsync(id);

            return isDeleted;
        }

        public async Task<List<TResponse?>> GetAllAsync()
        {
            IEnumerable<TEntity>? entities = await _genericRepository.GetAllAsync();
            //for example invokes ProductToProductResponseMappingProfile
            IEnumerable<TResponse?> entityResponses = _mapper.Map<IEnumerable<TResponse>>(entities);

            return entityResponses.ToList();
        }

        public async Task<List<TResponse?>> GetAllByCondition(Expression<Func<TEntity, bool>> conditionExpression)
        {
            IEnumerable<TEntity?> entities = await _genericRepository.GetAllByConditionsAsync(conditionExpression);
            //Invokes ProductToProductResponseMappingProfile
            IEnumerable<TResponse?> entityResponses = _mapper.Map<IEnumerable<TResponse>>(entities); 
            return entityResponses.ToList();
        }

        public async Task<TResponse?> GetSingleByCondition(Expression<Func<TEntity, bool>> conditionExpresion)
        {
           TEntity? entity = await _genericRepository.GetSingleByConditionAsync(conditionExpresion);

            if(entity == null)
            {
                return default;
            }

            //for example invokes ProductToProductResponseMappingProfile
            TResponse entityResponse = _mapper.Map<TResponse>(entity);

            return entityResponse;

        }

        public async Task<TResponse> UpdateAsync(TRequestUpdate entityRequest)
        {
            // 1.  Obtener el ID con el tipo correcto
            TId id = entityRequest.Id;

            //Verificar que la entidad exista (usa la caché con FindAsync)
            TEntity? existingEntity = await _genericRepository.GetByIdAsync(id);

            if (existingEntity == null)
            {
                throw new ArgumentException("Invalid Product ID");
            }

            //Validate the product using Fluent Validation
            ValidationResult validationResult = await _genericRequestUpdateValidator.ValidateAsync(entityRequest);

            // Check the validation result
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage)); //Error1, Error2, ...
                throw new ArgumentException(errors);
            }


            //usa la sobrecarga Map<TDestination>(source), que:
            //1.Crea una instancia nueva de TEntity.
            // 2.Copia las propiedades del DTO en esa instancia.
            //Si luego intentas adjuntar ese objeto al DbContext mientras ya existe otra instancia con la
            //misma PK(la que recuperaste con GetByIdAsync), EF Core lanza la excepción de tracking duplicado.
            //la otra opcion seria no verificar que existe antes de mapear, es decir no hacer nada de existingEntity
            //TEntity entity = _mapper.Map<TEntity>(entityRequest);


            //Map from ProductUpdateRequest to Product entity
            //Invokes ProductUpdateRequestToProductMappingProfile
            // ← NO se crea una entidad nueva
            TEntity entity =  _mapper.Map(entityRequest, existingEntity); 

            TEntity? updatedProduct = await _genericRepository.UpdateAsync(entity);

            TResponse? updatedEntityResponse = _mapper.Map<TResponse>(updatedProduct);

            return updatedEntityResponse;
        }
    }
}
