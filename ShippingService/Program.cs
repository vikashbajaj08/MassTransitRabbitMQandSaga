using MassTransit;
using ShippingService.Consumers;

var builder = WebApplication.CreateBuilder(args);

/*RabbitMQ Configuration*/
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration.GetValue<string>("RabbitMQHost"));

        config.ReceiveEndpoint("shipping-order-queue", e =>
        {
            e.Consumer<ShippingConsumer>();
            e.Bind("Shipping-exchange", x =>
            {
                x.RoutingKey = "ShippingOrder";
                x.ExchangeType = "direct";
            });
            //Retry
            e.UseMessageRetry(r=>r.Interval(3,TimeSpan.FromSeconds(5)));

            //e.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(5)));

            //Circuit Breaker

            e.UseKillSwitch(options =>
            {
                //10 continious failure
                options.SetActivationThreshold(10)
                // or 15% failure
                .SetTripThreshold(0.15)
                //restart
                .SetRestartTimeout(TimeSpan.FromMinutes(1));
            });

            //Fanout example (Ignore Routing Key)
            //e.Bind("order-exchage", x => { x.ExchangeType = "fanout";});

            //Hearder Exchange example
            //e.Bind("order-exchange", x =>
            //{
            //    x.ExchangeType = "headers";
            //    x.SetBindingArgument("Product", "Laptop");
            //    x.SetBindingArgument("ProductType", "Electronics");
            //    x.SetBindingArgument("x-match", "all");

            //});

            //Topics Example

            //e.Consumer<OrderPlacedConsumer>();
            //e.Bind("order-exchange", x =>
            //{
            //    x.RoutingKey = "order.*"; //* match exactly one word and # match zero or more words
            //    x.ExchangeType = "topic";
            //});
        });
        
    });

    
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();
