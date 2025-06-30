﻿namespace CashFlow.IdentityAndAccess.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}