namespace KiloMart.Domain.Orders.Shared;

 public class OrderActivity
    {
        public long Id { get; private set; }
        public long OrderId { get; private set; }
        public DateTime Date { get; private set; }
        public OrderActivityType ActivityType { get; private set; }
        public int OperatedBy { get; private set; }

        public OrderActivity(long orderId, DateTime date, OrderActivityType activityType, int operatedBy)
        {
            if (date == default)
                throw new ArgumentException("Date must be specified.");

            OrderId = orderId;
            Date = date;
            ActivityType = activityType;
            OperatedBy = operatedBy;
        }

        public void UpdateActivityType(OrderActivityType newActivityType)
        {
            ActivityType = newActivityType;
        }
    }