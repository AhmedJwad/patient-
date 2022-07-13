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
    }
}
