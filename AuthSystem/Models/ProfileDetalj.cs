using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;


namespace NotBlocket2.Models {
    public class Profile {

        //Konstruktor
        public Profile() { }

        //Publika egenskaper
        [Required]
        public string Name { get; set; }

        [Required, StringLength(60, MinimumLength = 3)]
        public string Email { get; set; }

        [Required, StringLength(60, MinimumLength = 3)]
        public string Password { get; set; }

        [LocationIdValidator]
        public int Location_Id { get; set; }

        public int Id { get; set; }

    }

    // Create a validator to make sure we only allow locations that are in the location table
    public class LocationIdValidatorAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            int locationId = (int)value;
            string errorMessage;
            LocationMethods lm = new LocationMethods();
            List<int> locationIds = lm.GetLocationsWithDataSet(out errorMessage).Select(x => x.Id).ToList();

            if (locationIds.Contains(locationId)) {
                return ValidationResult.Success;
            }
            else {
                return new ValidationResult("The Location ID is not valid.");
            }
        }
    }

}
