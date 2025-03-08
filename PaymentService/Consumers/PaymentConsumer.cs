using MassTransit;
using SharedMessages.Commands;

namespace PaymentService.Consumers
{
    public class PaymentConsumer : IConsumer<ProcessPayment>
    {
        public async Task Consume(ConsumeContext<ProcessPayment> context)
        {
            var orderId = context.Message.OrderId;

            //put logic for payment
            Console.WriteLine($"Processing payment for order: {orderId}");

            //Payment processed
            await context.Publish(new PaymentProcessed(orderId));
        }

        
    }
}
