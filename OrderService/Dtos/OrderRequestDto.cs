namespace OrderService.Dtos
{
    public record OrderRequestDto
    {
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
    }
}
