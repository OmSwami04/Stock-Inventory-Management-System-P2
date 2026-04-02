using MediatR;

namespace InventoryManagement.Application.Features.Auth.Commands;

public record RegisterCommand(
    string Username,
    string Password,
    string RoleName
) : IRequest;
