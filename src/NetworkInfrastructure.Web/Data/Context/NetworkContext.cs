using Microsoft.EntityFrameworkCore;
using NetworkInfrastructure.Web.Data.Entities;

namespace NetworkInfrastructure.Web.Data.Context
{
    public class NetworkContext : DbContext
    {
        public NetworkContext(DbContextOptions<NetworkContext> option) : base(option) { }

        public DbSet<NetworkAsset> NetworkAssets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<NetworkAsset>()
                .ToTable(nameof(NetworkAsset))
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x=>x.ServerName)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("ServerName");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x=>x.Ip)
                .IsRequired()
                .HasMaxLength(15)
                .HasColumnName("Ip");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.BackupType)
                .IsRequired()
                .HasColumnName("BackupType");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.Monitoring)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("Monitoring");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.Limitation)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("Limitation");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.ServiceOwner)
                .IsRequired()
                .HasColumnName("ServiceOwner");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.OsType)
                .IsRequired()
                .HasColumnName("OsType");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.LocationName)
                .IsRequired()
                .HasColumnName("LocationName");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.Description)
                .IsRequired()
                .HasColumnName("Description");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.FirewallWindows)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("FirewallWindows");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.NetBios)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("NetBios");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.FolderShare)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("FolderShare");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.Mcafee)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("Mcafee");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.Activation)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("Activation");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.LastUpdate)
                .IsRequired()
                .HasColumnName("LastUpdate");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.SplunkAgent)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("SplunkAgent");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.GroupPolicy)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("GroupPolicy");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.AccessLists)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("AccessLists");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.FtdDatacenter)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("FtdDatacenter");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.FtdInterne)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("FtdInterne");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.Sophos)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("Sophos");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.Waf)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("Waf");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.Asr)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("Asr");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.ServerName)
                .IsRequired()
                .HasColumnName("ServerName");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.Dns)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("Dns");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.DateCreated)
                .IsRequired()
                .HasDefaultValue(DateTime.Now)
                .HasColumnName("DateCreated");

            modelBuilder
                .Entity<NetworkAsset>()
                .Property(x => x.UserName)
                .IsRequired()
                .HasColumnName("UserName");

            base.OnModelCreating(modelBuilder);
        }
    }
}
