using Microsoft.EntityFrameworkCore;
using Курсач_1.Models;

namespace Курсач_1.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasMany(e => e.Events).WithOne(e => e.User).HasForeignKey(e => e.UserId);

        }
    }
}
