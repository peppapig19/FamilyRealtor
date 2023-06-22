using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FamilyRealtor.Models.DomainModels;

namespace FamilyRealtor.Models.SeedData
{
    public class SeedCountries : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> entity)
        {
            entity.HasData(
                new Country { Id = "TR", Name = "Турция" },
                new Country { Id = "AE", Name = "ОАЭ" },
                new Country { Id = "EG", Name = "Египет" },
                new Country { Id = "GE", Name = "Грузия" },
                new Country { Id = "TN", Name = "Тунис" },
                new Country { Id = "MV", Name = "Мальдивы" },
                new Country { Id = "TH", Name = "Таиланд" },
                new Country { Id = "ID", Name = "Индонезия" },
                new Country { Id = "GR", Name = "Греция" },
                new Country { Id = "CY", Name = "Кипр" },
                new Country { Id = "BG", Name = "Болгария" },
                new Country { Id = "VN", Name = "Вьетнам" },
                new Country { Id = "IT", Name = "Италия" },
                new Country { Id = "ES", Name = "Испания" },
                new Country { Id = "HR", Name = "Хорватия" },
                new Country { Id = "ME", Name = "Черногория" },
                new Country { Id = "MX", Name = "Мексика" },
                new Country { Id = "CU", Name = "Куба" },
                new Country { Id = "DO", Name = "Доминикана" },
                new Country { Id = "TW", Name = "Тайвань" },
                new Country { Id = "BR", Name = "Бразилия" },
                new Country { Id = "US", Name = "США" }
            );
        }
    }
}