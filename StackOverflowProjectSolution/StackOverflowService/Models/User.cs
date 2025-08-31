using StackOverflowService.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StackOverflowService.Models
{
    public class User
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Name is necessary!")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last name is necessary!")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is necessary!")]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Country is necessary!")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "City is necessary!")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Address is necessary!")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is necessary!")]
        [EmailAddress(ErrorMessage = "Invalid e-mail format!")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is necessary!")]
        [StringLength(100, ErrorMessage = "{0} mora biti najmanje {2} karaktera dugačka.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Profile picture")]
        public HttpPostedFileBase ProfileImage { get; set; }

        public string PictureUrl { get; set; }
    }
}