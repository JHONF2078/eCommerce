using AutoMapper;
using DataAccessLayer.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Org.BouncyCastle.Asn1.Cms;
using ProductsService.BusinessLogicLayer.DTO;
using ProductsService.BusinessLogicLayer.ServiceContracts;
using ProductsService.BusinessLogicLayer.Validators;
using ProductsService.DataAccessLayer.Entities;
using ProductsService.DataAccessLayer.Repositories;
using ProductsService.DataAccessLayer.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductsService.BusinessLogicLayer.Services
{
    public class GenericService<TEntity, TResponse, TRequestAdd, TRequestUpdate> : IGenericService<TEntity, TResponse, TRequestAdd, TRequestUpdate>
          where TEntity : class
          where TRequestUpdate : class, IRequestWithId
    {        
        private readonly IValidator<TRequestAdd> _genericRequestAddValidator;
        private readonly IValidator<TRequestUpdate> _genericRequestUpdateValidator;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<TEntity> _genericRepository;

        public GenericService(
            IValidator<TRequestAdd> genericRequestAddValidator,
            IValidator<TRequestUpdate> genericRequestUpdateValidator,
            IMapper mapper,
            IGenericRepository<TEntity> genericRepository
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

        public async Task<bool> DeleteAsync(Guid id)
        {
           TEntity? existingEntity = await _genericRepository.GetSingleByConditionAsync(x => x.Equals(id));

            if(existingEntity == null)
            {
                return false;
            }

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
            TEntity? existingProduct = await _genericRepository.GetByIdAsync(entityRequest.Id);

            if (existingProduct == null)
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


            //Map from ProductUpdateRequest to Product type
            TEntity product = _mapper.Map<TEntity>(entityRequest); //Invokes ProductUpdateRequestToProductMappingProfile

            TEntity? updatedProduct = await _genericRepository.UpdateAsync(product);

            TResponse? updatedEntityResponse = _mapper.Map<TResponse>(updatedProduct);

            return updatedEntityResponse;
        }
    }
}
