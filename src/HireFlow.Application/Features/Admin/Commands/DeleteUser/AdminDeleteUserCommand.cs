using MediatR;

namespace HireFlow.Application.Features.Admin.Commands.DeleteUser;

public record AdminDeleteUserCommand(long UserId) : IRequest;
