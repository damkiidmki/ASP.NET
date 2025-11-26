using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.Hubs;

/// <summary>
/// Хаб для работы с клиентами через SignalR
/// </summary>
public class CustomerHub : Hub
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Preference> _preferenceRepository;

    public CustomerHub(IRepository<Customer> customerRepository,
        IRepository<Preference> preferenceRepository)
    {
        _customerRepository = customerRepository;
        _preferenceRepository = preferenceRepository;
    }

    /// <summary>
    /// Получить список клиентов
    /// </summary>
    /// <returns></returns>
    public async Task<List<CustomerShortResponse>> GetCustomers()
    {
        var customers = await _customerRepository.GetAllAsync();

        var response = customers.Select(x => new CustomerShortResponse()
        {
            Id = x.Id,
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName
        }).ToList();

        return response;
    }

    /// <summary>
    /// Получить клиента по id
    /// </summary>
    /// <param name="id">Id клиента, например a6c8c6b1-4349-45b0-ab31-244740aaf0f0</param>
    /// <returns></returns>
    public async Task<CustomerResponse> GetCustomer(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        
        if (customer == null)
            throw new HubException($"Клиент с id {id} не найден");

        var response = new CustomerResponse(customer);
        return response;
    }

    /// <summary>
    /// Создать нового клиента
    /// </summary>
    /// <returns>Id созданного клиента</returns>
    public async Task<Guid> CreateCustomer(CreateOrEditCustomerRequest request)
    {
        // Получаем предпочтения из бд и сохраняем большой объект
        var preferences = await _preferenceRepository
            .GetRangeByIdsAsync(request.PreferenceIds);

        Customer customer = CustomerMapper.MapFromModel(request, preferences);

        await _customerRepository.AddAsync(customer);

        // Возвращаем результат только вызвавшему клиенту
        return customer.Id;
    }

    /// <summary>
    /// Обновить клиента
    /// </summary>
    /// <param name="id">Id клиента, например a6c8c6b1-4349-45b0-ab31-244740aaf0f0</param>
    /// <param name="request">Данные запроса</param>
    public async Task EditCustomer(Guid id, CreateOrEditCustomerRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null)
            throw new HubException($"Клиент с id {id} не найден");

        var preferences = await _preferenceRepository.GetRangeByIdsAsync(request.PreferenceIds);

        CustomerMapper.MapFromModel(request, preferences, customer);

        await _customerRepository.UpdateAsync(customer);
    }

    /// <summary>
    /// Удалить клиента
    /// </summary>
    /// <param name="id">Id клиента, например a6c8c6b1-4349-45b0-ab31-244740aaf0f0</param>
    public async Task DeleteCustomer(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null)
            throw new HubException($"Клиент с id {id} не найден");

        await _customerRepository.DeleteAsync(customer);
    }
}