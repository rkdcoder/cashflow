using AutoMapper;
using CashFlow.IdentityAndAccess.Application.Roles.Dto;
using CashFlow.IdentityAndAccess.Domain.Entities;

namespace CashFlow.IdentityAndAccess.Application.Roles.Mapping
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, SimpleRoleDto>();
        }
    }
}