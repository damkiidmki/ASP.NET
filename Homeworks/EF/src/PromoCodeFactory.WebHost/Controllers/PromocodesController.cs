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
/// Промокоды
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PromocodesController
    : ControllerBase
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper _mapper;
    private readonly IRepository<Preference> _preferenceRepository;
    private readonly IRepository<PromoCode> _repository;

    public PromocodesController(
        IRepository<PromoCode> repository,
        IRepository<Preference> preferenceRepository,
        IRepository<Customer> customerRepository,
        IMapper mapper)
    {
        _repository = repository;
        _preferenceRepository = preferenceRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }  

    /// <summary>
    /// Получить все промокоды.
    /// </summary>
    /// <returns>Возвращает список промокодов.</returns>
    /// <response code="200">Список промокодов.</response>
    /// <response code="400">Плохой запрос.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PromoCodeShortResponse>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<ActionResult> GetPromocodesAsync()
    {
        try
        {
            var promoCodes = await _repository.GetAllAsync();
            return Ok(_mapper.Map<List<PromoCodeShortResponse>>(promoCodes));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Создать промокод и выдать его клиентам с указанным предпочтением
    /// </summary>
    /// <returns>Ok</returns>
    /// <response code="200">Успешное выполнение операции.</response>
    /// <response code="400">Плохой запрос.</response>
    [HttpPost]
    [ProducesResponseType(typeof(List<PromoCodeShortResponse>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
    {
        try
        {
            var preference = await _preferenceRepository.GetQueryable()
                .FirstOrDefaultAsync(p => p.Name == request.Preference);

            if (preference == null)
            {
                return NotFound($"Предпочтение '{request.Preference}' не найдено");
            }

            var customersWithPreference = await _customerRepository.GetQueryable()
                .Where(c => c.CustomerPreferences.Any(cp => cp.PreferenceId == preference.Id))
                .ToListAsync();

            var customerPromoCodes = customersWithPreference
                .Select(customer => new PromoCode
                {
                    Code = request.PromoCode,
                    ServiceInfo = request.ServiceInfo,
                    PartnerName = request.PartnerName,
                    BeginDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    PreferenceId = preference.Id,
                    CustomerId = customer.Id
                })
                .ToList();

            await _repository.CreateRangeAsync(customerPromoCodes);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при выдаче промокодов: {ex.Message}");
        }
    }
}