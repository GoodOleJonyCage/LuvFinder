using LuvFinder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace LuvFinder.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        public UserController() { }

        private readonly LuvFinderContext? db ;
        public UserController(LuvFinderContext _db)
        {
            db = _db;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("registeruser")]
        public ActionResult RegisterUser([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
        {
            var user = new ViewModels.User()
            {
                UserName = userParams.GetProperty("username").ToString(),
                Password = userParams.GetProperty("password").ToString()
            };

            //validation
            if (string.IsNullOrEmpty(user.UserName))
            {
                return BadRequest("Username required");
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Password required");
            }

            //check for email validity
            try
            {
                MailAddress email = new MailAddress(user.UserName);
            }
            catch (FormatException fe)
            {
                //Bad email
                return BadRequest("Username should be a valid email address");
            }

            if (UserExists(user))
            {
                return BadRequest("User already exists");
            }
            else
            {
                db.Users.Add(new User()
                {
                    Username = user.UserName,
                    Password = user.Password

                });
                db.SaveChanges();

                return Ok(true);
            }
        }

        public bool UserExists(ViewModels.User user)
        {
            return db.Users.Where(u => u.Username == user.UserName).Any();
        }
        public int UserIDByName(string username)
        {
            using (LuvFinderContext db = new LuvFinderContext())
            {
                var userID = db.Users.Where(x => x.Username == username).Select(x => x.Id).SingleOrDefault();
                return userID;
            }
        }
    }
}
