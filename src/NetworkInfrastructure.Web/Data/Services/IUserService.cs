using Microsoft.AspNetCore.Mvc;
using NetworkInfrastructure.Web.Models;

namespace NetworkInfrastructure.Web.Data.Services
{
    public interface IUserService
    {
        Task<bool> Validate(string domain, UserDto user);
    }
}
