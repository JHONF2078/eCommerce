using FluentValidation;
using FluentValidation.Results;
using ProductsService.BusinessLogicLayer.DTO;
using ProductsService.BusinessLogicLayer.ServiceContracts;
using ProductsService.DataAccessLayer.Entities;

namespace ProductsService.API.APIEndpoints;

public static class ProductAPIEndpoints
{
  public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
  {
    //GET /api/products
    app.MapGet("/api/products", async (IGenericService< Product, Guid, ProductResponse, ProductAddRequest, ProductUpdateRequest > productsService) =>
    {
      List<ProductResponse?> products = await productsService.GetAllAsync();
      return Results.Ok(products);
    });


        //GET /api/products/search/product-id/00000000-0000-0000-0000-000000000000
    app.MapGet("/api/products/search/product-id/{ProductID:guid}", async (IGenericService < Product, Guid, ProductResponse, ProductAddRequest, ProductUpdateRequest >  productsService, Guid ProductID) =>
    {
      ProductResponse? product = await productsService.GetSingleByCondition(temp => temp.Id == ProductID);
        if (product == null)
            return Results.NotFound();

        return Results.Ok(product);
    });


    //GET /api/products/search/xxxxxxxxxxxxxxxxxx
    app.MapGet("/api/products/search/{SearchString}", async (IGenericService<Product, Guid, ProductResponse, ProductAddRequest, ProductUpdateRequest> productsService, string SearchString) =>
    {
      List<ProductResponse?> productsByProductName = await productsService.GetAllByCondition(temp => temp.ProductName != null && temp.ProductName.Contains(SearchString, StringComparison.OrdinalIgnoreCase));

      List<ProductResponse?> productsByCategory = await productsService.GetAllByCondition(temp => temp.Category != null && temp.Category.Contains(SearchString, StringComparison.OrdinalIgnoreCase));

      var products = productsByProductName.Union(productsByCategory);

      return Results.Ok(products);
    });


    //POST /api/products
    app.MapPost("/api/products", async (IGenericService<Product, Guid, ProductResponse, ProductAddRequest, ProductUpdateRequest> productsService, IValidator<ProductAddRequest> productAddRequestValidator, ProductAddRequest productAddRequest) =>
    {
      //Validate the ProductAddRequest object using Fluent Validation
      ValidationResult validationResult = await productAddRequestValidator.ValidateAsync(productAddRequest);

      //Check the validation result
      if (!validationResult.IsValid)
      {
        Dictionary<string, string[]> errors = validationResult.Errors
          .GroupBy(temp => temp.PropertyName)
          .ToDictionary(grp => grp.Key,
            grp => grp.Select(err => err.ErrorMessage).ToArray());
        return Results.ValidationProblem(errors);
      }


      var addedProductResponse = await productsService.AddAsync(productAddRequest);
      if (addedProductResponse != null)
        return Results.Created($"/api/products/search/product-id/{addedProductResponse.ProductID}", addedProductResponse);
      else
        return Results.Problem("Error in adding product");
    });


    //PUT /api/products
    app.MapPut("/api/products", async (IGenericService<Product, Guid, ProductResponse, ProductAddRequest, ProductUpdateRequest> productsService, IValidator<ProductUpdateRequest> productUpdateRequestValidator, ProductUpdateRequest productUpdateRequest) =>
    {
      //Validate the ProductUpdateRequest object using Fluent Validation
      ValidationResult validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

      //Check the validation result
      if (!validationResult.IsValid)
      {
        Dictionary<string, string[]> errors = validationResult.Errors
          .GroupBy(temp => temp.PropertyName)
          .ToDictionary(grp => grp.Key,
            grp => grp.Select(err => err.ErrorMessage).ToArray());
        return Results.ValidationProblem(errors);
      }


      var updatedProductResponse = await productsService.UpdateAsync(productUpdateRequest);
      if (updatedProductResponse != null)
        return Results.Ok(updatedProductResponse);
      else
        return Results.Problem("Error in updating product");
    });


    //DELETE /api/products/xxxxxxxxxxxxxxxxxxx
    app.MapDelete("/api/products/{ProductID:guid}", async (IGenericService<Product, Guid, ProductResponse, ProductAddRequest, ProductUpdateRequest> productsService, Guid ProductID) =>
    {
      bool isDeleted = await productsService.DeleteAsync(ProductID);
      if (isDeleted)
        return Results.Ok(true);
      else
        return Results.Problem("Error in deleting product");
    });
    return app;
  }
}
