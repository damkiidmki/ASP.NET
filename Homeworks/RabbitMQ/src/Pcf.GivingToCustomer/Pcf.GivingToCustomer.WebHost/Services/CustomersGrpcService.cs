using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pet.GivingToCustomer.WebHost.Protos;

namespace Pcf.GivingToCustomer.WebHost.Services;

public class CustomersGrpcService : CustomersService.CustomersServiceBase
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Preference> _preferenceRepository;

    public CustomersGrpcService(
        IRepository<Customer> customerRepository,
        IRepository<Preference> preferenceRepository)
    {
        _customerRepository = customerRepository;
        _preferenceRepository = preferenceRepository;
    }

    public override async Task<CustomerListResponse> GetCustomers(Empty request, ServerCallContext context)
    {
        
        var customers = await _customerRepository.GetAllAsync();
        return CustomerMapper.MapToGrpcResponseList(customers);
    }

    public override async Task<CustomerFullResponse> GetCustomer(GetCustomerRequest request, ServerCallContext context)
    {

        if (!Guid.TryParse(request.Id, out var customerId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid customer ID format"));
        }

        var customer = await _customerRepository.GetByIdAsync(customerId);

        if (customer == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Customer with ID {request.Id} not found"));
        }
        
        return CustomerMapper.MapToGrpcFullResponse(customer);
    }


    public override async Task<StringValue> CreateCustomer(CreateCustomerRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));
        }

        var preferenceIds = CustomerMapper.ParsePreferenceIds(request.PreferenceIds);
        var preferences = await _preferenceRepository.GetRangeByIdsAsync(preferenceIds);
    
        Customer customer = CustomerMapper.MapFromModel(request, preferences.ToList());
        await _customerRepository.AddAsync(customer);
        
        return new StringValue { Value = customer.Id.ToString() };
    }

    public override async Task<Empty> UpdateCustomer(UpdateCustomerRequest request, ServerCallContext context)
    {

        if (!Guid.TryParse(request.Id, out var customerId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid customer ID format"));
        }

        var customer = await _customerRepository.GetByIdAsync(customerId);

        if (customer == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Customer with ID {request.Id} not found"));
        }

        var preferenceIds = CustomerMapper.ParsePreferenceIds(request.PreferenceIds);
        var preferences = await _preferenceRepository.GetRangeByIdsAsync(preferenceIds);

        CustomerMapper.MapFromModel(request, preferences.ToList(), customer);
        await _customerRepository.UpdateAsync(customer);

        return new Empty();
    }

    public override async Task<Empty> DeleteCustomer(DeleteCustomerRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var customerId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid customer ID format"));
        }

        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Customer with ID {request.Id} not found"));
        }

        await _customerRepository.DeleteAsync(customer);

        return new Empty();
    }
}