using Hyper.Application.Features.Channels;
using Hyper.Domain.Features.Rewards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hyper.EventHandler.Application.Features.Channel;

internal sealed class HandleChannelConsumeAssetMessage(
    IRewardAssetService rewardAssetService,
    ILogger<HandleChannelConsumeAssetMessage> logger
) : IRequestHandler<ChannelConsumeAssetMessage>
{
    public async Task Handle(ChannelConsumeAssetMessage request, CancellationToken cancellationToken)
    {
        ConsumeAccetResponse response = await rewardAssetService.ConsumeReward(new()
        {
            Customer = request.CustomerMobile,
            Serial = request.Serial
        }, cancellationToken);

        logger.LogInformation("Asset consumed for channel {ChannelKey} serial {Serial}", request.ChannelId, request.Serial);
    }
}

