using KiloMart.Core.Contracts;
using Dapper;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.DateServices;
using KiloMart.Domain.Orders.DataAccess;

namespace KiloMart.Presentation.Services;

public class OrderCleanupService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly IServiceProvider _serviceProvider;

    public OrderCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Run every minute
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbFactory = scope.ServiceProvider.GetRequiredService<IDbFactory>();
        OrderDeleteService orderDeleteService = new(dbFactory);
        await orderDeleteService.Cancel();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}


public class OrderToDelete
{
    public long Id { get; set; }
}

public class OrderDeleteService
{
    private readonly IDbFactory _dbFactory;

    public OrderDeleteService(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }
    public async Task Cancel()
    {

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        var settings = await Db.GetSystemSettingsByIdAsync(0, connection, transaction);
        if (settings == null)
        {
            return;
        }
        var now = SaudiDateTimeHelper.GetCurrentTime();
        System.Console.WriteLine(now);
        var threshold = now.AddMinutes(-settings.MaxMinutesToCancelOrderWaitingAProvider);
        System.Console.WriteLine(threshold);
        var selectQuery = @"
            SELECT Id FROM [dbo].[Order]
            WHERE Date < @threshold AND OrderStatus = @status";

        var ordersToDelete = await connection.QueryAsync<OrderToDelete>
        (selectQuery, new { threshold, status = OrderStatus.ORDER_PLACED }, transaction);
        var orderIds = ordersToDelete.Select(o => o.Id).ToList();
        // System.Console.WriteLine(orderIds.Count);
        // System.Console.WriteLine("print order ids");
        foreach (var id in orderIds)
        {
            // System.Console.WriteLine(id);
        }
        if (!orderIds.Any()) return;

        // Delete related records first (respecting foreign key constraints)
        // var deleteQueries = new[]
        // {
        //     "DELETE FROM [dbo].[OrderActivity] WHERE [Order] IN @ids",
        //     "DELETE FROM [dbo].[OrderProductOffer] WHERE [Order] IN @ids",
        //     "DELETE FROM [dbo].[OrderProduct] WHERE [Order] IN @ids",
        //     "DELETE FROM [dbo].[OrderDeliveryInformation] WHERE [Order] IN @ids",
        //     "DELETE FROM [dbo].[OrderCustomerInformation] WHERE [Order] IN @ids",
        //     "DELETE FROM [dbo].[OrderProviderInformation] WHERE [Order] IN @ids",
        //     "DELETE FROM [dbo].[OrderDiscountCode] WHERE [Order] IN @ids",
        //     "DELETE FROM [dbo].[Order] WHERE Id IN @ids"
        // };
        var query =
            "UPDATE [dbo].[Order] SET OrderStatus = @status WHERE Id IN @ids"
        ;
        await connection.ExecuteAsync(query, new { ids = orderIds, status = OrderStatus.CANCELED }, transaction);
        foreach (var id in orderIds)
        {
            await OrdersDb.InsertOrderActivityAsync(connection,
                id,
                SaudiDateTimeHelper.GetCurrentTime(),
                (byte)OrderActivityType.CanceledByTheSystemBecauseNoProviderAcceptIt,
                90,
                transaction);
        }

        transaction.Commit();
    }
}
