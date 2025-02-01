using KiloMart.DataAccess.EFCore.Data;
using KiloMart.DataAccess.EFCore.DTOs;
using KiloMart.DataAccess.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KiloMart.DataAccess.EFCore.Services;
public static class OrderService
{
    public static async Task<Order?> GetOrderByExpressionAsync(
     this KilomartDbContext context,
     Expression<Func<Order, bool>> expression,
     bool asNoTracking = true)
    {
        var query = context.Orders
            .Where(expression)
            .AsSplitQuery()
            // Include all navigation properties in a single chain
            .Include(o => o.OrderStatusNavigation)
            .Include(o => o.PaymentTypeNavigation)
            .Include(o => o.OrderProducts)
                .ThenInclude(opo => opo.ProductNavigation)
            .Include(o => o.OrderProductOffers)
                .ThenInclude(opo => opo.ProductOfferNavigation)
            .Include(o => o.OrderCustomerInformations)
                .ThenInclude(oci => oci.CustomerNavigation)
                    .ThenInclude(d => d.PartyNavigation)
                        .ThenInclude(e => e.MembershipUsers)
            .Include(o => o.OrderCustomerInformations)
                .ThenInclude(oci => oci.LocationNavigation)
            .Include(o => o.OrderDeliveryInformations)
                .ThenInclude(odi => odi.DeliveryNavigation)
                    .ThenInclude(d => d.PartyNavigation)
                        .ThenInclude(e => e.MembershipUsers)
            .Include(o => o.OrderProviderInformations)
                .ThenInclude(opi => opi.ProviderNavigation)
                    .ThenInclude(d => d.PartyNavigation)
                        .ThenInclude(e => e.MembershipUsers)

            .Include(o => o.OrderProviderInformations)
                .ThenInclude(opi => opi.LocationNavigation)
            .Include(o => o.OrderActivities)
            .Include(o => o.SystemActivities)
            .Include(o => o.DiscountCodes);

        return await (asNoTracking ? query.AsNoTracking().FirstOrDefaultAsync() : query.FirstOrDefaultAsync());
    }

    public static async Task<OrderDto?> GetOrderDtoByExpressionAsync(
            this KilomartDbContext context,
            Expression<Func<Order, bool>> expression,
            bool asNoTracking = true)
    {
        var query = context.Orders
            .Where(expression)
            .AsSplitQuery()
            // Include all navigation properties in a single chain
            .Include(o => o.OrderStatusNavigation)
            .Include(o => o.PaymentTypeNavigation)
            .Include(o => o.OrderProducts)
                .ThenInclude(opo => opo.ProductNavigation)
            .Include(o => o.OrderProductOffers)
                .ThenInclude(opo => opo.ProductOfferNavigation)
            .Include(o => o.OrderCustomerInformations)
                .ThenInclude(oci => oci.CustomerNavigation)
                    .ThenInclude(d => d.PartyNavigation)
                        .ThenInclude(e => e.MembershipUsers)
            .Include(o => o.OrderCustomerInformations)
                .ThenInclude(oci => oci.LocationNavigation)
            .Include(o => o.OrderDeliveryInformations)
                .ThenInclude(odi => odi.DeliveryNavigation)
                    .ThenInclude(d => d.PartyNavigation)
                        .ThenInclude(e => e.MembershipUsers)
            .Include(o => o.OrderProviderInformations)
                .ThenInclude(opi => opi.ProviderNavigation)
                    .ThenInclude(d => d.PartyNavigation)
                        .ThenInclude(e => e.MembershipUsers)

            .Include(o => o.OrderProviderInformations)
                .ThenInclude(opi => opi.LocationNavigation)
            .Include(o => o.OrderActivities)
            .Include(o => o.SystemActivities)
            .Include(o => o.DiscountCodes);

        var order = await (asNoTracking ? query.AsNoTracking().FirstOrDefaultAsync() : query.FirstOrDefaultAsync());

        return order is not null ? order?.ToOrderDto() : null;
    }

