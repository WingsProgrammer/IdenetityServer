using MsftFramework.Persistence.EfCore.Postgres;
using MsftFramework.Scheduling.Internal;

namespace MS.Services.Identity.Shared.Data;

public class DbContextDesignFactory : DbContextDesignFactoryBase<IdentityContext>
{
    public DbContextDesignFactory() : base("IdentityServiceConnection")
    {
    }
}