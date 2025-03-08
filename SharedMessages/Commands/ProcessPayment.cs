namespace SharedMessages.Commands
{
    public record ProcessPayment(Guid OrderId, decimal Amount);
}
