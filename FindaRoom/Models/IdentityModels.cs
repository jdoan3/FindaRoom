using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace FindaRoom.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public virtual ICollection<ApplicationUser> Likes { get; set; }
        public virtual ICollection<ApplicationUser> Matches { get; set; }
        public virtual ICollection<FbInfo> FbInfo { get; set; }
        public virtual ICollection<Questions> Questions { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Likes)
                .WithMany()
                .Map(conf => conf.ToTable("Likes"));

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Matches)
                .WithMany()
                .Map(conf => conf.ToTable("Matches"));

            base.OnModelCreating(modelBuilder);
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<FindaRoom.Models.FbInfo> FbInfoes { get; set; }

        public System.Data.Entity.DbSet<FindaRoom.Models.Questions> Questions { get; set; }

        public System.Data.Entity.DbSet<FindaRoom.Models.Message> Messages { get; set; }
    }
}