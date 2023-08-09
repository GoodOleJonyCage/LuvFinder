using LuvFinder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace LuvFinder.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        const string roleAdmin = "Admin";
        const string roleUser = "User";

        private readonly LuvFinderContext db ;
        private readonly IConfiguration _config;
        public UserController(LuvFinderContext _db, IConfiguration config)
        {
            db = _db;
            _config = config;
        }

        private string GenerateToken(Models.User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
                new Claim(ClaimTypes.Role,(user.IsAdmin.HasValue ? user.IsAdmin.Value:false) ? roleAdmin : roleUser)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_config["Timeout"])),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }


        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public ActionResult Login([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
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

            var userExists = db.Users.Where(u => u.Username == user.UserName && u.Password == user.Password)
                            .SingleOrDefault();

            if (userExists != null)
                return Ok(GenerateToken(userExists));
            else
                return BadRequest("Invalid username/password");
                
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public ActionResult Register([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
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
                var userID = db.Users.Where(x => x.Username == username).Select(x => x.Id).SingleOrDefault();
                return userID;
        }
    }
}
