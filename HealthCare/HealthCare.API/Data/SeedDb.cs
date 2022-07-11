using HealthCare.API.Data.Entities;

namespace HealthCare.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb( DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckBloodTypeAsync();
            await CheckNationalityypeAsync();
            await CheckCitiesypeAsync();
        }

        private async Task CheckCitiesypeAsync()
        {
            if (!_context.Cities.Any())
            {
                _context.Cities.Add(new City { Description = "Baghdad" });
                _context.Cities.Add(new City { Description = "Mosul" });
                _context.Cities.Add(new City { Description = "Basra" });
                _context.Cities.Add(new City { Description = "Nasiriyah" });
                _context.Cities.Add(new City { Description = "Hillah" });
                _context.Cities.Add(new City { Description = "Najaf" });
                _context.Cities.Add(new City { Description = "Kut" });
                _context.Cities.Add(new City { Description = "Diwaniyah	" });
                _context.Cities.Add(new City { Description = "Karbala" });
                _context.Cities.Add(new City { Description = "Amarah" });
                _context.Cities.Add(new City { Description = "Samawah" });                
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckNationalityypeAsync()
        {
            if (!_context.natianalities.Any())
            {
                _context.natianalities.Add(new Natianality { Description = "Arabs" });
                _context.natianalities.Add(new Natianality { Description = "Kurds" });
                _context.natianalities.Add(new Natianality { Description = "Turkmens" });              
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckBloodTypeAsync()
        {
            if(! _context.BloodTypes.Any())
            {
                _context.BloodTypes.Add(new BloodType { Description = "A+" });
                _context.BloodTypes.Add(new BloodType { Description = "A-" });
                _context.BloodTypes.Add(new BloodType { Description = "B+" });
                _context.BloodTypes.Add(new BloodType { Description = "B-" });
                _context.BloodTypes.Add(new BloodType { Description = "O+" });
                _context.BloodTypes.Add(new BloodType { Description = "O-" });
                _context.BloodTypes.Add(new BloodType { Description = "AB+" });
                _context.BloodTypes.Add(new BloodType { Description = "AB-" });
                await _context.SaveChangesAsync();
            }
        }
    }
}
