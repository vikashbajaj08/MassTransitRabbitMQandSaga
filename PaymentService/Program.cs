using MassTransit;
using PaymentService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("RabbitMQHost"));
        cfg.ReceiveEndpoint("payment-queue", e =>
        {
            e.Consumer<PaymentConsumer>();
            e.Bind("payment-exchange", x =>
            {
                x.RoutingKey = "ProcessPayment";
                x.ExchangeType = "direct";
            });
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
