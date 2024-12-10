using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using MongoDB.Driver;

namespace Dotnet.Homeworks.DataAccess.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders;

    public OrderRepository(IMongoCollection<Order> mongoCollection)
    {
        _orders = mongoCollection;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersFromUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _orders.Find(x => x.OrdererId == userId).ToListAsync(cancellationToken);
        return result;
    }

    public async Task<Order?> GetOrderByGuidAsync(Guid orderId, CancellationToken cancellationToken)
    {
       var result = await _orders.Find(x => x.Id == orderId).FirstOrDefaultAsync(cancellationToken);
       return result;
    }

    public async Task DeleteOrderByGuidAsync(Guid orderId, CancellationToken cancellationToken)
    {
        await _orders.DeleteOneAsync(x => x.Id == orderId, cancellationToken);
    }

    public async Task UpdateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        await _orders.ReplaceOneAsync(x => x.Id == order.Id, order, cancellationToken: cancellationToken);
    }

    public async Task<Guid> InsertOrderAsync(Order order, CancellationToken cancellationToken)
    {
        await _orders.InsertOneAsync(order, cancellationToken: cancellationToken);
        return order.Id;
    }
}