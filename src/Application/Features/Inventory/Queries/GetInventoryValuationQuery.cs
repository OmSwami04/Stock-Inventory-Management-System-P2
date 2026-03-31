using MediatR;
using InventoryManagement.Application.Features.Inventory.DTOs;
using InventoryManagement.Interfaces.Factories;

namespace InventoryManagement.Application.Features.Inventory.Queries;

public record GetInventoryValuationQuery(ValuationMethod Method) : IRequest<InventoryValuationDto>;
