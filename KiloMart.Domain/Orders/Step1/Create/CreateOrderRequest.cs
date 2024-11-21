// namespace KiloMart.Domain.Orders.Step1.Create;

// public class CreateOrderRequest
// {
//     public int CustomerLocation { get; set; }
//     public List<OrderItemRequest> OrderItems { get; set; } = [];

//     // Validate method to check if the request data meets the required criteria
//     public (bool Success, string[] Errors) Validate()
//     {
//         var errors = new List<string>();

//         if (OrderItems == null || !OrderItems.Any())
//             errors.Add("Order must contain at least one order item.");

//         if (CustomerLocation <= 0)
//             errors.Add("Customer Location is required");
//         if (OrderItems is not null)
//         {
//             foreach (var item in OrderItems)
//             {
//                 var (itemSuccess, itemErrors) = item.Validate();
//                 if (!itemSuccess)
//                     errors.AddRange(itemErrors);
//             }
//         }

//         return (errors.Count == 0, errors.ToArray());
//     }
// }

// public class OrderItemRequest
// {

//     public int ProductOffer { get; set; }
//     public float Quantity { get; set; }

//     public (bool Success, string[] Errors) Validate()
//     {
//         var errors = new List<string>();

//         if (Quantity <= 0)
//             errors.Add("Quantity must be at least 1.");

//         if (ProductOffer <= 0)
//             errors.Add("ProductOffer is required");

//         return (errors.Count == 0, errors.ToArray());
//     }
// }