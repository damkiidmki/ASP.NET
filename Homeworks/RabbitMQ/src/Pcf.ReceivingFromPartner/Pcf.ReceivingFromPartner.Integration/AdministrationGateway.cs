using System;
using System.Threading.Tasks;
using EasyNetQ;
using Pcf.Integration;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class PartnerManagerEventPublisher 
        : IPartnerManagerEventPublisher
    {
        private readonly IBus _bus;

        public PartnerManagerEventPublisher (IBus bus)
        {
            _bus = bus;
        }

        public async Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId)
        {
            var message = new PartnerManagerPromoCodeAppliedEvent
            {
                PartnerManagerId = partnerManagerId
            };

            await _bus.PubSub.PublishAsync(message, config => 
                config.WithTopic("PartnerManagerPromoCodeApplied"));
        }
    }
}