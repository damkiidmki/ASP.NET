using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class Customer
    : BaseEntity
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; set; }

    public virtual IEnumerable<CustomerPreference> CustomerPreferences { get; set; } = new List<CustomerPreference>();

    public virtual IEnumerable<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();
}