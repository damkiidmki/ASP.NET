using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetData
{
    public Partner BasePartner()
    {
        var partner = new Partner()
        {
            Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
            Name = "Суперигрушки",
            IsActive = true,
            PartnerLimits = new List<PartnerPromoCodeLimit>()
            {
                new PartnerPromoCodeLimit()
                {
                    Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
                    CreateDate = new DateTime(2020, 07, 9),
                    EndDate = new DateTime(2020, 10, 9),
                    Limit = 100
                }
            }
        };

        return partner;
    }
}