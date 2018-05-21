using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private PlanContext _context;
 
        public HomeController(PlanContext context) 
        {
            _context = context; 
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("Index"); 
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterUser(RegUser newRegUser) 
        {
            if (ModelState.IsValid) 
            {
                User current = _context.Users.SingleOrDefault(e => e.Email == newRegUser.Email); 
                if(current != null)
                {
                    ModelState.AddModelError("Email", "Email already exists!"); 
                    
                    return View("Index");
                }
                else 
                {
                    PasswordHasher<RegUser> Hasher = new PasswordHasher<RegUser>(); 
                    string hashed = Hasher.HashPassword(newRegUser, newRegUser.Password); 
                    User user = new User // Creating a new User using the info provided by the user via the form
                    {
                        FirstName = newRegUser.FirstName,
                        LastName = newRegUser.LastName,
                        Email = newRegUser.Email,  
                        Password = hashed,    
                    };
                    _context.Add(user); 
                    _context.SaveChanges(); 
                    User sessionuser = _context.Users.Where(u => u.Email == newRegUser.Email).SingleOrDefault(); 
                    HttpContext.Session.SetInt32("userID", sessionuser.UserId); 
                    HttpContext.Session.SetString("firstname", sessionuser.FirstName); 
                    return RedirectToAction("Dash"); 
                }
            }
            else 
            {
                return View("Index"); 
                
            }
        }

        [HttpGet]
        [Route("login")]
        public IActionResult LoginView()
        {
            return View("Login");
        }

        [HttpPost]
        [Route("login")]
        public IActionResult LoginUser(LoginUser loginUser)
        {
            if(ModelState.IsValid) 
            {
                User current = _context.Users.Where(u => u.Email == loginUser.Email).SingleOrDefault(); 
                if(current == null) 
                {
                    ModelState.AddModelError("Email", "Email does not exist!"); 

                    return View("Login");
                }
                else
                {
                    var hasher = new PasswordHasher<User>(); 
                    if(hasher.VerifyHashedPassword(current, current.Password, loginUser.Password) == 0) 
                    {
                        ModelState.AddModelError("Password", "Incorrect password!"); 
                        return View("Login");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("userID", current.UserId); 
                        HttpContext.Session.SetString("firstname", current.FirstName); 
                        return RedirectToAction("Dash");
                    }
                }
            }
            else 
            {
                return View("Login");
            }
        }

        [HttpGet]
        [Route("overview/{Weddingid}")]
        public IActionResult Overview(int Weddingid) 
        {
            Planner oneWedding = _context.Planners.SingleOrDefault (w => w.WeddingId == Weddingid); 
            ViewBag.WeddingInfo = oneWedding; 
            List<Planner> guests = _context.Planners.Where (w => w.WeddingId == Weddingid).Include (r => r.Guests).ThenInclude (u => u.user).ToList (); 
            ViewBag.AllGuests = guests; 
            return View("Overview");
        }

        [HttpPost]
        [Route("planner")]
        public IActionResult Planner(CreateWedding newPlan) 
        {
            if(ModelState.IsValid) 
            {
                int? id = HttpContext.Session.GetInt32("userID"); 
                Planner planner = new Planner 
                {
                    UserId = (int)id,
                    WedderOne = newPlan.WedderOne,
                    WedderTwo = newPlan.WedderTwo,
                    WeddingDate = newPlan.WeddingDate,
                    Address = newPlan.Address,
                };
                User user = _context.Users.Where(u => u.UserId == planner.UserId).SingleOrDefault();
                _context.Add(planner); 
                _context.SaveChanges(); 
                return RedirectToAction("Dash");
            }
            else 
            {
                return View("Create");
            }
        }
       
        [HttpGet]
        [Route("CreateWedding")]
        public IActionResult CreateWedding() 
        {
            return View("Create");
        }

        [HttpGet]
        [Route("dash")]
        public IActionResult Dash() 
        {
            int? UserId = HttpContext.Session.GetInt32("userID"); 
            List<User> user = _context.Users.Include(u => u.Planners).ToList(); 
            List<Planner> allWeddings = _context.Planners.Include(u => u.Guests).Include(u => u.user).ToList();
            List<Guest> guests = _context.Guests.Include(u => u.planner).ThenInclude(u => u.user).ToList();
            ViewBag.AllWeddings = allWeddings;
            User banana = _context.Users.SingleOrDefault (u => u.UserId == UserId);
            ViewBag.UserId = UserId; 
            ViewBag.user = banana;
            return View("Dash");
        }

        [HttpGet]
        [Route("rsvp/{WeddingId}")]
        public IActionResult Rsvp(int GuestId, int WeddingId)
        {
            Guest RSVP = new Guest
            {
                UserId = (int)HttpContext.Session.GetInt32("userID"),
                WeddingId = WeddingId
            };
            Guest existingGuest = _context.Guests.SingleOrDefault(r => r.UserId == (int)HttpContext.Session.GetInt32("userID") && r.WeddingId == WeddingId);
            if (existingGuest == null)
            {
                _context.Guests.Add(RSVP);
                _context.SaveChanges();
            }
            return RedirectToAction("Dash");
        }

        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            Planner RetrievedWedding = _context.Planners.SingleOrDefault(w => w.WeddingId == id);
            _context.Remove(RetrievedWedding);
            _context.SaveChanges();
            return RedirectToAction("Dash");
        }

        [HttpGet]
        [Route("unrsvp/{WeddingId}")]
        public IActionResult Unrsvp(int WeddingId)
        {
            int? Session = HttpContext.Session.GetInt32("userID");
            Guest banana = _context.Guests.Where(r=> r.WeddingId == WeddingId).Where(u => u.UserId == Session).SingleOrDefault();
            _context.Remove(banana);
            _context.SaveChanges();
            return RedirectToAction("Dash");
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("update/{WeddingId}")]
        public IActionResult UpdateView()
        {
            return View("Update");
        }


        [HttpPost]
        [Route("edit")]
        public IActionResult Edit(CreateWedding newPlan)
        {
            if(ModelState.IsValid) 
            {
                int? Session = HttpContext.Session.GetInt32("userID"); 
                User user = _context.Users.SingleOrDefault(u => u.UserId == Session);
                Planner planner = new Planner 
                {
                    UserId = (int)Session,
                    WedderOne = newPlan.WedderOne,
                    WedderTwo = newPlan.WedderTwo,
                    WeddingDate = newPlan.WeddingDate,
                    Address = newPlan.Address,
                };
                _context.Planners.Update(planner);
                _context.SaveChanges();
                return RedirectToAction("Dash");
            }
            else 
            {
                return View("edit");
            }
        }
    }
} 

