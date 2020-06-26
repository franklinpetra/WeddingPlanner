using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

    
    //This is to implement a onepage login and registration //

namespace WeddingPlanner.Models
{
    public class Wrapper
    {
        public List<User> AllUsers { get; set; }
        public List<LoginUser> AllLogins { get; set; }
        public User UserForm { get; set; }
        public LoginUser LoginForm { get; set; }
        public int OneUserId { get; set; }
    }
}
