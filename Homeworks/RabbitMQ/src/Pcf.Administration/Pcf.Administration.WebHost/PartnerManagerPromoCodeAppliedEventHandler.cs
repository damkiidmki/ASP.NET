using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Integration;

namespace Pcf.Administration.WebHost;

public class PartnerManagerPromoCodeAppliedEventHandler
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly ILogger<PartnerManagerPromoCodeAppliedEventHandler> _logger;

    public PartnerManagerPromoCodeAppliedEventHandler(
        IRepository<Employee> employeeRepository,
        ILogger<PartnerManagerPromoCodeAppliedEventHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task HandleAsync(PartnerManagerPromoCodeAppliedEvent message)
    {
        var employee = await _employeeRepository.GetByIdAsync(message.PartnerManagerId);

        if (employee == null)
        {
            return;
        }

        employee.AppliedPromocodesCount++;
        await _employeeRepository.UpdateAsync(employee);
    }
}