using System;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways
{
    public interface IPartnerManagerEventPublisher
    {
        Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId);
    }
}