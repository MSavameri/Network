using NetworkInfrastructure.Web.Models;
using Novell.Directory.Ldap;

namespace NetworkInfrastructure.Web.Data.Services
{
    public class UserService : IUserService
    {
        public async Task<bool> Validate(string domain, UserDto user)
        {
			try
			{
				var userDn = $"{user.UserName}@{domain}";

				using var connection = new LdapConnection();

				connection.SecureSocketLayer = false ;

				await connection.ConnectAsync(domain, LdapConnection.DefaultPort);

				await connection.BindAsync(LdapConnection.LdapV3, userDn, user.Password);

				return connection.Bound;
			}
			catch (Exception)
			{
				return false;
			}
        }
    }
}
