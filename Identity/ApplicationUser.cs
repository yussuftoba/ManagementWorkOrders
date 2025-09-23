using Microsoft.AspNetCore.Identity;

namespace Identity;

public class ApplicationUser :IdentityUser<Guid>
{
    public string? PersonName {  get; set; }
}
