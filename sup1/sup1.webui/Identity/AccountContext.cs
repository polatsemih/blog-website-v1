using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace sup1.webui.Identity
{
    public class AccountContext : IdentityDbContext<User>
    {
        public AccountContext(DbContextOptions<AccountContext> options) : base(options)
        {
            
        }
    }
}