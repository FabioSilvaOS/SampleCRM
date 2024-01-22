﻿using OpenRiaServices.DomainServices.Client;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SampleCRM.Web.Models
{
    public partial class Customers : Entity
    {
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FirstName)
                || e.PropertyName == nameof(LastName))
            {
                RaisePropertyChanged(nameof(FullName));
                RaisePropertyChanged(nameof(Initials));
            }
            else if (e.PropertyName == nameof(AddressLine1)
                || e.PropertyName == nameof(AddressLine2)
                || e.PropertyName == nameof(City)
                || e.PropertyName == nameof(Region)
                || e.PropertyName == nameof(PostalCode)
                || e.PropertyName == nameof(CountryName))
            {
                RaisePropertyChanged(nameof(FullAddress));
            }

            base.OnPropertyChanged(e);
        }

        public bool IsNew => CustomerID <= 0;

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                if (_isEditMode != value)
                {
                    _isEditMode = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsEditMode)));
                }
            }
        }

        private IEnumerable<CountryCodes> _countryCodes;
        public IEnumerable<CountryCodes> CountryCodes
        {
            get { return _countryCodes; }
            set
            {
                if (_countryCodes != value)
                {
                    _countryCodes = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(CountryCodes)));
                    if (_countryCodes != null)
                        CountryName = _countryCodes.FirstOrDefault(x => x.CountryCodeID == CountryCode)?.Name;
                }
            }
        }

        private string _countryName;
        public string CountryName
        {
            get { return _countryName; }
            set
            {
                if (_countryName != value)
                {
                    _countryName = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(CountryName)));
                }
            }
        }

        public string FullName => $"{FirstName} {LastName}";

        public string Initials => string.Format("{0}{1}", $"{FirstName} "[0], $"{LastName} "[0]).Trim().ToUpper();

        public string FullAddress => $"{AddressLine1} {AddressLine2}\n{City}, {Region} {PostalCode}\n{CountryName}";

        public string PictureUrl
        {
            get
            {
                var app = (System.Windows.Application.Current as App);
                var imageUrl = app.ImageUrl;
                return $"{imageUrl}?customerid={CustomerID}";
            }
        }
    }
}