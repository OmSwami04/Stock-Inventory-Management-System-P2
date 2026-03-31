using MediatR;

namespace InventoryManagement.Application.Features.Auth.Commands;

public record LoginCommand(
    string Username,
    string Password
) : IRequest<string>;
