namespace CashFlow.Shared.Utilities
{
    public static class UserHierarchyUtils
    {
        private static readonly Dictionary<string, int> RoleLevels = new()
        {
            { "USER", 1 },
            { "MANAGER", 2 },
            { "ADMIN", 3 }
        };

        // Retorna o nível de uma única role
        public static int GetRoleLevel(string role)
        {
            return RoleLevels.TryGetValue(role, out var level) ? level : 0;
        }

        // Retorna o maior nível entre múltiplas roles
        public static int GetHighestRoleLevel(IEnumerable<string> roles)
        {
            if (roles == null) return 0;
            return roles.Select(GetRoleLevel).DefaultIfEmpty(0).Max();
        }

        // Verifica se o usuário tem pelo menos o nível requerido
        public static bool HasRequiredRole(IEnumerable<string> userRoles, string requiredRole)
        {
            var requiredLevel = GetRoleLevel(requiredRole);
            var userLevel = GetHighestRoleLevel(userRoles);
            return userLevel >= requiredLevel;
        }
    }
}