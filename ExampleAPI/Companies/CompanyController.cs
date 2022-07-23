using ExampleAPI.Companies.Commands;
using ExampleAPI.Companies.DTO;
using ExampleAPI.Companies.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Companies;

[ApiController]
[Route("[controller]")]
public class CompanyController : ControllerBase {

    private readonly ILogger<CompanyController> _logger;
    private readonly ISender _sender;

    public CompanyController(ILogger<CompanyController> logger, ISender sender) {
        _logger = logger;
        _sender = sender;
    }

    [HttpPost]
    public Task<IActionResult> Create([FromBody] NewCompany newCompany) {
        _logger.LogInformation("Creating new company");
        return _sender.Send(new Create.Command(newCompany));
    }

    [Route("GetAllCompanies")]
    [HttpGet]
    public Task<IActionResult> GetAll() {
        _logger.LogInformation("Getting all companies");
        return _sender.Send(new GetAll.Query());
    }

    [Route("GetCompany/{companyId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Get(Guid companyId) {
        _logger.LogInformation("Getting company {companyId}", companyId);
        return _sender.Send(new Get.Query(companyId));
    }

    [Route("DeleteCompany/{companyId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid companyId) {
        _logger.LogInformation("Deleting company {companyId}", companyId);
        return await _sender.Send(new Delete.Command(companyId));
    }
    
    [Route("SetName/{companyId}/{newName}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> SetName(Guid companyId, string newName) {
        _logger.LogInformation("Updating company name {companyId}", companyId);
        return _sender.Send(new SetName.Command(companyId, newName));
    }

    [Route("SetAddress/{companyId}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> SetAddress(Guid companyId, [FromBody] AddressDTO newAddress) {
        _logger.LogInformation("Setting address for company {companyId}", companyId);
        return _sender.Send(new SetAddress.Command(companyId, newAddress));
    }

}
