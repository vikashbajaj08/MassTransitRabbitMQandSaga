using MassTransit;
using SharedMessages.Commands;

namespace ShippingService.Consumers
{
    public class ShippingConsumer : IConsumer<ShipOrder>
    {
        public async Task Consume(ConsumeContext<ShipOrder> context)
        {
            var orderId = context.Message.OrderId;

            Console.WriteLine($"Shipping order: {orderId}");

            await context.Publish(new OrderShipped(orderId));
        }
    }
}
