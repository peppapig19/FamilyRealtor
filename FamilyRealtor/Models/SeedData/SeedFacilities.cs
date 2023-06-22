using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FamilyRealtor.Models.DomainModels;

namespace FamilyRealtor.Models.SeedData
{
    public class SeedFacilities : IEntityTypeConfiguration<Facility>
    {
        public void Configure(EntityTypeBuilder<Facility> entity)
        {
            entity.HasData(
                new Facility { Id = 1, Name = "Интернет", IconClass = "fa fa-wifi" },
                new Facility { Id = 2, Name = "Телевизор", IconClass = "fa fa-tv" },
                new Facility { Id = 3, Name = "Вид на море", IconClass = "fa fa-water" },
                new Facility { Id = 4, Name = "Питание", IconClass = "fa fa-burger" },
                new Facility { Id = 5, Name = "Курение", IconClass = "fa fa-smoking" },
                new Facility { Id = 6, Name = "Вечеринки", IconClass = "fa fa-champagne-glasses" },
                new Facility { Id = 7, Name = "Питомцы разрешены", IconClass = "fa fa-cat" }
            );
        }
    }
}