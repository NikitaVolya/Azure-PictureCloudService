using Microsoft.EntityFrameworkCore;

namespace PictureCloudService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Models.BannedUser> BannedUsers { get; set; }
        public DbSet<Models.Personne> Personnes { get; set; }
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Picture> Pictures { get; set; }
        public DbSet<Models.Collection> Collections { get; set; }
        public DbSet<Models.CollectionPicture> CollectionPictures { get; set; }
        public DbSet<Models.Liked> Likes { get; set; }
        public DbSet<Models.PictureComment> PictureComments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.BannedUser>().
                HasKey(bu => bu.UserId);

            modelBuilder.Entity<Models.BannedUser>()
                .HasOne(bu => bu.User)
                .WithMany()
                .HasForeignKey(bu => bu.UserId);

            modelBuilder.Entity<Models.Admin>()
                .HasKey(a => a.PersonneId);

            modelBuilder.Entity<Models.User>()
                .HasKey(u => u.PersonneId);

            modelBuilder.Entity<Models.Liked>()
                .HasKey(l => new { l.UserId, l.PictureId });

            modelBuilder.Entity<Models.Admin>()
                .HasOne(u => u.Personne)
                .WithOne()
                .HasForeignKey<Models.Admin>(u => u.PersonneId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.User>()
                .HasOne(u => u.Personne)
                .WithOne()
                .HasForeignKey<Models.User>(u => u.PersonneId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.Picture>()
                .HasOne(p => p.User)
                .WithMany(u => u.Pictures)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Models.Collection>()
                .HasOne(c => c.User)
                .WithMany(u => u.Collections)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Models.CollectionPicture>()
                .HasOne(cp => cp.Collection)
                .WithMany()
                .HasForeignKey(cp => cp.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.CollectionPicture>()
                .HasOne(cp => cp.Picture)
                .WithMany()
                .HasForeignKey(cp => cp.PictureId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Models.Liked>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Models.Liked>()
                .HasOne(l => l.Picture)
                .WithMany()
                .HasForeignKey(l => l.PictureId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.PictureComment>()
                .HasOne(pc => pc.User)
                .WithMany()
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Models.PictureComment>()
                .HasOne(pc => pc.Picture)
                .WithMany()
                .HasForeignKey(pc => pc.PictureId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
