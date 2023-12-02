using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS.Services.Identitty.Domain.Shared.Models;
using MsftFramework.Core.Persistence.EfCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Services.Identitty.Domain.Shared.Data.EntityConfigurations
{
    public class VerifyCodeConfiguration : IEntityTypeConfiguration<VerifyCode>
    {
        public void Configure(EntityTypeBuilder<VerifyCode> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Id)
                .IsUnique();

            builder.Property(x => x.VerifyCodeValue)
                .HasMaxLength(5);

            builder.Property(x => x.MobileNumber)
                .HasMaxLength(11);

            builder.Property(x => x.Used)
                .HasColumnType("boolean")
                .HasDefaultValueSql("false");
        }
    }
}
