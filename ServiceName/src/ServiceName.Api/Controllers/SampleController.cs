using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceName.Application.DTO;
using ServiceName.Application.Events;
using ServiceName.Application.Interfaces;

namespace ServiceName.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/samples")]
public class SampleController: ControllerBase
{
    private readonly ISampleService _service;
    private readonly IEventBus _eventBus;

    public SampleController(ISampleService service, IEventBus eventBus)
    {
        _service = service;
        _eventBus = eventBus;
    }

    [Authorize(Policy = "ReadPolicy")]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [Authorize(Policy = "ReadPolicy")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null) 
            return NotFound();

        return Ok(result);
    }

    [Authorize(Policy = "WritePolicy")]
    [HttpPost]
    public async Task<IActionResult> AddAsync(SampleDTO dto)
    {
        await _service.AddAsync(dto);

        await _eventBus.PublishAsync(
            new SampleCreatedEvent(dto.Name));

        return Ok();
    }

    [Authorize(Policy = "WritePolicy")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, SampleDTO dto)
    {
        // Prevent inconsistent updates if client sends mismatched ids
        if (dto.Id != Guid.Empty && dto.Id != id)
            return BadRequest("Route id and body id must match.");

        dto.Id = id;

        await _service.UpdateAsync(dto);

        await _eventBus.PublishAsync(
            new SampleUpdatedEvent(dto.Id));

        return Ok();
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByIdAsync(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        await _service.DeleteByIdAsync(id);

        if (result != null)
        {
            await _eventBus.PublishAsync(
                new SampleDeletedEvent(result.Id));
        }
        return Ok();
    }
}
