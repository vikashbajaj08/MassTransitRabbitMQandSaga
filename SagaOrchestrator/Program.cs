using MassTransit;
using SagaOrchestrator;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderSagaMachine, OrderSagaState>()
     .InMemoryRepository();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("RabbitMQHost"), u =>
        {
            u.Username("guest");
            u.Password("guest");
        });

        cfg.ReceiveEndpoint("Order-Submitted-Queue", e =>
        {
            e.Bind("order-exchange", s =>
            {
                s.ExchangeType = "direct";
                s.RoutingKey = "order.submitted";
            });
            e.StateMachineSaga<OrderSagaState>(context);
        });
        
    });
    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
