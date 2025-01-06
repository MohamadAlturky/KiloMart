using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql
{
    public static string PartyMembershipUserSql => @$"
    SELECT 
            m.*,
            p.*
        FROM ({Sql.MembershipUserSql}) m
        INNER JOIN ({Sql.PartySql}) p 
        ON p.Id = m.Party";

    public static DbQuery<PartyMembershipUserSqlResponse> PartyMembershipUserSqlQuery(
        int? membershipUserId = null,
        string? membershipUserEmail = null,
        bool? membershipUserEmailConfirmed = null,
        int? membershipUserRole = null,
        int? membershipUserParty = null,
        bool? membershipUserIsActive = null,
        bool? membershipUserIsDeleted = null,
        byte? membershipUserLanguage = null,
        int? partyId = null,
        bool? partyIsActive = null,
        string? partyDisplayName = null,
        long? offset = null,
        int? take = null)
    {
        return new DbQuery<PartyMembershipUserSqlResponse>(PartyMembershipUserSql)
                        // member ship filtering
                        .Where("m.[Id] = @MembershipUserId", new { MembershipUserId = membershipUserId })
                        .Where("m.[Email] = @MembershipUserEmail", new { MembershipUserEmail = membershipUserEmail })
                        .Where("m.[EmailConfirmed] = @MembershipUserEmailConfirmed", new { MembershipUserEmailConfirmed = membershipUserEmailConfirmed })
                        .Where("m.[Role] = @MembershipUserRole", new { MembershipUserRole = membershipUserRole })
                        .Where("m.[Party] = @MembershipUserParty", new { MembershipUserParty = membershipUserParty })
                        .Where("m.[IsActive] = @MembershipUserIsActive", new { MembershipUserIsActive = membershipUserIsActive })
                        .Where("m.[Language] = @MembershipUserLanguage", new { MembershipUserLanguage = membershipUserLanguage })
                        .Where("m.[IsDeleted] = @MembershipUserIsDeleted", new { MembershipUserIsDeleted = membershipUserIsDeleted })

                        // party filtering
                        .Where("p.[Id] = @PartyId", new { PartyId = partyId })
                        .Where("p.[IsActive] = @PartyIsActive", new { PartyIsActive = partyIsActive })
                        .Where("p.[DisplayName] = @PartyDisplayName", new { PartyDisplayName = partyDisplayName })

                        .Page(offset, take);
    }
}
public class PartyMembershipUserSqlResponse
{
    public MembershipUserSqlResponse MembershipUser { get; set; } = null!;
    public PartySqlResponse Party { get; set; } = null!;
}