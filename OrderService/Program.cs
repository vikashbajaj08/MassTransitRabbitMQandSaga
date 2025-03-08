using MassTransit;
using OrderService.Dtos;
using SharedMessages.Commands;
using SharedMessages.Events;


var builder = WebApplication.CreateBuilder(args);

/*RabbitMQ Configuration*/
builder.Services.AddMassTransit((x) =>
{
    x.UsingRabbitMq((context, config) =>
    {
        //Setup host
        config.Host(builder.Configuration.GetValue<string>("RabbitMQHost"));

        //Setup dirct exchange
        config.Message<OrderSubmitted>(x => x.SetEntityName("order-exchange"));
        config.Publish<OrderSubmitted>(x => { x.ExchangeType = "direct"; });

        //Setup Fanout exchange
        //config.Message<OrderPlaced>(x => x.SetEntityName("order-exchange"));
        //config.Publish<OrderPlaced>(x => { x.ExchangeType = "fanout"; });

        //Setup Topic exchange
        //config.Message<OrderPlaced>(x => x.SetEntityName("order-exchange"));
        //config.Publish<OrderPlaced>(x => { x.ExchangeType = "topic"; });

        //Setup Headers exchange
        //config.Message<OrderPlaced>(x => x.SetEntityName("order-exchange"));
        //config.Publish<OrderPlaced>(x => { x.ExchangeType = "headers"; });


    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

/*Endpoint*/

app.MapPost("/orders", async (decimal amount, List<OrderItem> items, IBus bus) =>
{
    var order = new OrderSubmitted(Guid.NewGuid(), amount,items);


    try
    {
        await bus.Publish(order, context => { context.SetRoutingKey("order.submitted"); });

        //Topic example
        //var routingKey = order.Quantity > 10 ? "order.created" : "order.created.Plus";
        //await bus.Publish(orderMessage, context => { context.SetRoutingKey(routingKey);  });

        //Headers example
        //await bus.Publish(orderMessage, context => { context.Headers.Set("product", "Laptop"); context.Headers.Set("type", "electronics"); });

        //Fanout example
        //await bus.Publish(orderMessage);

        return Results.Created($"/orders/{order.OrderId}", order);
    }
    catch (Exception ex)
    {
        // Log the exception and handle it appropriately
        Console.WriteLine($"Error publishing message: {ex.Message}");
        return Results.StatusCode(500); // Return an appropriate HTTP status code
    }
});

//async Task FailOrder(Guid OrderId, string Reason, IBus bus)
//{
//    Console.WriteLine($"Order: {OrderId} failed to process with reason: {Reason}");

    
//   await bus.Publish(new OrderFailed(OrderId, Reason), context =>
//   {
//       context.SetRoutingKey("order.failed");
//   });
//}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
