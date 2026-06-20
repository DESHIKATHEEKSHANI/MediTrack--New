using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MediTrack.Core.Data;

public class MediTrackDbContextFactory : IDesignTimeDbContextFactory<MediTrackDbContext>
{
    public MediTrackDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MediTrackDbContext>();
        optionsBuilder.UseMySql(
            "server=localhost;database=meditrack-db;user=root;password=1234;",
            ServerVersion.AutoDetect("server=localhost;database=meditrack-db;user=root;password=1234;"));

        return new MediTrackDbContext(optionsBuilder.Options);
    }
}
