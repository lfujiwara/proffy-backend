namespace ProffyBackend.Models
{
    public static class AuthorizationPolicies
    {
        public const string SuperAdmin = Role.SuperAdmin;
        public const string Admin = Role.Admin;
        public const string User = Role.User;
        public const string RefreshToken = "RefreshToken";
    }
}