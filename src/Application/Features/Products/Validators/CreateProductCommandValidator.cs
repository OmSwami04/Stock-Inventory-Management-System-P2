using FluentValidation;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Interfaces.Repositories;

namespace InventoryManagement.Application.Features.Products.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.")
            .MustAsync(BeUniqueSku).WithMessage("SKU must be unique.");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product Name is required.")
            .MaximumLength(200).WithMessage("Product Name must not exceed 200 characters.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0).WithMessage("Cost must be greater than or equal to 0.");

        RuleFor(x => x.ListPrice)
            .GreaterThanOrEqualTo(x => x.Cost).WithMessage("List Price must be greater than or equal to Cost.");
    }

    private async Task<bool> BeUniqueSku(string sku, CancellationToken cancellationToken)
    {
        return !await _productRepository.SkuExistsAsync(sku);
    }
}
