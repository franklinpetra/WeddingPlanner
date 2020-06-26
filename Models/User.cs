using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace WeddingPlanner.Models{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required(ErrorMessage="A first name is neccessary. Here are a few possibilities: Janet, Henry, Lawrence, Madeline")]
        [MinLength(2, ErrorMessage="We'll need a first name of more than 2 characters.")]
        [Display(Name = "First Name: ")]
        public string FirstName {get;set;}


        [Required(ErrorMessage="A last name is neccessary. Here are a few possibilities: Brown, Smith, Washington, Childs")]
        [MinLength(2, ErrorMessage="We'll need a last name of more than 2 characters.")]
        [Display(Name = "Last Name: ")]
        public string LastName {get;set;}


        [Required(ErrorMessage="We'll need an email to contact you.")]
        [EmailAddress(ErrorMessage="Whoops. Something is missing? Is that a valid email address?")]
        [Display(Name = "Email: ")]
        public string Email {get;set;}


        [DataType(DataType.Password)]
        [Required(ErrorMessage="A password of 8 characters is required.")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$", ErrorMessage="Password must contain at least 1 letter and 1 number")]
        [MinLength(8, ErrorMessage="Your password must be 8 characters in length.")]
        [Display(Name = "Password: ")]
        public string Password {get;set;}

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password: ")]
        public string Confirm {get;set;}   

        
        // Navigation properties/foreign key //
        public List<Wedding> CreatedWeddings { get;set; }
        public List<Confirmation> WeddingsAttending { get;set; }
        
        
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        
    }
}