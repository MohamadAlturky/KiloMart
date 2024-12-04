using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
//  CREATE TABLE [dbo].[Notification](
// 	[Id] [bigint] IDENTITY(1,1) NOT NULL,
// 	[Title] [varchar](100) NOT NULL,
// 	[Content] [varchar](2000) NOT NULL,
// 	[Date] [datetime] NOT NULL,
// 	[ForParty] [int] NOT NULL,
// 	[JsonPayLoad] [varchar](2000) NOT NULL,
// 	[IsRead] [bit] NOT NULL)
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertNotificationAsync(IDbConnection connection,
        string title,
        string content,
        DateTime date,
        int forParty,
        string jsonPayLoad,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Notification]
                            ([Title], [Content], [Date], [ForParty], [JsonPayLoad], [IsRead])
                            VALUES (@Title, @Content, @Date, @ForParty, @JsonPayLoad, 0)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Title = title,
            Content = content,
            Date = date,
            ForParty = forParty,
            JsonPayLoad = jsonPayLoad
        }, transaction);
    }

    public static async Task<bool> UpdateNotificationAsync(IDbConnection connection,
        long id,
        string title,
        string content,
        DateTime date,
        int forParty,
        string jsonPayLoad,
        bool isRead,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Notification]
                                SET 
                                [Title] = @Title,
                                [Content] = @Content,
                                [Date] = @Date,
                                [ForParty] = @ForParty,
                                [JsonPayLoad] = @JsonPayLoad,
                                [IsRead] = @IsRead
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Title = title,
            Content = content,
            Date = date,
            ForParty = forParty,
            JsonPayLoad = jsonPayLoad,
            IsRead = isRead
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteNotificationAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Notification]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Notification?> GetNotificationByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Title], 
                            [Content], 
                            [Date], 
                            [ForParty], 
                            [JsonPayLoad], 
                            [IsRead]
                            FROM [dbo].[Notification]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Notification>(query, new
        {
            Id = id
        });
    }

    public static async Task<int> MarkNotificationsAsReadAsync(IDbConnection connection,
    List<long> idList,
    int party)
    {
        const string query = @"UPDATE [dbo].[Notification]
                           SET [IsRead] = 1
                           WHERE [Id] IN @IdList
                           AND [ForParty] = @Party";

        return await connection.ExecuteAsync(query, new
        {
            IdList = idList,
            Party = party
        });
    }
    public static async Task<IEnumerable<Notification>> GetTopUnreadNotificationsAsync(
        IDbConnection connection,
        int party,
        int topCount)
    {
        const string query = @"SELECT TOP (@TopCount)
                            [Id], 
                            [Title], 
                            [Content], 
                            [Date], 
                            [ForParty], 
                            [JsonPayLoad], 
                            [IsRead]
                           FROM [dbo].[Notification]
                           WHERE [ForParty] = @Party
                           ORDER BY [Id] DESC";

        return await connection.QueryAsync<Notification>(query, new
        {
            Party = party,
            TopCount = topCount
        });
    }
    public static async Task<(IEnumerable<Notification> Notifications, int TotalCount)> GetNotificationsPagedAsync(
        IDbConnection connection,
        int party,
        int pageNumber,
        int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;

        var whereClause = "[ForParty] = @Party";

        var query = $@"
        SELECT COUNT(*) FROM [dbo].[Notification] WHERE {whereClause};

        SELECT [Id], [Title], [Content], [Date], [ForParty], [JsonPayLoad], [IsRead]
        FROM [dbo].[Notification]
        WHERE {whereClause}
        ORDER BY [Id] DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";

        using var multi = await connection.QueryMultipleAsync(query, new
        {
            Party = party,
            Offset = offset,
            PageSize = pageSize
        });

        var totalCount = await multi.ReadSingleAsync<int>();
        var notifications = await multi.ReadAsync<Notification>();

        return (notifications, totalCount);
    }
}

public class Notification
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime Date { get; set; }
    public int ForParty { get; set; }
    public string JsonPayLoad { get; set; } = null!;
    public bool IsRead { get; set; }
}