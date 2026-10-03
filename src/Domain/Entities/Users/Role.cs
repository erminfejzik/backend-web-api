namespace Domain.Entities.Users
{
    public sealed class Role(int id, string name)
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string User = "User";
        public const int SuperAdminId = 1;
        public const int AdminId = 2;
        public const int UserId = 3;

        public int Id { get; init; } = id;
        public string Name { get; init; } = name;
    }
}
