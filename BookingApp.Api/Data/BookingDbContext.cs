using Microsoft.EntityFrameworkCore;
using BookingApp.Shared.Models;

namespace BookingApp.Api.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<Kraj> Kraje { get; set; }
        public DbSet<Miejscowosc> Miejscowsci {  get; set; }
        public DbSet<Hotel> Hotele { get; set; }
        public DbSet<RodzajPokoju> RodzajePokojow {  get; set; }
        public DbSet<ZdjecieHotelu> ZdjeciaHotelow { get; set; }
        public DbSet<ZdjeciePokoju> ZdjeciaPokojow { get; set; }
        public DbSet<TypWyzywienia> TypyWyzywienia { get; set; }
        public DbSet<Oferta> Oferty {  get; set; }
        public DbSet<TerminCena> TerminyCeny {  get; set; }
        public DbSet<Klient> Klienci {  get; set; }
        public DbSet<Pracownik> Pracownicy { get; set; }
        public DbSet<Rola> Role {  get; set; }
        public DbSet<Rezerwacja> Rezerwacje { get; set; }
        public DbSet<StatusRezerwacji> StatusyRezerwacji { get; set; }
        public DbSet<Doplata> Doplaty { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Hotel>()
                .OwnsOne(h => h.Adres);
        }
    }
}
