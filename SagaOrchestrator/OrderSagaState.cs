using MassTransit;
using SharedMessages.Commands;

namespace SagaOrchestrator
{
    public class OrderSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = default!;
        public Guid OrderId { get; set; }
        public decimal  Amount { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
