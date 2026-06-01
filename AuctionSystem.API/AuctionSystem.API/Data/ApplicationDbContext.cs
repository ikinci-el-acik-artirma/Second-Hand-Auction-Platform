using AuctionSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<DevicePhoto> DevicePhotos => Set<DevicePhoto>();

        public DbSet<AuctionItem> AuctionItems => Set<AuctionItem>();

        public DbSet<Bid> Bids => Set<Bid>();
    }
}
