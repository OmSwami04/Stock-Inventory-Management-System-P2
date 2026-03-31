using FluentValidation;
using InventoryManagement.Application.Features.Stock.Commands;

namespace InventoryManagement.Application.Features.Stock.Validators;

public class CreateStockTransactionCommandValidator : AbstractValidator<CreateStockTransactionCommand>
{
    public CreateStockTransactionCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be positive.");
    }
}
