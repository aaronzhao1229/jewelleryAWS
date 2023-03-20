namespace API.Entities.OrderAggregate
{
    // the idea behind an enum is that instead of using something like an integer to represent a value, we can use a restricted set of values instead of our human readable
    public enum OrderStatus
    {
        Pending,
        PaymentReceived,
        PaymentFailed
    }
}