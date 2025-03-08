using SharedMessages.Commands;

namespace SharedMessages.Events
{
    public record OrderSubmitted(Guid OrderId, decimal Amount, List<OrderItem> Items);
    
}
