using System.Text.RegularExpressions;
using Clipo.Domain.AggregatesModel.Base;
using Clipo.Domain.AggregatesModel.VideoAggregate;
using Microsoft.EntityFrameworkCore;

namespace Clipo.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }

        public DbSet<VideoStatus> VideoStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("videoStatus");

            foreach(Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                string tableName = ToKebabCase(entity.ClrType.Name);
                modelBuilder.Entity(entity.ClrType).ToTable(tableName);

                Microsoft.EntityFrameworkCore.Metadata.IMutableProperty? idProp = entity.FindProperty("Id");
                if(idProp != null &&
                    (idProp.ClrType == typeof(int) || idProp.ClrType == typeof(long)))
                {
                    modelBuilder.Entity(entity.ClrType)
                        .Property("Id")
                        .ValueGeneratedOnAdd();
                }
            }
        }


        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            TouchUpdatedAt();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess = true)
        {
            TouchUpdatedAt();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void TouchUpdatedAt()
        {
            DateTime utc = DateTime.UtcNow;
            ChangeTracker.Entries<BaseModel>()
                .Where(e => e.State is EntityState.Modified)
                .ToList()
                .ForEach(e => e.Entity.UpdatedAt = utc);
        }

        private static readonly Regex _toKebab = new(@"(?<!^)(?=[A-Z])", RegexOptions.Compiled);

        private static string ToKebabCase(string name) =>
            _toKebab.Replace(name, "-").ToLowerInvariant();
    }

}
