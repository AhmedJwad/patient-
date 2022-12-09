using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Models;
using HealthCare.Common.Enums;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace HealthCare.API.Helpers
{
    public class ConverterHelper : IconverterHelper
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(DataContext context  , ICombosHelper combosHelper)
        {
            _context = context;
           _combosHelper = combosHelper;
        }

        

        public async Task<Detail> ToDetailAsync(DetailViewModel model, bool isNew)
        {
            return new Detail
            {
                Id = isNew ? 0 : model.Id,
                History = await _context.histories.FindAsync(model.HistoryId),
                diagonisic = await _context.diagonisics.FindAsync(model.diagonisicId),
              Description=model.Description,
            };
        }

        public DetailViewModel ToDetailViewModel(Detail detail)
        {
            return new DetailViewModel
            {
                HistoryId = detail.History.Id,
                Id = detail.Id,
                Description = detail.Description,
                diagonisicId = detail.diagonisic.Id,
                diagonisics = _combosHelper.GetCombodiagnosic(),
              

            };
        }

        public async Task<Patient> ToPatientAsync(patientViewmodel model, bool isNew)
        {
            return new Patient
            {
                FirstName=model.FirstName,
                LastName=model.LastName,
                Address=model.Address,
                bloodType=await _context.BloodTypes.FindAsync(model.BloodTypeId),
                gendre=await  _context.gendres.FindAsync(model.GendreId),
                Natianality=await _context.natianalities.FindAsync(model.NationalityId),
                City=await _context.Cities.FindAsync(model.CityId),
                userPatient=await _context.UserPatients.FindAsync(model.UserpatientId),
                EPCNNumber=model.EPCNNumber,
                Date=model.Date,
                MobilePhone=model.MobilePhone,
                Description=model.Description,
                Id = isNew ? 0 : model.Id,
                
            };
        }

        public  patientViewmodel ToPatientViewModel(Patient patient)
        {
            return new patientViewmodel
            {
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Address = patient.Address,
                Id = patient.Id,
                CityId = patient.City.Id,
                Cities = _combosHelper.GetCities(),
                NationalityId = patient.Natianality.Id,
                Nationaliteis = _combosHelper.GetNationalities(),
                GendreId = patient.gendre.Id,
                Gendres = _combosHelper.Getgendres(),
                BloodTypeId = patient.bloodType.Id,
                bloodTypes = _combosHelper.GetComboBloodtypes(),
                Description = patient.Description,
                MobilePhone = patient.MobilePhone,
                EPCNNumber = patient.EPCNNumber,
                UserId = patient.User.Id,
                patientPhotos=patient.patientPhotos,
                Date = patient.Date,    
                UserpatientId=patient.userPatient.Id,
                UserPatients=_combosHelper.GetUserPatients(),
            };
        }

        public async Task<User> ToUserAsync(UserViewModel model, Guid imageId, bool isNew)
        {
            return new User
            {
                Address=model.Address,  
                CountryCode=model.CountryCode,
                PhoneNumber=model.PhoneNumber,  
                Email=model.Email,
                LastName=model.LastName,
                ImageId=imageId,
                Id = isNew ? Guid.NewGuid().ToString() : model.Id,
                FirstName =model.FirstName,
                userType=model.UserType,    
                UserName=model.Email,

            };
        }

        public UserViewModel ToUserViewModel(User user)
        {
            return new UserViewModel
            {
                Address = user.Address,
                CountryCode=user.CountryCode,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                LastName = user.LastName,
                ImageId = user.ImageId,
                Id = user.Id,
                FirstName = user.FirstName,
                UserType = user.userType,               
            };
        }
        public async Task AddDays(int day, string id)
        {
            DateTime initialDate;

            User user = await _context.Users.Include(x => x.Agendas)
               .FirstOrDefaultAsync(x => x.Id == id);
            var agenda = _context.agendas.FirstOrDefault();
            if (!_context.agendas.Any())
            {
                initialDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            }
            else
            {

                initialDate = new DateTime(agenda.Date.Year, agenda.Date.Month, agenda.Date.AddDays(1).Day, 8, 0, 0);
            }

            var finalDate = initialDate.AddDays(day);
            while (initialDate < finalDate)
            {
                if (initialDate.DayOfWeek != DayOfWeek.Friday)
                {
                    var finalDate2 = initialDate.AddHours(10);
                    while (initialDate < finalDate2)
                    {
                        _context.agendas.Add(new Agenda
                        {
                            user = user,
                            Date = initialDate.ToUniversalTime(),
                            IsAvailable = true
                        });

                        initialDate = initialDate.AddMinutes(30);
                    }

                    initialDate = initialDate.AddHours(14);
                }
                else
                {
                    initialDate = initialDate.AddDays(1);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
