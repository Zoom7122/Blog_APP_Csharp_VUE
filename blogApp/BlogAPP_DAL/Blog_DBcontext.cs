using blogApp_DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace blogApp_DAL
{
    public class Blog_DBcontext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public DbSet<Article_Tag> Article_Tags { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Reaction> Reactions { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> User { get; set; }

        public Blog_DBcontext(DbContextOptions<Blog_DBcontext> options) : base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasDefaultValue("User");

            modelBuilder.Entity<User>()
                .Property(u => u.Is_active)
                .HasDefaultValue(true);

            modelBuilder.Entity<Reaction>()
                .Property(x => x.Type)
                .HasDefaultValue("like");

            modelBuilder.Entity<Reaction>()
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Article_Tag>()
                .HasKey(x => new { x.TagId, x.ArticleId });

        }
    }
}
