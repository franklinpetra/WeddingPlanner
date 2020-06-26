using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;


namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {

        private MyContext DbContext;
        public HomeController(MyContext context)
        {
            DbContext = context;
        }

        // controlls for one-page login-registration  with "register,password hasher, match, success and a logout //
        [HttpGet("")]
        public ViewResult Index()
        {
            return View("index");
        }

        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        [HttpPost("register")]
        public IActionResult Register(Wrapper FromForm)
        {
            // User user=WrapperUser.UserForm;
            // if(DbContext.Users.Any(u => u.Email == user.Email))
            // {
            //     ModelState.AddModelError("Email", "Email already in use!");
            // }
            if(ModelState.IsValid)
            {
                                    // Establishing that email is unique //
                if(DbContext.Users.Any(u => u.Email == FromForm.UserForm.Email))
                {
                    ModelState.AddModelError("Register.Email", "Email already in use! Please use it to login.");
                    RedirectToAction ("login");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                FromForm.UserForm.Password = Hasher.HashPassword(FromForm.UserForm, FromForm.UserForm.Password);
                DbContext.Add(FromForm.UserForm);
                DbContext.SaveChanges();

                // User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.Email); 

                HttpContext.Session.SetInt32("UserId", FromForm.UserForm.UserId);

                // Wrapper OneUserId = User userInD
                return RedirectToAction ("dashboard");
            }
            else    
            {
                return View("Index"); 
            }
        }
        [HttpPost("login")]
        public IActionResult Login(Wrapper FromForm)
        {
            if(ModelState.IsValid)
            {
                User InDb = DbContext.Users.FirstOrDefault(u => u.Email == FromForm.LoginForm.Email);

                if(InDb == null)
                {
                    ModelState.AddModelError("Login.Email", "Invalid email/password");
                    return Index();
                }

                PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                PasswordVerificationResult Result = Hasher.VerifyHashedPassword(FromForm.LoginForm, InDb.Password, FromForm.LoginForm.Password);

                if(Result == 0)
                {
                    ModelState.AddModelError("Login.Email", "Invalid email/password");
                    return Index();
                }
                HttpContext.Session.SetInt32("UserId", InDb.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return Index();
            }
        }

        // CRUD  for Wedding Create and Read, Update and Delete Guests and RSVPs and Weddings //

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if (LoggedId == null)
            {
                    return RedirectToAction("Index");
            }
            DashboardWrapper Wrap = new DashboardWrapper()
                {
                    AllWeddings = DbContext.Weddings
                        .Include(w => w.Creator)
                        .Include (w=> w.GuestsAttending)
                            .ThenInclude(g =>g.Guest)
                        .Where(w => w.Date > DateTime.Today)
                        .ToList(),
                    LoggedUser = DbContext.Users
                        .FirstOrDefault(u =>u.UserId ==(int)LoggedId)
                };
                return View("Dashboard", Wrap);
        }


        [HttpGet("weddings/new")]
        public IActionResult NewWedding()
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine("$$$$$$$$$$$$$right before id check in HttpGet in Http GET$$$$$$$$$$$$$$");
            if(LoggedId == null)
            {
                return RedirectToAction("Index");
            }
                return View("NewWedding");
        }    

        [HttpPost("weddings/create")]
        public IActionResult CreateWedding(Wedding wedding)

        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if(LoggedId == null)
            {
                return RedirectToAction("Index");
            }
            // return View("Create");
            //The goal here is to attach the logged in user to this particular wedding//
            
            wedding.UserId = (int)LoggedId;
                if (ModelState.IsValid)
                {
                    if(wedding.Date< DateTime.Today)
                    {
                        ModelState.AddModelError("Date", "We need a future date.");
                        return View ("NewWedding");
                    }
                
                    DbContext.Add(wedding);
                    DbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    return NewWedding();
                }   
        }

        [HttpGet("weddings/{weddingId}")]
        public IActionResult Wedding(int weddingId)
        {
            
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if(LoggedId == null)
            {
                return RedirectToAction("Index");
            }
            
            Wedding OneWedding = DbContext.Weddings

                .Include(w=>w.GuestsAttending)
                .ThenInclude(g =>g.Guest)
                .FirstOrDefault(c => c.WeddingId == weddingId);



            if(OneWedding ==null)
            {
                return RedirectToAction ("Dashboard"); 
            }

            return View("Wedding", OneWedding);     
        }

        [HttpGet("weddings/{WeddingId}/edit")]
        public IActionResult EditWedding(int WeddingId)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if(LoggedId == null)
            {
                return RedirectToAction("Index");
            }

            Wedding ToEdit = DbContext.Weddings.FirstOrDefault(w => w.WeddingId == WeddingId);

            if(ToEdit == null || ToEdit.UserId != (int)LoggedId)
            {
                return RedirectToAction("Dashboard");
            }

            return View("EditWedding", ToEdit);
        }

        [HttpPost("weddings/{WeddingId}/update")]
        public IActionResult UpdateWedding(int WeddingId, Wedding FromForm)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if(LoggedId == null)
            {
                return RedirectToAction("Index");
            }
            
            if(!DbContext.Weddings.Any(w => w.WeddingId == WeddingId && w.UserId == (int)LoggedId))
            {
                return RedirectToAction("Dashboard");
            }
            FromForm.UserId = (int)LoggedId;
            if(ModelState.IsValid)
            {
                FromForm.WeddingId = WeddingId;
                DbContext.Update(FromForm);
                DbContext.Entry(FromForm).Property("CreatedAt").IsModified = false;
                DbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return EditWedding(WeddingId);
            }
        }
            DbContext.SaveChanges();
        
        
        [HttpGet("weddings/{WeddingId}/rsvp")]
        public RedirectToActionResult RSVP(int WeddingId)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if(LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }
            Wedding ToJoin = DbContext.Weddings
                .Include(w => w.GuestsAttending)
                .FirstOrDefault(w => w.WeddingId == WeddingId);

            if(ToJoin == null || ToJoin.UserId == (int)LoggedId || ToJoin.GuestsAttending.Any(r => r.UserId == (int)LoggedId))
            {
                return RedirectToAction("Dashboard");
            }
            else
            {
                Confirmation NewRsvp = new Confirmation()
                {
                    UserId = (int)LoggedId,
                    WeddingId = WeddingId
                };
                DbContext.Add(NewRsvp);
                DbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet("weddings/{WeddingId}/unrsvp")]
        public RedirectToActionResult UnRSVP(int WeddingId)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if(LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }
            Wedding ToLeave = DbContext.Weddings
                .Include(w => w.GuestsAttending)
                .FirstOrDefault(w => w.WeddingId == WeddingId);

            if(ToLeave == null || !ToLeave.GuestsAttending.Any(r => r.UserId == (int)LoggedId))
            {
                return RedirectToAction("Dashboard");
            }
            else
            {
                Confirmation ToRemove = DbContext.Confirmations.FirstOrDefault(r => r.UserId == (int)LoggedId && r.WeddingId == WeddingId);
                DbContext.Remove(ToRemove);
                DbContext.SaveChanges();

                return RedirectToAction("Dashboard");
            }
        }

        // [HttpPost("weddings/{WeddingId}/unrsvp")]
        // public IActionResult UnRSVP(int WeddingId)
        // {
            
        //     Confirmation ConfirmationToRemove = DbContext.Confirmations
        //         .FirstOrDefault(a => a.WeddingId == WeddingId && a.UserId ==Convert.ToInt32(HttpContext.Session.GetInt32("UserId")));
        //         DbContext.Remove(ConfirmationToRemove);
        //         DbContext.SaveChanges();

        //         return RedirectToAction("Dashboard");                        
        // }

        [HttpGet("weddings/{WeddingId}/delete")]
        public RedirectToActionResult DeleteWedding(int WeddingId)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if(LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }

            Wedding ToDelete = DbContext.Weddings
                .FirstOrDefault(w => w.WeddingId == WeddingId);

            if(ToDelete == null || ToDelete.UserId != (int)LoggedId)
            {
                return RedirectToAction("Dashboard");
            }

            DbContext.Remove(ToDelete);
            DbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        
        [HttpGet("TimeDisplay")]
        public IActionResult TimeDisplay()
        {
            ViewBag.Date=DateTime.Now.ToString("MMM dd,yyyy");
            ViewBag.Time=DateTime.Now.ToString("h:mm tt");
            return View("TimeDisplay");
        }


        [HttpGet("TimeTravel")]
        public IActionResult TimeTravel()
        {
            return View("TimeTravel");
        }

        [HttpGet("Cat")]
        public IActionResult Cat()
        {
            return View("Cat");
        }
        [HttpGet("Pup")]
        public IActionResult Pup()
        {
            return View("Pup");
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View ("Privacy");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    
    }

}

//         [HttpPost("weddings/{weddingId}/delete")]
//         public IActionResult DeleteWedding(int WeddingId)
//         {
            
//             Wedding OneSingleWedding =DbContext.Weddings
//                 .Include(w => w.Creator)
//                 .FirstOrDefault(n =>n.WeddingId == weddingId);

//             int? userId = HttpContext.Session.GetInt32("UserId");
//             if (userId == null)
//                 {
//                     return RedirectToAction("Index");
//                 }   
//             else if (userId != OneSingleWedding.UserId)
//                 {
//                     return RedirectToAction("Dashboard");  
//                 }  
//                 DbContext.Weddings.Remove(OneSingleWedding);
//                 DbContext.SaveChanges();
//                 RedirectToAction("Dashboard");             
//             }



        