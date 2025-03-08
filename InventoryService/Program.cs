using InventoryService.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InventoryConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("RabbitMQHost"));
        cfg.ReceiveEndpoint("inventory-queue", e =>
        {
            e.Consumer<InventoryConsumer>();
            e.Bind("inventory-exchange", x =>
            {
                x.RoutingKey = "ReserveInventory";
                x.ExchangeType = "direct";
            });

            e.Bind("inventory-exchange", x =>
            {
                x.RoutingKey = "ReleaseInventory";
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

