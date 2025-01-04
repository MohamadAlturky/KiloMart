using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string PartyCardMembershipSql => @$"
    {SELECT}
        p.[Id] {AS} PartyId,
        p.[DisplayName] {AS} PartyDisplayName,
        p.[IsActive] {AS} PartyIsActive,

        m.[Id] {AS} MembershipUserId,
        m.[Email] {AS} MembershipUserEmail,
        m.[EmailConfirmed] {AS} MembershipUserEmailConfirmed,
        m.[Role] {AS} MembershipUserRole,
        m.[Language] {AS} MembershipUserLanguage,
        m.[IsActive] {AS} MembershipUserIsActive,

        c.[Id] {AS} CardId,
        c.[HolderName] {AS} CardHolderName,
        c.[Number] {AS} CardNumber,
        c.[SecurityCode] {AS} CardSecurityCode,
        c.[ExpireDate] {AS} CardExpireDate,
        c.[IsActive] {AS} CardIsActive
    FROM 
        ({Sql.PartySql}) p
        {INNER} {JOIN} ({Sql.MembershipUserSql}) m {ON} p.[Id] = m.[Party]
        {INNER} {JOIN} ({Sql.CardSql}) c {ON} c.[Customer] = p.[Id]";

    public static DbQuery<PartyCardMembershipResponse> PartyCardMembershipSqlQuery 
    => new(PartyCardMembershipSql);
}

public class PartyCardMembershipResponse
{
    public int PartyId { get; set; }
    public string PartyDisplayName { get; set; } = null!;
    public bool PartyIsActive { get; set; }


    public int MembershipUserId { get; set; }
    public string MembershipUserEmail { get; set; } = null!;
    public bool MembershipUserEmailConfirmed { get; set; }
    public byte MembershipUserRole { get; set; }
    public byte MembershipUserLanguage { get; set; }
    public bool MembershipUserIsActive { get; set; }


    public int CardId { get; set; }
    public string CardHolderName { get; set; } = null!;
    public string CardNumber { get; set; } = null!;
    public string CardSecurityCode { get; set; } = null!;
    public DateTime CardExpireDate { get; set; }
    public bool CardIsActive { get; set; }
}



public partial class Sql 
{
    public static string AS {get;set;} = "AS";
    public static string INNER {get;set;} = "INNER";
    public static string JOIN {get;set;} = "JOIN";
    public static string ON {get;set;} = "ON";
    public static string SELECT {get;set;} = "SELECT";
}
