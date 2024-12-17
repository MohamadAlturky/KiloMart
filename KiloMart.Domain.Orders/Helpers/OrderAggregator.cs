using KiloMart.Domain.Orders.Repositories;

namespace KiloMart.Domain.Orders.Helpers;
public class AggregatedOrder
{
    public OrderDetailsDto OrderDetails { get; set; } = null!;
    public List<OrderProductDetailsDto> OrderProductDetails { get; set; } = [];
    public List<OrderProductOfferDetailsDto> OrderProductOfferDetails { get; set; } = [];
}

public static class OrderAggregator
{
    public static List<AggregatedOrder> Aggregate(
        List<OrderDetailsDto> orderDetails,
        List<OrderProductDetailsDto> orderProductDetails,
        List<OrderProductOfferDetailsDto> orderProductOfferDetails)
    {
        // Create a dictionary to hold aggregated orders by Order Id
        var aggregatedOrders = new Dictionary<long, AggregatedOrder>();

        // Populate the dictionary with OrderDetails
        foreach (var order in orderDetails)
        {
            if (!aggregatedOrders.ContainsKey(order.Id))
            {
                aggregatedOrders[order.Id] = new AggregatedOrder
                {
                    OrderDetails = order,
                    OrderProductDetails = [],
                    OrderProductOfferDetails = []
                };
            }
        }

        // Add OrderProductDetails to the corresponding aggregated orders
        foreach (OrderProductDetailsDto productDetail in orderProductDetails)
        {
            if (aggregatedOrders.ContainsKey(productDetail.ItemOrder))
            {
                aggregatedOrders[productDetail.ItemOrder].OrderProductDetails.Add(productDetail);
            }
        }

        // Add OrderProductOfferDetails to the corresponding aggregated orders
        foreach (var offerDetail in orderProductOfferDetails)
        {
            if (aggregatedOrders.ContainsKey(offerDetail.Order))
            {
                aggregatedOrders[offerDetail.Order].OrderProductOfferDetails.Add(offerDetail);
            }
        }

        // Return the list of aggregated orders
        return [.. aggregatedOrders.Values];
    }
}