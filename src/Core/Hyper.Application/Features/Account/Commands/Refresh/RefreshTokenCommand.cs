using Neo.Application.Exceptions;
using Neo.Domain.Features.Client.Dto;

namespace Hyper.Application.Features.Account.Commands.Refresh;
public class RefreshTokenCommand : IRequest<RefreshTokenCommandResponse>
{
    public required string RefreshToken { get; set; }
}


public record RefreshTokenCommandResponse(TokenResponseDto? Token)
{
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
                  .NotEmpty().WithMessage("الزامی می باشد.");
    }
}

public class RefreshTokenCommandHandler(IIdpService idpService)
        : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandResponse>
{
    public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await idpService.GetUserTokenByRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (result is null)
        {
            throw new ForbiddenAccessException();
        }
        return new RefreshTokenCommandResponse(result);
    }
}
