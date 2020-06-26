using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;

namespace WeddingPlanner.Models 
{   
    public class LoginUser
    {

        [Required(ErrorMessage="An email is required for login.")]
        [EmailAddress(ErrorMessage="Whoops. Something is missing? Is that a valid email address? Typo perhaps?")]
        [Display(Name = "Email: ")]
        public string Email {get;set;}

        [Required(ErrorMessage="A password is required.")]
        [Display(Name = "Password: ")]
        [DataType(DataType.Password)]
        public string Password {get;set;}
        
    }
}