    private static OrderDto ToOrderDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderStatus = order.OrderStatusNavigation?.Name, // Null check
            TotalPrice = order.TotalPrice,
            TransactionId = order.TransactionId,
            Date = order.Date,
            PaymentType = order.PaymentTypeNavigation?.Name, // Null check
            IsPaid = order.IsPaid,
            DeliveryFee = order.DeliveryFee,
            SystemFee = order.SystemFee,
            ItemsPrice = order.ItemsPrice,
            SpecialRequest = order.SpecialRequest,
            OrderProducts = order.OrderProducts?.Select(op => new OrderProductDto
            {
                Product = op.ProductNavigation != null ? new ProductDto // Check ProductNavigation
                {
                    Id = op.ProductNavigation.Id,
                    Name = op.ProductNavigation.Name,
                    ImageUrl = op.ProductNavigation.ImageUrl,
                    MeasurementUnit = op.ProductNavigation.MeasurementUnit
                } : null, // Handle null case
                Quantity = op.Quantity
            }).ToList(),
            OrderProductOffers = order.OrderProductOffers?.Select(opo => new OrderProductOfferDto
            {
                ProductOffer = opo.ProductOfferNavigation != null ? new ProductOfferDto // Check ProductOfferNavigation
                {
                    Id = opo.ProductOfferNavigation.Id,
                    Price = opo.ProductOfferNavigation.Price,
                    OffPercentage = opo.ProductOfferNavigation.OffPercentage,
                    FromDate = opo.ProductOfferNavigation.FromDate,
                    ToDate = opo.ProductOfferNavigation.ToDate,
                    Quantity = opo.ProductOfferNavigation.Quantity,
                    Product = opo.ProductOfferNavigation.ProductNavigation != null ? new ProductDto // Check ProductNavigation
                    {
                        Id = opo.ProductOfferNavigation.ProductNavigation.Id,
                        Name = opo.ProductOfferNavigation.ProductNavigation.Name,
                        ImageUrl = opo.ProductOfferNavigation.ProductNavigation.ImageUrl,
                        MeasurementUnit = opo.ProductOfferNavigation.ProductNavigation.MeasurementUnit
                    } : null,
                    Provider = opo.ProductOfferNavigation.ProviderNavigation != null ? new ProviderDto // Check ProviderNavigation
                    {
                        PartyId = opo.ProductOfferNavigation.ProviderNavigation.Party,
                        DisplayName = opo.ProductOfferNavigation.ProviderNavigation.PartyNavigation?.DisplayName,
                        Email = opo.ProductOfferNavigation.ProviderNavigation.PartyNavigation?.MembershipUsers?.FirstOrDefault()?.Email
                    } : null
                } : null,
                UnitPrice = opo.UnitPrice,
                Quantity = opo.Quantity,
                DiscountCodes = opo.DiscountCodes?.Select(dc => new DiscountCodeDto
                {
                    Id = dc.Id,
                    Code = dc.Code,
                    Value = dc.Value,
                    Description = dc.Description,
                    StartDate = dc.StartDate,
                    EndDate = dc.EndDate,
                    DiscountType = dc.DiscountTypeNavigation?.Name // Null check
                }).ToList()
            }).ToList(),
            OrderCustomerInformations = order.OrderCustomerInformations?.Select(oci => new OrderCustomerInformationDto
            {
                Customer = oci.CustomerNavigation != null ? new CustomerDto // Check CustomerNavigation
                {
                    PartyId = oci.CustomerNavigation.Party,
                    DisplayName = oci.CustomerNavigation.PartyNavigation?.DisplayName,
                    Email = oci.CustomerNavigation.PartyNavigation?.MembershipUsers?.FirstOrDefault()?.Email
                } : null,
                Location = oci.LocationNavigation != null ? new LocationDto // Check LocationNavigation
                {
                    Id = oci.LocationNavigation.Id,
                    Address = oci.LocationNavigation.Name,
                    Lat = oci.LocationNavigation.Latitude,
                    Lng = oci.LocationNavigation.Longitude
                } : null
            }).ToList(),
            OrderDeliveryInformations = order.OrderDeliveryInformations?.Select(odi => new OrderDeliveryInformationDto
            {
                Delivery = odi.DeliveryNavigation != null ? new DeliveryDto // Check DeliveryNavigation
                {
                    PartyId = odi.DeliveryNavigation.Party,
                    DisplayName = odi.DeliveryNavigation.PartyNavigation?.DisplayName,
                    Email = odi.DeliveryNavigation.PartyNavigation?.MembershipUsers?.FirstOrDefault()?.Email
                } : null
            }).ToList(),
            OrderProviderInformations = order.OrderProviderInformations?.Select(opi => new OrderProviderInformationDto
            {
                Provider = opi.ProviderNavigation != null ? new ProviderDto // Check ProviderNavigation
                {
                    PartyId = opi.ProviderNavigation.Party,
                    DisplayName = opi.ProviderNavigation.PartyNavigation?.DisplayName
                } : null,
                Location = opi.LocationNavigation != null ? new LocationDto // Check LocationNavigation
                {
                    Id = opi.LocationNavigation.Id,
                    Address = opi.LocationNavigation.Name,
                    Lat = opi.LocationNavigation.Latitude,
                    Lng = opi.LocationNavigation.Longitude
                } : null
            }).ToList(),
            OrderActivities = order.OrderActivities?.Select(oa => new OrderActivityDto
            {
                Date = oa.Date,
                OrderActivityType = oa.OrderActivityTypeNavigation?.Name, // Null check
                OperatedBy = oa.OperatedByNavigation?.DisplayName // Null check
            }).ToList(),
            DiscountCodes = order.DiscountCodes?.Select(dc => new DiscountCodeDto // Null check added
            {
                Id = dc.Id,
                Code = dc.Code,
                Value = dc.Value,
                Description = dc.Description,
                StartDate = dc.StartDate,
                EndDate = dc.EndDate,
                DiscountType = dc.DiscountTypeNavigation?.Name // Null check
            }).ToList() ?? new List<DiscountCodeDto>() // Handle null case
        };
    }
}