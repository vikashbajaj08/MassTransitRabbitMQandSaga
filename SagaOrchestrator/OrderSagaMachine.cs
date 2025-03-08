using MassTransit;
using SharedMessages.Commands;
using SharedMessages.Events;

namespace SagaOrchestrator
{
    public class OrderSagaMachine : MassTransitStateMachine<OrderSagaState>
    {
        //State
        public State Submitted { get; private set; }
        public State PaymentProcessed { get; private set; }
        public State InventoryReserved { get; private set; }
        public State Shipped { get; private set; }
        public State Failed { get; private set; }

        //Event

        public Event<OrderSubmitted> OrderSubmittedEvent { get; private set; }
        public Event<PaymentProcessed> PaymentProcessedEvent { get; private set; }
        public Event<InventoryReserved> InventoryReservedEvent { get; private set; }
        public Event<OrderShipped> OrderShippedEvent { get; private set; }
        public Event<OrderFailed> OrderFailedEvent { get; private set; }
        public Event<PaymentRefunded> PaymentRefundedEvent { get; private set; }

        public OrderSagaMachine()
        {
            //Defining current state

            InstanceState(x => x.CurrentState);

            //Define the behaviour of ordersubmitted event

            Initially(
                When(OrderSubmittedEvent)
                .Then(context =>
                {
                    Console.WriteLine($"Order submitted with order Id: {context.Message.OrderId}");
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.Items = context.Message.Items;
                }).TransitionTo(Submitted)
            .Send(new Uri("exchange:payment-exchange?bind=true&routingKey=ProcessPayment"),context=>new ProcessPayment(context.Saga.OrderId,context.Saga.Amount)));


            During(Submitted, When(PaymentProcessedEvent)
                .Then(context =>
                {
                    Console.WriteLine($"Payment Processed: {context.Message.OrderId}");
                    context.Saga.OrderId = context.Message.OrderId;
                }).TransitionTo(PaymentProcessed)
            .Send(new Uri("exchange:inventory-exchange?bind=true&routingKey=ReserveInventory"),context=>new ReserveInventory(context.Saga.OrderId,context.Saga.Items)));


            During(PaymentProcessed, When(InventoryReservedEvent)
                .Then(context =>
                {
                    Console.WriteLine($"Payment processed for orderId: {context.Message.OrderId}");
                }).TransitionTo(InventoryReserved)
                .Send(new Uri("exchange:Shipping-exchange?bind=true&routingKey=ShippingOrder"),context=> new ShipOrder(context.Saga.OrderId)));

            During(InventoryReserved, When(OrderShippedEvent)
                .Then(context =>
                {
                    Console.WriteLine($"Order shipped for orderId: {context.Message.OrderId}");
                }).TransitionTo(Shipped).Finalize());

            DuringAny(
                When(OrderFailedEvent)
                .Then(context =>
                {
                    Console.WriteLine($"Order failed: {context.Saga.OrderId}");
                }).TransitionTo(Failed)
                .IfElse(context => context.Saga.CurrentState == PaymentProcessed.Name,
                then => then.Send(new Uri("exchange:refund-exchange?bind=true&routingKey=RefundProcess"), context => new RefundPayment(context.Saga.OrderId, context.Saga.Amount)),
                @else => @else.Send(new Uri("exchange:inventory-exchange?bind=true&routingKey=ReleaseInventory"), context => new ReleaseInventory(context.Saga.OrderId, context.Saga.Items))
                ));

            During(Failed, When(PaymentRefundedEvent)
                .Then(context =>
                {
                    Console.WriteLine($"Payment refunded for order: {context.Saga.OrderId}");
                }).Finalize()
                
                );
            SetCompletedWhenFinalized();
        }
    }
}
