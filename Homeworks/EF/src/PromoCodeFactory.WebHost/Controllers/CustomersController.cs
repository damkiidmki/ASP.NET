using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IRepository<CustomerPreference> _customerPreferenceRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper _mapper;

    public CustomersController(
        IMapper mapper,
        IRepository<Customer> repository,
        IRepository<CustomerPreference> customerPreferenceRepository)
    {
        _mapper = mapper;
        _customerRepository = repository;
        _customerPreferenceRepository = customerPreferenceRepository;
    }

    /// <summary>
    /// Получить всех клиентов.
    /// </summary>
    /// <returns>Возвращает список клиентов.</returns>
    /// <response code="200">Клиенты получены.</response>
    /// <response code="400">Плохой запрос.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerShortResponse>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<ActionResult> GetCustomersAsync()
    {
        try
        {
            var customers = await _customerRepository.GetAllAsync();
            var result = _mapper.Map<List<CustomerShortResponse>>(customers);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Получить клиента по Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Возвращает клиента.</returns>
    /// <response code="200">Клиент получен.</response>
    /// <response code="400">Плохой запрос.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<ActionResult> GetCustomerAsync(Guid id)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            var result = _mapper.Map<CustomerResponse>(customer);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Создать клиента с предпочтениями.
    /// </summary>
    /// <param name="request">Данные о клиенте.</param>
    /// <returns>CreatedAtAction.</returns>
    /// <response code="201">Клиент успешно создан.</response>
    /// <response code="400">Плохой запрос.</response>
    [ProducesResponseType(typeof(CustomerResponse), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [HttpPost]
    public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
    {
        try
        {
            var newCustomer = await _customerRepository.CreateAsync(new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            });

            var customerPreferences = request.PreferenceIds
                .Distinct()
                .Select(preferenceId => new CustomerPreference
                {
                    CustomerId = newCustomer.Id,
                    PreferenceId = preferenceId,
                });

            await _customerPreferenceRepository.CreateRangeAsync(customerPreferences);

            return CreatedAtAction(
                nameof(GetCustomerAsync),
                new { id = newCustomer.Id },
                _mapper.Map<CustomerResponse>(newCustomer)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Обновляет клиента с предпочтениями.
    /// </summary>
    /// <param name="id">Id клиента.</param>
    /// <param name="request">Обновленные данные.</param>
    /// <returns>NoContent.</returns>
    /// <response code="204">Данные о клиенте обновлены.</response>
    /// <response code="400">Плохой запрос.</response>
    /// <response code="404">Клиент не найден.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;

            await _customerRepository.UpdateAsync(customer);

            if (request.PreferenceIds != null)
            {
                var existingPreferences = await _customerPreferenceRepository.GetQueryable()
                    .Where(cp => cp.CustomerId == id)
                    .ToListAsync();

                if (existingPreferences.Any())
                {
                    await _customerPreferenceRepository.DeleteRangeAsync(existingPreferences);
                }

                if (request.PreferenceIds.Any())
                {
                    var customerPreferences = request.PreferenceIds
                        .Distinct()
                        .Select(preferenceId => new CustomerPreference
                        {
                            CustomerId = id,
                            PreferenceId = preferenceId,
                        });

                    await _customerPreferenceRepository.CreateRangeAsync(customerPreferences);
                }
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Удаляет клиента.
    /// </summary>
    /// <param name="id">Id клиента.</param>
    /// <returns>NoContent.</returns>
    /// <response code="204">Клиент удален.</response>
    /// <response code="400">Плохой запрос.</response>
    /// <response code="404">Клиент не найден.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> DeleteCustomerAsync(Guid id)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            await _customerRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}