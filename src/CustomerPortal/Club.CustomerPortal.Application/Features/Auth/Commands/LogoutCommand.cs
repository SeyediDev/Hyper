namespace Hyper.CustomerPortal.Application.Features.Auth.Commands;

public record LogoutCommand : IRequest;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    public Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // TODO: Invalidate refresh token if needed
        return Task.CompletedTask;
    }
}

