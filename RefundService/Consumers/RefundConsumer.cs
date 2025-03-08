using MassTransit;
using SharedMessages.Commands;

namespace RefundService.Consumers
{
    public class RefundConsumer : IConsumer<RefundPayment>
    {
        public async Task Consume(ConsumeContext<RefundPayment> context)
        {
            var orderId = context.Message.OrderId;

            //put logic for payment
            Console.WriteLine($"Refunding payment for order: {orderId}");

            //Payment refund processed
            await context.Publish(new PaymentRefunded(orderId));
        }
    }
}
