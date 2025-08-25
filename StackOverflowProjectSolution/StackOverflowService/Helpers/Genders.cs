using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StackOverflowService.Helpers
{
    public enum Gender
    {
        [Display(Name = "Muški")]
        Male,
        [Display(Name = "Ženski")]
        Female,
        [Display(Name = "Drugo")]
        Other
    }
}