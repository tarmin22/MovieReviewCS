using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieReview.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;


namespace MovieReview.Controllers
{
    public class HomeController : Controller
    {
        //Note that MyContext must match the name of your context file
        private MyContext dbContext;
        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("regLog")]
        public IActionResult RegLog()
        {
            return View();
        }

        [HttpPost("add")]
        public IActionResult Add(User user)
        {
            if (ModelState.IsValid)
            {



                if (dbContext.Users.Any(u => u.Username == user.Username))
                {

                    ModelState.AddModelError("Username", "Username already in use!");



                    return View("RegLog");
                }

                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return RedirectToAction("Dashboard");

                }
            }

            return View("RegLog");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("loginU")]
        public IActionResult LoginU(Login l_user)
        {

            if (ModelState.IsValid)
            {
                User currentUser = dbContext.Users.FirstOrDefault(s => s.Username == l_user.LUsername);
                if (currentUser == null)
                {
                    ModelState.AddModelError("LUsername", "Invalid Username/Password");
                    return View("RegLog");
                }

                var hasher = new PasswordHasher<Login>();
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(l_user, currentUser.Password, l_user.LPassword);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    ModelState.AddModelError("LUsername", "Invalid Email/Password");
                    return View("RegLog");
                }
                HttpContext.Session.SetInt32("UserId", currentUser.UserId);
                return RedirectToAction("Dashboard");


            }
            return View("RegLog");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {


            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            var loggedUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            var allmovies = dbContext.Movies.Include(u => u.Createdby).Include(a => a.MovieReview).ThenInclude(u => u.ReviewUser).ToList();

            // var allactivities = dbContext.Activities.OrderByDescending(a => a.Date).Where(a => a.Date > DateTime.Now).Include(u => u.Createdby).Include(a => a.ActivityUser).ThenInclude(u => u.ActivityParticipant).ToList();

            // Added where to check if the date of the activity has passed the current date
            // var allactivities = dbContext.Activities.OrderByDescending(a => a.Date).Include(u => u.Createdby).Include(a => a.ActivityUser).ThenInclude(u => u.ActivityParticipant).Where(a => a.Date < DateTime.Now).ToList();


            // foreach (var i in allweddings)
            // {
            //     if ((DateTime.Compare(DateTime.Today, i.WedDate) > 0))
            //     {
            //         dbContext.Weddings.Remove(i);
            //         dbContext.SaveChanges();
            //     }
            // }
            // var newweddings = dbContext.Weddings.Include(w => w.WeddingGuest).ThenInclude(u => u.Planner).ToList();
            ViewBag.userName = loggedUser.FirstName + " " + loggedUser.LastName;
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");


            return View(allmovies);
        }

        [HttpGet("showInfo/{movId}")]
        public IActionResult ShowInfo(int movId)
        {
            var movie = dbContext.Movies.Include(u => u.Createdby).Include(r => r.MovieReview).ThenInclude(u => u.ReviewUser).FirstOrDefault(h => h.MovieId == movId);
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(movie);

        }


        [HttpGet("newMovie")]
        public IActionResult NewMovie()
        {

            return View();
        }

        [HttpPost("addMovie")]
        public IActionResult AddMovie(Movie movie)
        {

            if (ModelState.IsValid)
            {

                if (dbContext.Movies.Any(m => m.Title == movie.Title))
                {

                    ModelState.AddModelError("Name", "Movie name is already in use!");

                    return View("NewHobby");
                }
                var newMov = movie;
                newMov.UserId = (int)HttpContext.Session.GetInt32("UserId");
                dbContext.Movies.Add(newMov);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {

                return View("NewMovie");

            }
        }

        [HttpGet("newReview/{movID}")]
        public IActionResult NewReview(int movID)
        {

            ViewBag.movie = dbContext.Movies.FirstOrDefault(m => m.MovieId == movID);




            return View();
        }


        [HttpPost("addReivew/{movID}")]
        public IActionResult AddReview(Review review, int movID)
        {

            if (ModelState.IsValid)
            {

                var newRev = review;
                newRev.UserId = (int)HttpContext.Session.GetInt32("UserId");
                newRev.MovieId = movID;
                newRev.Description = review.Description;
                newRev.Rating = review.Rating;
                dbContext.Reviews.Add(newRev);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {

                return View("NewMovie");

            }
        }


        // [HttpGet("editHobby/{hobID}")]
        // public IActionResult EditHobby(int hobID)
        // {
        //     Hobby hobby = dbContext.Hobbies.FirstOrDefault(a => a.HobbyId == hobID);
        //     return View(hobby);
        // }

        // [HttpPost("updHubby/{hobID}")]

        // public IActionResult UpdHobby(Hobby hobby, int hobID)
        // {

        //     if (ModelState.IsValid)
        //     {

        //         Hobby u_hob = dbContext.Hobbies.FirstOrDefault(a => a.HobbyId == hobID);

        //         u_hob.Name = hobby.Name;
        //         u_hob.Description = hobby.Description;
        //         u_hob.UpdatedAt = DateTime.Now;


        //         dbContext.SaveChanges();
        //         return RedirectToAction("ShowInfo", new { HobID = hobID });

        //     }
        //     else
        //     {
        //         Hobby u_hob = dbContext.Hobbies.FirstOrDefault(a => a.HobbyId == hobID);
        //         // ModelState.AddModelError("Date", "Invalid Activity Date.  Activity Date must be after today's date.");
        //         return View("EditHobby", u_hob);

        //     }
        // }

        // [HttpGet("join/{hobID}")]
        // public IActionResult Join(int hobID)
        // {
        //     var check_hob = dbContext.Enthusiasts.Where(e => e.HobbyId == hobID);
        //     var userID = (int)HttpContext.Session.GetInt32("UserId");

        //     if (check_hob != null)
        //     {
        //         foreach (var i in check_hob)
        //         {
        //             if (i.HobbyId == hobID && i.UserId == userID)
        //             {
        //                 return RedirectToAction("ShowInfo", new { HobID = hobID });
        //             }
        //         }
        //     }

        //     Enthusiast uhob = new Enthusiast();
        //     uhob.UserId = (int)HttpContext.Session.GetInt32("UserId");
        //     uhob.HobbyId = hobID;
        //     dbContext.Enthusiasts.Add(uhob);
        //     dbContext.SaveChanges();
        //     return RedirectToAction("Dashboard");
        // }

        // [HttpPost("addActivity")]
        // public IActionResult AddActivity(Activity activity)
        // {

        //     if (ModelState.IsValid)
        //     {

        //         int result = DateTime.Compare(DateTime.Today, activity.Date);
        //         if (result < 1)
        //         {
        //             var newAct = activity;
        //             newAct.UserId = (int)HttpContext.Session.GetInt32("UserId");
        //             dbContext.Activities.Add(newAct);
        //             dbContext.SaveChanges();

        //             // Activity act = dbContext.Activities.LastOrDefault(a => a.UserId == newAct.UserId);
        //             // int actID = act.ActivityId;

        //             return RedirectToAction("ActInfo", new { actID = newAct.ActivityId });
        //         }
        //         else
        //         {
        //             ModelState.AddModelError("Date", "Invalid activity Date.  Activity date must be past today's date.");
        //             return View("NewActivity");

        //         }
        //     }
        //     return View("NewActivity");
        // }

        // [HttpGet("actInfo/{actID}")]
        // public IActionResult ActInfo(int actID)
        // {

        //     var loggedUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
        //     ViewBag.userName = loggedUser.FirstName;
        //     ViewBag.userID = loggedUser.UserId;

        //     Activity activity = dbContext.Activities.Include(u => u.Createdby).Include(a => a.ActivityUser).ThenInclude(u => u.ActivityCreator).FirstOrDefault(a => a.ActivityId == actID);
        //     return View(activity);
        // }



        // [HttpGet("leave/{actID}")]
        // public IActionResult Leave(int actID)
        // {
        //     UserActivity toDelete = dbContext.UserActivities.FirstOrDefault(u => u.ActivityId == actID);
        //     if (toDelete == null)
        //         return RedirectToAction("Dashboard");

        //     dbContext.UserActivities.Remove(toDelete);
        //     dbContext.SaveChanges();

        //     return RedirectToAction("Dashboard");
        // }


        // [HttpGet("delete/{actID}")]
        // public IActionResult Delete(int actID)
        // {
        //     Activity toDelete = dbContext.Activities.FirstOrDefault(u => u.ActivityId == actID);
        //     if (toDelete == null)
        //         return RedirectToAction("Dashboard");

        //     dbContext.Activities.Remove(toDelete);
        //     dbContext.SaveChanges();

        //     return RedirectToAction("Dashboard");
        // }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }







    }
}
