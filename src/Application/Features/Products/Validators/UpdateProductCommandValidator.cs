using FluentValidation;
using InventoryManagement.Application.Features.Products.Commands;

namespace InventoryManagement.Application.Features.Products.Validators;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        
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
}
