﻿namespace CashFlow.IdentityAndAccess.Application.Roles.Dto
{
    public class SimpleRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}