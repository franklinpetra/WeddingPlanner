using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Confirmation
    {
    [Key]
    public int ConfirmationId {get;set;}
    
        // ManyToMany relationship here //
    public int UserId  {get;set;}
        
    public int WeddingId {get;set;}

    public User Guest {get;set;}
    public Wedding Wedding { get;set; }

    public DateTime CreatedAt { get;set; } = DateTime.Now;
    public DateTime UpdatedAt { get;set; } = DateTime.Now;
    }   
}
