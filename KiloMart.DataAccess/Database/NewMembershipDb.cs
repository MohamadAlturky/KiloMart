using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace KiloMart.DataAccess.Database
{
    public static partial class Db
    {
        // Insert a new MembershipUser record
        public static async Task<int> InsertMembershipUserAsync(
            IDbConnection connection,
            string email,
            bool emailConfirmed,
            string passwordHash,
            byte role,
            int party,
            bool isActive,
            byte language,
            bool? isDeleted = null,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[MembershipUser]
                                ([Email], [EmailConfirmed], [PasswordHash], [Role], [Party], [IsActive], [Language], [IsDeleted])
                                VALUES (@Email, @EmailConfirmed, @PasswordHash, @Role, @Party, @IsActive, @Language, @IsDeleted)
                                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                Email = email,
                EmailConfirmed = emailConfirmed,
                PasswordHash = passwordHash,
                Role = role,
                Party = party,
                IsActive = isActive,
                Language = language,
                IsDeleted = isDeleted
            }, transaction);
        }

        // Update an existing MembershipUser record by Id
        public static async Task<bool> UpdateMembershipUserAsync(
            IDbConnection connection,
            int id,
            string? email = null,
            bool? emailConfirmed = null,
            string? passwordHash = null,
            byte? role = null,
            int? party = null,
            bool? isActive = null,
            byte? language = null,
            bool? isDeleted = null,
            IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var updateColumns = new System.Text.StringBuilder();

            if (email != null)
            {
                parameters.Add("@Email", email);
                updateColumns.AppendLine("[Email] = @Email,");
            }
            if (emailConfirmed.HasValue)
            {
                parameters.Add("@EmailConfirmed", emailConfirmed.Value);
                updateColumns.AppendLine("[EmailConfirmed] = @EmailConfirmed,");
            }
            if (passwordHash != null)
            {
                parameters.Add("@PasswordHash", passwordHash);
                updateColumns.AppendLine("[PasswordHash] = @PasswordHash,");
            }
            if (role.HasValue)
            {
                parameters.Add("@Role", role.Value);
                updateColumns.AppendLine("[Role] = @Role,");
            }
            if (party.HasValue)
            {
                parameters.Add("@Party", party.Value);
                updateColumns.AppendLine("[Party] = @Party,");
            }
            if (isActive.HasValue)
            {
                parameters.Add("@IsActive", isActive.Value);
                updateColumns.AppendLine("[IsActive] = @IsActive,");
            }
            if (language.HasValue)
            {
                parameters.Add("@Language", language.Value);
                updateColumns.AppendLine("[Language] = @Language,");
            }
            if (isDeleted.HasValue)
            {
                parameters.Add("@IsDeleted", isDeleted.Value);
                updateColumns.AppendLine("[IsDeleted] = @IsDeleted,");
            }

            if (updateColumns.Length == 0)
                return true; // Nothing to update

            updateColumns.Length -= 1; // Remove the trailing comma

            const string query = @"UPDATE [dbo].[MembershipUser]
                                SET {0}
                                WHERE [Id] = @Id";

            var fullQuery = string.Format(query, updateColumns.ToString());

            var updatedRowsCount = await connection.ExecuteAsync(fullQuery, parameters, transaction);

            return updatedRowsCount > 0;
        }

        // Delete a MembershipUser record by Id
        public static async Task<bool> DeleteMembershipUserAsync(
            IDbConnection connection,
            int id,
            IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[MembershipUser]
                                WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        // Get MembershipUser by Id
        public static async Task<MembershipUser?> GetMembershipUserByIdAsync(
            IDbConnection connection,
            int id,
            IDbTransaction? transaction = null)
        {
            const string query = @"SELECT 
                                [Id], 
                                [Email], 
                                [EmailConfirmed], 
                                [PasswordHash], 
                                [Role], 
                                [Party], 
                                [IsActive], 
                                [Language], 
                                [IsDeleted]
                                FROM [dbo].[MembershipUser]
                                WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<MembershipUser>(query, new
            {
                Id = id
            }, transaction);
        }
        public static async Task<MembershipUser?> GetMembershipUserByPartyAsync(
                    IDbConnection connection,
                    int id,
                    IDbTransaction? transaction = null)
        {
            const string query = @"SELECT 
                                [Id], 
                                [Email], 
                                [EmailConfirmed], 
                                [PasswordHash], 
                                [Role], 
                                [Party], 
                                [IsActive], 
                                [Language], 
                                [IsDeleted]
                                FROM [dbo].[MembershipUser]
                                WHERE [Party] = @Id";

            return await connection.QueryFirstOrDefaultAsync<MembershipUser>(query, new
            {
                Id = id
            }, transaction);
        }

        // Get MembershipUser by Email
        public static async Task<MembershipUser?> GetMembershipUserByEmailAsync(
            IDbConnection connection,
            string email,
            IDbTransaction? transaction = null)
        {
            const string query = @"SELECT 
                                [Id], 
                                [Email], 
                                [EmailConfirmed], 
                                [PasswordHash], 
                                [Role], 
                                [Party], 
                                [IsActive], 
                                [Language], 
                                [IsDeleted]
                                FROM [dbo].[MembershipUser]
                                WHERE [Email] = @Email";

            return await connection.QueryFirstOrDefaultAsync<MembershipUser>(query, new
            {
                Email = email
            }, transaction);
        }

        // Get all MembershipUsers with optional filters
        public static async Task<IEnumerable<MembershipUser>> GetMembershipUsersAsync(
            IDbConnection connection,
            int? party = null,
            byte? role = null,
            bool? isActive = null,
            bool? isDeleted = null,
            int pageNumber = 1,
            int pageSize = 10,
            IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();

            var whereClauses = new System.Text.StringBuilder();

            if (party.HasValue)
            {
                whereClauses.AppendLine("[Party] = @Party");
                parameters.Add("@Party", party.Value);
            }
            if (role.HasValue)
            {
                if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
                whereClauses.AppendLine("[Role] = @Role");
                parameters.Add("@Role", role.Value);
            }
            if (isActive.HasValue)
            {
                if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
                whereClauses.AppendLine("[IsActive] = @IsActive");
                parameters.Add("@IsActive", isActive.Value);
            }
            if (isDeleted.HasValue)
            {
                if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
                whereClauses.AppendLine("[IsDeleted] = @IsDeleted");
                parameters.Add("@IsDeleted", isDeleted.Value);
            }

            var whereClause = whereClauses.Length > 0 ? " WHERE " + whereClauses.ToString() : "";

            const string query = @"SELECT 
                                [Id], 
                                [Email], 
                                [EmailConfirmed], 
                                [PasswordHash], 
                                [Role], 
                                [Party], 
                                [IsActive], 
                                [Language], 
                                [IsDeleted]
                                FROM [dbo].[MembershipUser]
                                {0}
                                ORDER BY [Id] DESC
                                OFFSET @Offset ROWS
                                FETCH NEXT @PageSize ROWS ONLY;";

            var offset = (pageNumber - 1) * pageSize;
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", pageSize);

            var fullQuery = string.Format(query, whereClause);

            return await connection.QueryAsync<MembershipUser>(fullQuery, parameters, transaction);
        }

        // Get count of MembershipUsers with optional filters
        public static async Task<int> GetMembershipUsersCountAsync(
            IDbConnection connection,
            int? party = null,
            byte? role = null,
            bool? isActive = null,
            bool? isDeleted = null,
            IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();

            var whereClauses = new System.Text.StringBuilder();

            if (party.HasValue)
            {
                whereClauses.AppendLine("[Party] = @Party");
                parameters.Add("@Party", party.Value);
            }
            if (role.HasValue)
            {
                if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
                whereClauses.AppendLine("[Role] = @Role");
                parameters.Add("@Role", role.Value);
            }
            if (isActive.HasValue)
            {
                if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
                whereClauses.AppendLine("[IsActive] = @IsActive");
                parameters.Add("@IsActive", isActive.Value);
            }
            if (isDeleted.HasValue)
            {
                if (whereClauses.Length > 0) whereClauses.AppendLine(" AND ");
                whereClauses.AppendLine("[IsDeleted] = @IsDeleted");
                parameters.Add("@IsDeleted", isDeleted.Value);
            }

            var whereClause = whereClauses.Length > 0 ? " WHERE " + whereClauses.ToString() : "";

            const string query = @"SELECT COUNT([Id])
                                FROM [dbo].[MembershipUser]
                                {0};";

            var fullQuery = string.Format(query, whereClause);

            return await connection.QueryFirstOrDefaultAsync<int>(fullQuery, parameters, transaction);
        }
    }
}

public class MembershipUser
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = null!;
    public byte Role { get; set; }
    public int Party { get; set; }
    public bool IsActive { get; set; }
    public byte Language { get; set; }
    public bool? IsDeleted { get; set; }
}