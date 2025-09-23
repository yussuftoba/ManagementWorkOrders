using Identity;

namespace IServices
{
    public interface IJWTService
    {
        Task<object> CreateJwtToken(ApplicationUser user);
    }
}
