using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace WeddingPlanner.Models{
    public class Wedding
    {
    
        //Validation to establish that the wedding is in the future//
        // public class WeddingInFuture :ValidationAttribute
        // {
        //     protected override ValidationResult IsValid(object value, ValidationContext validationcontext)
        //     {
        //         DateTime InputDate = Convert.ToDateTime(value);
        //         if (InputDate.AddMinutes(1) > DateTime.Now)
        //         {
        //             return ValidationResult.Success;
        //         }
        //         return new ValidationResult("Plase update your date. Your Wedding day must be in the future.");
        //     }
        // }
    
        [Key]
        public int WeddingId {get;set;}
        
        
        [Required(ErrorMessage="A name is neccessary here. Here are a few possibilities: Janet, Henry, Lawrence, Madeline")]
        [Display(Name = "Partner One: ")]
        public string PartnerOne {get;set;}


        [Required(ErrorMessage="A name is neccessary here. Need ideas? Janet? Malcolm? Ralph?")]
        [Display(Name = "Partner Two: ")]  
        public string PartnerTwo {get;set;}


        [Required(ErrorMessage="Whoops. Something is missing?  We'll need a date to confim you.")]
        [DataType(DataType.Date)]
        public DateTime? Date { get;set; }


        [Required(ErrorMessage="Whoops. Something is missing?  We'll need a location to confim you.")]
        [Display(Name = "Location: ")]
        public string Location {get;set;}

        
        // Navigation properties UserId and Creator/the foreign key is GuestsAttending //
        public int UserId { get;set; }
        public User Creator {get; set; }
        public List<Confirmation> GuestsAttending { get;set; }
        
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}