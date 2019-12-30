using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using AIMS.Services.Helpers;

namespace AIMS.Services
{
    public interface ICountryService
    {
        /// <summary>
        /// Add list of countries
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        ActionResponse AddList(List<IATICountryModel> countriesList);

        /// <summary>
        /// Gets primary country
        /// </summary>
        /// <returns></returns>
        string GetActiveCountry();

        /// <summary>
        /// Sets primary country
        /// </summary>
        /// <returns></returns>
        ActionResponse SetActiveCountry(string code);

        /// <summary>
        /// Get countries list
        /// </summary>
        /// <returns></returns>
        ICollection<IATICountryModel> GetAll();
    }

    public class CountryService : ICountryService
    {
        AIMSDbContext context;
        IMapper mapper;

        public CountryService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public ActionResponse AddList(List<IATICountryModel> countriesList)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var countriesInDb = unitWork.IATICountryRepository.GetProjection(c => c.Id != 0, c => c.Code);
                int newCountriesCount = 0;
                foreach(var country in countriesList)
                {
                    var countryExists = (from code in countriesInDb
                                         where code.Equals(country.Code, StringComparison.OrdinalIgnoreCase)
                                         select code).FirstOrDefault();
                    if (countryExists == null)
                    {
                        unitWork.IATICountryRepository.Insert(new EFIATICountryCodes()
                        {
                            Code = country.Code,
                            Country = country.Country
                        });
                        ++newCountriesCount;
                    }
                }
                try
                {
                    if (newCountriesCount > 0)
                    {
                        unitWork.Save();
                    }
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse SetActiveCountry(string code)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var country = unitWork.IATICountryRepository.GetOne(c => c.Code == code);
                if (country == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Country");
                    return response;
                }

                try
                {
                    country.IsActive = true;
                    unitWork.IATICountryRepository.Update(country);
                    unitWork.Save();
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public string GetActiveCountry()
        {
            var unitWork = new UnitOfWork(context);
            string code = "";
            var country = unitWork.IATICountryRepository.GetOne(c => c.IsActive == true);
            if (country != null)
            {
                code = country.Code;
            }
            return code;
        }

        public ICollection<IATICountryModel> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var list = unitWork.IATICountryRepository.GetAll();
                return mapper.Map<List<IATICountryModel>>(list);
            }
        }
    }
}
