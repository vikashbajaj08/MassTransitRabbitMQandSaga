using MassTransit;
using SharedMessages.Commands;

namespace InventoryService.Consumers
{
    public class InventoryConsumer : IConsumer<ReserveInventory>, IConsumer<ReleaseInventory>
    {
        public async Task Consume(ConsumeContext<ReserveInventory> context)
        {
            var orderId = context.Message.OrderId;

            //put the logic to reserve the inventory

            Console.WriteLine($"Reserving the inventory for order: {orderId}");

            //publish the inventory reserved event

            await context.Publish(new InventoryReserved(orderId));
        }

        public async Task Consume(ConsumeContext<ReleaseInventory> context)
        {
            var orderId = context.Message.OrderId;

            Console.WriteLine($"Releasing the inventory for order: {orderId}");



            await context.Publish(new InventoryReleased(orderId));
        }
    }
}
