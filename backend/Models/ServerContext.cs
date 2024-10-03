using Microsoft.EntityFrameworkCore;
namespace backend.Models
{
    public class ServerContext: DbContext
    {
      public ServerContext(DbContextOptions <ServerContext> options): base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Articles> Articles { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Comments> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.Article)
                .WithMany(a => a.Likes)
                .HasForeignKey(l => l.articles_id)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.user)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.user_id)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Comments>()
            .HasOne(l => l.Article)
            .WithMany(a => a.Comments)
            .HasForeignKey(l => l.articles_id)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comments>()
                .HasOne(l => l.user)
                .WithMany(u => u.Comments)
                .HasForeignKey(l => l.user_id)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Articles>()
                .HasOne(l => l.category)
                .WithMany(u => u.Articles)
                .HasForeignKey(l => l.category_id)
                .OnDelete(DeleteBehavior.Cascade);


        }

    }
}
