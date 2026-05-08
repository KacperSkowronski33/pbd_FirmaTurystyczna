using BookingApp.Shared.Models;
namespace BookingApp.Api.Data
{
    public static class DbSeeder
    {
        public static void Seed(BookingDbContext context)
        {
            context.Database.EnsureCreated();
            if (!context.Role.Any())
            {
                context.Role.AddRange(
                    new Rola { TypRoli = TypRoli.Brak },
                    new Rola { TypRoli = TypRoli.Klient },
                    new Rola { TypRoli = TypRoli.Pracownik },
                    new Rola { TypRoli = TypRoli.Administrator }
                    );
                context.SaveChanges();
            }
            if(!context.Pracownicy.Any())
            {
                var admin = context.Role.First(r => r.TypRoli == TypRoli.Administrator);
                context.Pracownicy.Add(new Pracownik
                {
                    Imie = "Główny",
                    Nazwisko = "Admin",
                    Email = "admin",
                    Haslo = BCrypt.Net.BCrypt.HashPassword("123"),
                    RolaId = admin.Id,

                });
                context.SaveChanges();
            }
        }
    }
}
