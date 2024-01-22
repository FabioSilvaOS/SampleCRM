﻿using OpenRiaServices.DomainServices.Hosting;
using OpenRiaServices.DomainServices.Server;
using SampleCRM.Web.Attributes;
using SampleCRM.Web.Models;
using System.Data.Entity.Migrations;
using System.Linq;

namespace SampleCRM.Web
{
    [EnableClientAccess]
    public class CountryCodesService : SampleCRMService
    {
        [Query]
        public IQueryable<CountryCodes> GetCountries() => 
            _context.CountryCodes.OrderBy(c => c.Name);

        public CountryCodes GetCountryById(string countryCodeID) =>
            GetCountries().SingleOrDefault(x => x.CountryCodeID == countryCodeID);

        [Delete]
        [RestrictAccessReadonlyMode]
        public void DeleteCountry(CountryCodes country) => _context.CountryCodes.Remove(country);

        [Insert]
        [RestrictAccessReadonlyMode]
        public void InsertCountry(CountryCodes country) => _context.CountryCodes.AddOrUpdate(country);

        [Update]
        [RestrictAccessReadonlyMode]
        public void UpdateCountry(CountryCodes country) => _context.CountryCodes.AddOrUpdate(country);
    }
}