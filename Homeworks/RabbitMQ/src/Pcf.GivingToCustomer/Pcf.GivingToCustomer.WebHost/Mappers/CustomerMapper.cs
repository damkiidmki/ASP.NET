using System;
using System.Collections.Generic;
using System.Linq;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;
using Pet.GivingToCustomer.WebHost.Protos;
using CustomerShortResponse = Pet.GivingToCustomer.WebHost.Protos.CustomerShortResponse;
using PreferenceResponse = Pet.GivingToCustomer.WebHost.Protos.PreferenceResponse;

namespace Pcf.GivingToCustomer.WebHost.Mappers
{
    public class CustomerMapper
    {
        public static Customer MapFromModel(CreateCustomerRequest request, List<Preference> preferences)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Preferences = preferences?.Select(p => new CustomerPreference
                {
                    CustomerId = Guid.NewGuid(),
                    PreferenceId = p.Id
                }).ToList()
            };
        }
   
        public static void MapFromModel(UpdateCustomerRequest request, List<Preference> preferences, Customer customer)
        {
            customer.Email = request.Email;
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            
            if (preferences != null)
            {
                customer.Preferences = preferences.Select(p => new CustomerPreference
                {
                    CustomerId = customer.Id,
                    PreferenceId = p.Id
                }).ToList();
            }
        }
        
        public static CustomerShortResponse MapToGrpcShortResponse(Customer customer)
        {
            return new CustomerShortResponse
            {
                Id = customer.Id.ToString(),
                Email = customer.Email ?? "",
                FirstName = customer.FirstName ?? "",
                LastName = customer.LastName ?? ""
            };
        }
        
        public static CustomerFullResponse MapToGrpcFullResponse(Customer customer)
        {
            var response = new CustomerFullResponse
            {
                Id = customer.Id.ToString(),
                Email = customer.Email ?? "",
                FirstName = customer.FirstName ?? "",
                LastName = customer.LastName ?? ""
            };

            if (customer.Preferences != null)
            {
                response.Preferences.AddRange(customer.Preferences.Select(cp => new PreferenceResponse
                {
                    Id = cp.PreferenceId.ToString(),
                    Name = cp.Preference?.Name ?? ""
                }));
            }

            if (customer.PromoCodes != null)
            {
                response.PromoCodes.AddRange(customer.PromoCodes.Select(pc => new PromoCodeResponse
                {
                    Id = pc.PromoCodeId.ToString(),
                    Code = pc.PromoCode?.Code ?? "",
                    ServiceInfo = pc.PromoCode?.ServiceInfo ?? "",
                    BeginDate = pc.PromoCode?.BeginDate.ToString("yyyy-MM-ddTHH:mm:ss") ?? "",
                    EndDate = pc.PromoCode?.EndDate.ToString("yyyy-MM-ddTHH:mm:ss") ?? "",
                    PartnerId = pc.PromoCode?.PartnerId.ToString() ?? ""
                }));
            }

            return response;
        }
        
        public static CustomerListResponse MapToGrpcResponseList(IEnumerable<Customer> customers)
        {
            var response = new CustomerListResponse();
            response.Customers.AddRange(customers.Select(MapToGrpcShortResponse));
            return response;
        }
        
        public static List<Guid> ParsePreferenceIds(IEnumerable<string> preferenceIds)
        {
            var guids = new List<Guid>();
            foreach (var id in preferenceIds)
            {
                if (Guid.TryParse(id, out var guid))
                    guids.Add(guid);
            }
            return guids;
        }
        
        public static Customer MapFromModel(CreateOrEditCustomerRequest model, IEnumerable<Preference> preferences, Customer customer = null)
        {
            if (customer == null)
            {
                customer = new Customer();
                customer.Id = Guid.NewGuid();
            }

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Email = model.Email;

            customer.Preferences = preferences.Select(x => new CustomerPreference()
            {
                CustomerId = customer.Id,
                Preference = x,
                PreferenceId = x.Id
            }).ToList();

            return customer;
        }
    }
}
