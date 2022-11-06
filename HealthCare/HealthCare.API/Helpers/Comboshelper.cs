using HealthCare.API.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthCare.API.Helpers
{
    public class Comboshelper : ICombosHelper
    {
        private readonly DataContext _context;

        public Comboshelper(DataContext context)
        {
            _context = context;
        }
        public  IEnumerable<SelectListItem> GetCities()
        {
            List<SelectListItem> list = _context.Cities.Select(x => new SelectListItem
            {
                Text = x.Description,
                Value = $"{x.Id}"
            }).OrderBy(x => x.Text).ToList();
            list.Insert(0, new SelectListItem
            {
                Text = "[Select a City...]",
                Value = "0"
            });
            return list;    
        }

        public IEnumerable<SelectListItem> GetComboBloodtypes()
        {
            List<SelectListItem> list = _context.BloodTypes.Select(x => new SelectListItem
            {
                Text = x.Description,
                Value = $"{x.Id}"
            }).OrderBy(x => x.Text).ToList();
            list.Insert(0, new SelectListItem
            {
                Text = "[Select a Type of blood...]",
                Value = "0"
            });
            return list;
        }

        public IEnumerable<SelectListItem> GetCombodiagnosic()
        {
            List<SelectListItem> list = _context.diagonisics.Select(x => new SelectListItem
            {
                Text = x.Description,
                Value = $"{x.Id}"
            }).OrderBy(x => x.Text).ToList();
            list.Insert(0, new SelectListItem
            {
                Text = "[Select a diagnosic...]",
                Value = "0"
            });
            return list;
        }
      
        public IEnumerable<SelectListItem> Getgendres()
        {
            List<SelectListItem> list = _context.gendres.Select(x => new SelectListItem
            {
                Text = x.Description,
                Value = $"{x.Id}"
            }).OrderBy(x => x.Text).ToList();
            list.Insert(0, new SelectListItem
            {
                Text = "[Select a Gendre...]",
                Value = "0"
            });
            return list;
        }

        public IEnumerable<SelectListItem> GetNationalities()
        {
            List<SelectListItem> list = _context.natianalities.Select(x => new SelectListItem
            {
                Text = x.Description,
                Value = $"{x.Id}"
            }).OrderBy(x => x.Text).ToList();
            list.Insert(0, new SelectListItem
            {
                Text = "[Select a Nationality...]",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboRoles()
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "(Select a role...)" },
                new SelectListItem { Value = "1", Text = "Doctor" },
                new SelectListItem { Value = "2", Text = "Patient" }
            };
            return list;
        }
        public IEnumerable<SelectListItem> GetUserPatients()
        {
            List<SelectListItem> list = _context.UserPatients.Select(x => new SelectListItem
            {
                Text = x.User.Email,
                Value = $"{x.Id}"
            }).OrderBy(x => x.Text).ToList();
            list.Insert(0, new SelectListItem
            {
                Text = "[Select a User as patients...]",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetPatient(string userId)
        {
            var list = _context.patients
              .Where(x => x.User.Id == userId).Select(p => new SelectListItem
              {
                  Text = p.FirstName + p.LastName,
                  Value = $"{p.Id}"
              }).OrderBy(p => p.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a patient...)",
                Value = "0"
            });

            return list;
        }
    }
}
