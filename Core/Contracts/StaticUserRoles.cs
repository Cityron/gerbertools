namespace GerberBackend.Core.Contracts;

public static class StaticUserRoles
{
    public const string Owner = "Owner";

    public const string Admin = "Admin";

    public const string Manager = "Manager";

    public const string User = "User";



    public const string OwnerAdmin = "Owner,Admin";

    public const string OwnerAdminManager = "Owner,Admin,Manager";

    public const string OwnerAdminManagerUser = "Owner,Admin,Manager,User";
}
