using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FamilyRealtor.Models.DomainModels;

namespace FamilyRealtor.Models.SeedData
{
    public class SeedCategories : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity.HasData(
                new Category { Id = 1, Name = "Квартира" },
                new Category { Id = 2, Name = "Комната" },
                new Category { Id = 3, Name = "Дом" }
            );
        }
    }
}