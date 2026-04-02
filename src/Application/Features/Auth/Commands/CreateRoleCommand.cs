using MediatR;

namespace InventoryManagement.Application.Features.Auth.Commands;

public record CreateRoleCommand(
    string RoleName
) : IRequest;
