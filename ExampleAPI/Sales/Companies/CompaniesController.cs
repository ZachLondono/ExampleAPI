﻿using ExampleAPI.Sales.Companies.Commands;
using ExampleAPI.Sales.Companies.DTO;
using ExampleAPI.Sales.Companies.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Sales.Companies;

[ApiController]
[Route("[controller]")]
public class CompaniesController : ControllerBase {

    private readonly ILogger<CompaniesController> _logger;
    private readonly ISender _sender;

    public CompaniesController(ILogger<CompaniesController> logger, ISender sender) {
        _logger = logger;
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CompanyDTO))]
    public Task<IActionResult> Create([FromBody] NewCompany newCompany) {
        _logger.LogInformation("Creating new company");
        return _sender.Send(new Create.Command(HttpContext, newCompany));
    }

    [HttpGet]
    public Task<IActionResult> GetAll() {
        _logger.LogInformation("Getting all companies");
        return _sender.Send(new GetAll.Query(HttpContext));
    }

    [Route("{companyId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Get(Guid companyId) {
        _logger.LogInformation("Getting company {companyId}", companyId);
        return _sender.Send(new Get.Query(HttpContext, companyId));
    }

    [Route("{companyId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid companyId) {
        _logger.LogInformation("Deleting company {companyId}", companyId);
        return await _sender.Send(new Delete.Command(HttpContext, companyId));
    }
    
    [Route("{companyId}/name")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> SetName(Guid companyId, [FromBody] NewCompanyName newName) {
        _logger.LogInformation("Updating company name {companyId}", companyId);
        return _sender.Send(new SetName.Command(HttpContext, companyId, newName));
    }

    [Route("{companyId}/address")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> SetAddress(Guid companyId, [FromBody] AddressDTO newAddress) {
        _logger.LogInformation("Setting address for company {companyId}", companyId);
        return _sender.Send(new SetAddress.Command(HttpContext, companyId, newAddress));
    }

}
