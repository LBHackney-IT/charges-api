using ChargeApi.V1.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChargeApi.V1.Infrastructure
{

    public class ChargeContext : DbContext
    {
        //TODO: rename DatabaseContext to reflect the data source it is representing. eg. MosaicContext.
        //Guidance on the context class can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/DatabaseContext
        public ChargeContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ChargeDbEntity> ChargeEntities { get; set; }
    }
}
