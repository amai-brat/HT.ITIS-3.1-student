using Dotnet.Homeworks.Features.Orders.Commands.CreateOrder;
using Dotnet.Homeworks.Features.Orders.Commands.DeleteOrder;
using Dotnet.Homeworks.Features.Orders.Commands.UpdateOrder;
using Dotnet.Homeworks.Features.Orders.Queries.GetOrder;
using Dotnet.Homeworks.Features.Orders.Queries.GetOrders;
using Dotnet.Homeworks.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Homeworks.MainProject.Controllers;

[ApiController]
public class OrderManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetUserOrdersAsync(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrdersQuery(), cancellationToken);
        return result.IsSuccess 
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("order/{id:guid}")]
    public async Task<IActionResult> GetUserOrdersAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderQuery(id), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("order")]
    public async Task<IActionResult> CreateOrderAsync([FromBody] IEnumerable<Guid> productsIds,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateOrderCommand(productsIds), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("order/{id:guid}")]
    public async Task<IActionResult> UpdateOrderAsync(Guid id, [FromBody] IEnumerable<Guid> productsIds,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateOrderCommand(id, productsIds), cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Error);
    }

    [HttpDelete("order/{id:guid}")]
    public async Task<IActionResult> DeleteOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteOrderByGuidCommand(id), cancellationToken);
        return result.IsSuccess 
            ? NoContent()
            : BadRequest(result.Error);
    }
}