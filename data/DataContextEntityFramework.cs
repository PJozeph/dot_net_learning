using DOT_NET_API;
using dot_net_learning_api.Model;
using Microsoft.EntityFrameworkCore;

namespace dot_net_learning_api.Data{
    public class DataContextEntityFramework : DbContext
    {
        private readonly IConfiguration _configuration;

        public DataContextEntityFramework(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSalary> UserSalarie { get; set; }
        public virtual DbSet<UserJobinfo> UserJobinfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"),
                optionsBuilder => optionsBuilder.EnableRetryOnFailure()
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users", "TutorialAppSchema").HasKey(u => u.UserId);
            modelBuilder.Entity<UserSalary>().HasKey(u => u.UserId);
            modelBuilder.Entity<UserJobinfo>().HasKey(u => u.UserId);
        }

    }
}