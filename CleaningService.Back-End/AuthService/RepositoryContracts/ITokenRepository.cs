using Microsoft.AspNetCore.Identity;

namespace RepositoryContracts
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
