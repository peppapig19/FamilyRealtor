using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FamilyRealtor.Models.DomainModels;

namespace FamilyRealtor.Models.SeedData
{
    public class SeedCities : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> entity)
        {
            entity.HasData(
                new City { Id = 1, CountryId = "TR", Name = "Стамбул" },
                new City { Id = 2, CountryId = "TR", Name = "Анталья" },
                new City { Id = 3, CountryId = "AE", Name = "Дубай" },
                new City { Id = 4, CountryId = "AE", Name = "Абу-Даби" },
                new City { Id = 5, CountryId = "EG", Name = "Каир" },
                new City { Id = 6, CountryId = "EG", Name = "Хургада" },
                new City { Id = 7, CountryId = "GE", Name = "Тбилиси" },
                new City { Id = 8, CountryId = "GE", Name = "Батуми" },
                new City { Id = 9, CountryId = "TN", Name = "Тунис" },
                new City { Id = 10, CountryId = "TN", Name = "Сусс" },
                new City { Id = 11, CountryId = "MV", Name = "Мале" },
                new City { Id = 12, CountryId = "MV", Name = "Ари-Атолл" },
                new City { Id = 13, CountryId = "TH", Name = "Бангкок" },
                new City { Id = 14, CountryId = "TH", Name = "Паттайя" },
                new City { Id = 15, CountryId = "ID", Name = "Бали" },
                new City { Id = 16, CountryId = "ID", Name = "Яккарта" },
                new City { Id = 17, CountryId = "GR", Name = "Афины" },
                new City { Id = 18, CountryId = "GR", Name = "Салоники" },
                new City { Id = 19, CountryId = "CY", Name = "Никосия" },
                new City { Id = 20, CountryId = "CY", Name = "Лимассол" },
                new City { Id = 21, CountryId = "BG", Name = "София" },
                new City { Id = 22, CountryId = "BG", Name = "Варна" },
                new City { Id = 23, CountryId = "VN", Name = "Ханой" },
                new City { Id = 24, CountryId = "VN", Name = "Хошимин" },
                new City { Id = 25, CountryId = "IT", Name = "Рим" },
                new City { Id = 26, CountryId = "IT", Name = "Милан" },
                new City { Id = 27, CountryId = "ES", Name = "Барселона" },
                new City { Id = 28, CountryId = "ES", Name = "Мадрид" },
                new City { Id = 29, CountryId = "HR", Name = "Дубровник" },
                new City { Id = 30, CountryId = "HR", Name = "Сплит" },
                new City { Id = 31, CountryId = "ME", Name = "Будва" },
                new City { Id = 32, CountryId = "ME", Name = "Котор" },
                new City { Id = 33, CountryId = "MX", Name = "Канкун" },
                new City { Id = 34, CountryId = "MX", Name = "Мексико" },
                new City { Id = 35, CountryId = "CU", Name = "Гавана" },
                new City { Id = 36, CountryId = "CU", Name = "Варадеро" },
                new City { Id = 37, CountryId = "DO", Name = "Пунта-Кана" },
                new City { Id = 38, CountryId = "DO", Name = "Санто-Доминго" },
                new City { Id = 39, CountryId = "TW", Name = "Тайбэй" },
                new City { Id = 40, CountryId = "TW", Name = "Каошиунг" },
                new City { Id = 41, CountryId = "BR", Name = "Рио-де-Жанейро" },
                new City { Id = 42, CountryId = "BR", Name = "Сан-Паулу" },
                new City { Id = 43, CountryId = "US", Name = "Нью-Йорк" },
                new City { Id = 44, CountryId = "US", Name = "Лос-Анджелес" }
                );
        }
    }
}