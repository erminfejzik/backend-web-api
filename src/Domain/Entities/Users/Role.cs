namespace Domain.Entities.Users
{
    public sealed class Role
    {
        public int Id { get; private init; }
        public string Name { get; private init; }

        private Role(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public const int SuperAdminId = 1;
        public const int AdminId = 2;
        public const int UserId = 3;

        public static Role SuperAdminRole() => new(SuperAdminId, "SuperAdmin");
        public static Role AdminRole() => new(AdminId, "Admin");
        public static Role UserRole() => new(UserId, "User");
    }
}
