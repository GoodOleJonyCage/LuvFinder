using LuvFinder.Models;
using LuvFinder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LuvFinder.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly LuvFinderContext db;
        private readonly IConfiguration _config;
        public ProfileController(LuvFinderContext _db, IConfiguration config)
        {
            db = _db;
            _config = config;
        }

        private List<ProfileQuestion> GetProfileQuestions()
        {
            List<ViewModels.ProfileQuestion> lst = new List<ViewModels.ProfileQuestion>();

            lst = db.Profiles.Select(profilequestion =>
            new ViewModels.ProfileQuestion()
            {
                ID = profilequestion.Id,
                Question = new ViewModels.Question()
                {
                    QuestionID = profilequestion.QuestionId,
                    Text = profilequestion.Question.Text,
                    ShortDesc = profilequestion.Question.ShortDesc,
                    QuestionTypeID = profilequestion.Question.QuestionType,
                    QuestionType = (ViewModels.QuestionType)profilequestion.Question.QuestionType,
                    Answers = profilequestion.Question.Answers
                        .Select(answer => new ViewModels.Answer()
                        {
                            ID = answer.Id,
                            Text = answer.Text,
                        })
                        .ToList()
                }

            }).ToList();
            return lst;
        }

        private List<UserProfileQuestion> GetUserProfileQuestions(int userID)
        {
            var lst = new List<ViewModels.UserProfileQuestion>();

            lst = db.UserProfiles
                .Where(userProfile => userProfile.UserId == userID)
                .Select(profilequestion =>
                new ViewModels.UserProfileQuestion()
                {
                    ID = profilequestion.Id,
                    UserID = profilequestion.UserId,
                    QuestionID = profilequestion.QuestionId,
                    AnswerID = profilequestion.AnswerId ?? 0,
                    AnswerText = profilequestion.AnswerText ?? string.Empty,
                    Date = profilequestion.Date,
                    Selected = profilequestion.Selected ?? false
                })
                .ToList();

            return lst;
        }

        [HttpGet]
        [Route("profilequestionnaire")]
        public ActionResult ProfileQuestionnaire()
        {
            List<ProfileQuestion> lst = GetProfileQuestions();
            return Ok(lst);
        }

        [HttpPost]
        [Route("userprofile")]
        public ActionResult UserProfile([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
        {
            var username = userParams.GetProperty("username").ToString();
            var userID = (new UserController(new LuvFinderContext(), _config)).UserIDByName(username);

            List<ProfileQuestion> lstProfileQuestions = GetProfileQuestions();
            List<UserProfileQuestion> lstUserProfile = GetUserProfileQuestions(userID);
            if(lstUserProfile.Count == 0) //profile not created
            {
                return BadRequest("User profile not found");
            }
            
            lstProfileQuestions.ForEach(question =>
            {
                question.AnswerText = lstUserProfile?
                                            .AsQueryable()?
                                            .Where(entry => entry.QuestionID == question.ID &&
                                                             entry.AnswerID  == 0)
                                            .SingleOrDefault()?
                                            .AnswerText ?? string.Empty;

                question?.Question.Answers.ForEach(answer =>
                {
                    answer.Selected = lstUserProfile?
                                            .AsQueryable()?
                                            .Where(entry => entry.QuestionID == question.ID &&
                                                             entry.AnswerID == answer.ID)
                                            .SingleOrDefault()?.Selected ?? false;
                });
            });
           
            return Ok(lstProfileQuestions);
        }

        [HttpGet]
        [Route("countries")]
        public ActionResult Countries()
        {

            var lst = db.Countries.Select(c => new ViewModels.Country()
            {
                ID = c.Id,
                Name = c.Name
            }).ToList();

            return Ok(lst);
        }

        [HttpPost]
        [Route("regions")]
        public ActionResult Regions([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
        {
            var countryid = Int32.Parse(userParams.GetProperty("countryid").ToString());
            var lst = db.Regions
                .Where( r => r.CountryId == countryid)
                .Select(c => new ViewModels.Region()
            {
                ID = c.Id,
                Name = c.Name
            }).ToList();

            return Ok(lst);
        }

        [HttpPost]
        [Route("cities")]
        public ActionResult Cities([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
        {
            var regionid = Int32.Parse(userParams.GetProperty("regionid").ToString());
            var lst = db.Cities.Where(c => c.RegionId == regionid)
                        .Select(c => new ViewModels.City()
                        { 
                            ID = c.Id,
                            Name =c.Name
                        }).ToList();

            return Ok(lst);
        }

        [HttpPost]
        [Route("userinfo")]
        public ActionResult UserInfo([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
        {
            var username = userParams.GetProperty("username").ToString();
            var userID = (new UserController(new LuvFinderContext(), _config)).UserIDByName(username);

            ViewModels.UserInfo? userinfo = GetUserInfo(userID);

            if (userinfo == null)
                return BadRequest("User profile not found");

            return Ok(userinfo);
        }

        private ViewModels.UserInfo? GetUserInfo(int userID)
        {
            var userinfo =  db.UserInfos.Where(u => u.UserId == userID)
                .Select(info => new ViewModels.UserInfo()
                {
                    UserName =info.User.Username??string.Empty,
                    LastName = info.LastName ?? string.Empty,
                    FirstName = info.FirstName ?? string.Empty,
                    GenderID = info.GenderId ?? 0,
                    MaritalStatusID = info.MaritalStatusId ?? 0,
                    UserID = info.UserId ?? 0,
                    Age = 42,
                    DOB = info.Dob ?? DateTime.MinValue,
                    SeekingGenderID = info.SeekingGenderId ?? 0,
                    CountryID = info.CountryId,
                    CityID = info.CityId,
                    RegionID = info.RegionId,
                }).SingleOrDefault();
            if (userinfo != null)
            {
                userinfo.Gender = db.UserGenders.Where(g => g.Id == userinfo.GenderID).SingleOrDefault()?.Gender ?? string.Empty;
                userinfo.MaritalStatus = db.UserMaritalStatuses?.Where(s => s.Id == userinfo.MaritalStatusID).SingleOrDefault()?.Status ?? string.Empty;
                userinfo.SeekingGender = db.UserGenders?.Where(g => g.Id == userinfo.SeekingGenderID).SingleOrDefault()?.Gender ?? string.Empty;
                userinfo.CountryName = db.Countries?.Where(c => c.Id == userinfo.CountryID).SingleOrDefault()?.Name ?? string.Empty;
                userinfo.RegionName = db.Regions?.Where(c => c.Id == userinfo.RegionID).SingleOrDefault()?.Name ?? string.Empty;
                userinfo.CityName = db.Cities?.Where(c => c.Id == userinfo.CityID).SingleOrDefault()?.Name ?? string.Empty;
            }
            return userinfo;
        }

        [HttpGet]
        [Route("initializeduserinfo")]
        public ActionResult InitializedUserInfo()
        {
            //return an initialized object to instantiate the vm
            return Ok(new ViewModels.UserInfo());
        }

        [HttpPost]
        [Route("saveprofile")]
        public ActionResult SaveProfile([FromBody] System.Text.Json.JsonElement param)
        {
            var username = param.GetProperty("username").ToString();
            var vm = JsonConvert.DeserializeObject<List<ViewModels.ProfileQuestion>>(param.GetProperty("vm").ToString());
            var vminfo = JsonConvert.DeserializeObject<ViewModels.UserInfo>(param.GetProperty("info").ToString());
            var userID = (new UserController(new LuvFinderContext(), _config)).UserIDByName(username);

            List<string> lstErrors = new List<string>();
            
            if (userID == 0 )
                return BadRequest("User Not found");

            if (string.IsNullOrEmpty(vminfo?.FirstName))
            {
                lstErrors.Add($"First name required");
            }
            if (string.IsNullOrEmpty(vminfo?.LastName))
            {
                lstErrors.Add($"Last name required");
            }
            if (!vminfo.DOB.HasValue)
            {
                lstErrors.Add($"Date of birth required");
            }
            
            if (vminfo?.GenderID == 0)
            {
                lstErrors.Add($"Gender required");
            }
            if (vminfo?.SeekingGenderID == 0)
            {
                lstErrors.Add($"Seeking Gender required");
            }
            if (vminfo?.MaritalStatusID == 0)
            {
                lstErrors.Add($"Marital Status required");
            }
            if (vminfo?.CountryID == 0)
            {
                lstErrors.Add($"Country required");
            }
            if (vminfo?.RegionID == 0)
            {
                lstErrors.Add($"Region required");
            }
            if (vminfo?.CityID == 0)
            {
                lstErrors.Add($"City required");
            }

            if (lstErrors.Count > 0)
            {
                return BadRequest(lstErrors);
            }

            try
            {
                var infotodelete = db.UserInfos.Where(u => u.UserId == userID).SingleOrDefault();
                if (infotodelete != null)
                    db.UserInfos.Remove(infotodelete);

                db.UserInfos.Add(new Models.UserInfo()
                {
                    FirstName = vminfo?.FirstName,
                    LastName = vminfo?.LastName,
                    CityId = vminfo?.CityID??0,
                    CountryId = short.Parse((vminfo?.CountryID??0).ToString()),
                    RegionId = vminfo?.RegionID??0,
                    Dob = vminfo?.DOB,
                    GenderId = vminfo?.GenderID,
                    SeekingGenderId = vminfo?.SeekingGenderID,
                    MaritalStatusId = vminfo?.MaritalStatusID,
                    UserId = userID
                });
            }
            catch (Exception exc)
            {
                lstErrors.Add($"Error while updating basic info {exc.Message}");
                return BadRequest(lstErrors);
            }
            
            if (vm != null)
            {
                vm.ToList().ForEach(question =>
                {
                    bool questionReponseInvalid = false;
                    if (question.Question.QuestionType == ViewModels.QuestionType.OpenText)
                    {
                        if (string.IsNullOrEmpty(question.AnswerText))
                            questionReponseInvalid = true;
                    }
                    else
                    {
                        if (question.Question.Answers.All(ans => !ans.Selected))
                        {
                            questionReponseInvalid = true;
                        }
                    }
                    
                    if (questionReponseInvalid)
                    {
                        lstErrors.Add($"Value required for {question.Question.ShortDesc}");
                    }
                    else
                    {
                        if ( question.Question.Answers.Count == 0 ) {

                            db.UserProfiles.Add(new UserProfile()
                            {
                                UserId = userID,
                                QuestionId = question.Question.QuestionID,
                                AnswerText = string.IsNullOrEmpty(question.AnswerText) ? string.Empty : question.AnswerText,
                                Selected = false
                            });
                        }
                        else
                        question.Question.Answers.ForEach(answer =>
                        {
                            db.UserProfiles.Add(new UserProfile()
                            {
                                UserId = userID,
                                QuestionId = question.Question.QuestionID,
                                AnswerId = answer.ID,
                                AnswerText = string.Empty,
                                Selected = answer.Selected
                            });
                        });
                    }
                });

                if (lstErrors.Count == 0)
                {
                    db.UserProfiles.RemoveRange(db.UserProfiles.Where(u => u.UserId == userID).ToList());
                    db.SaveChanges();
                }
                else
                    return BadRequest(lstErrors);
            }
            
            return Ok(true);
        }

        [HttpGet]
        [Route("maritalstatuses")]
        public ActionResult MaritalStatuses()
        {
            var lst = db.UserMaritalStatuses
                        .Select(m => new ViewModels.MaritalStatus()
                        {
                            Id = m.Id,
                            Name = m.Status??string.Empty,
                        }).ToList();

            return Ok(lst);
        }

        [HttpGet]
        [Route("genders")]
        public ActionResult Genders()
        {
            var lst = db.UserGenders
                        .Select(m => new ViewModels.Gender()
                        {
                            Id = m.Id,
                            Name = m.Gender ?? string.Empty,
                        }).ToList();

            return Ok(lst);
        }

        [HttpGet]
        [Route("profiles")]
        public ActionResult GetProfiles()
        {
            //put icon status in this call instead of making separte calls for each profile
            var lst =(from u in db.Users join i in db.UserInfos on u.Id equals i.UserId
            select new ViewModels.UserInfo()
            {
                UserName = u.Username,
                LastName = i.LastName ?? string.Empty,
                FirstName = i.FirstName ?? string.Empty,
                GenderID = i.GenderId ?? 0,
                SeekingGenderID = i.SeekingGenderId ?? 0,
                MaritalStatusID = i.MaritalStatusId ?? 0,
                CountryID = i.CountryId,
                CityID = i.CityId,
                RegionID = i.RegionId,
                DOB = i.Dob??DateTime.MinValue,
            }).ToList();

            lst.ForEach(entry =>
            {
                entry.Age = entry.DOB.HasValue ? CalculateAge(entry.DOB.Value) : 0;
                entry.Gender = db.UserGenders.Where(g => g.Id == entry.GenderID)?.SingleOrDefault()?.Gender ?? string.Empty;
                entry.SeekingGender = db.UserGenders.Where(g => g.Id == entry.SeekingGenderID).SingleOrDefault()?.Gender ?? string.Empty;
                entry.MaritalStatus = db.UserMaritalStatuses.Where(m => m.Id == entry.MaritalStatusID).SingleOrDefault()?.Status ?? string.Empty;
                entry.CountryName = db.Countries.Where(c => c.Id == entry.CountryID).SingleOrDefault()?.Name ?? string.Empty;
                entry.CityName = db.Cities.Where(c => c.Id == entry.CityID).SingleOrDefault()?.Name ?? string.Empty;
                entry.RegionName = db.Regions.Where(r => r.Id == entry.RegionID).SingleOrDefault()?.Name ?? string.Empty;
            });

            
            return Ok(lst);
        }


        [HttpPost]
        [Route("friendrequestcount")]
        public ActionResult GetFriendRequestCount([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
        {
            var usernameTo = userParams.GetProperty("usernameto").ToString();
            var userIDTo = (new UserController(new LuvFinderContext(), _config)).UserIDByName(usernameTo);
            var count = 0;
            try
            {
                count = db.UserLikes.Count(l => l.ToId == userIDTo && 
                                                l.LikeAccepted == false );
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }

            return Ok(count);
        }

        [HttpPost]
        [Route("likeuser")]
        public ActionResult LikeUser([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
        {
            var usernameFrom = userParams.GetProperty("usernamefrom").ToString();
            var usernameTo = userParams.GetProperty("usernameto").ToString();
            
            var userIDFrom = (new UserController(new LuvFinderContext(), _config)).UserIDByName(usernameFrom);
            var userIDTo = (new UserController(new LuvFinderContext(), _config)).UserIDByName(usernameTo);

            try
            {
                if (!db.UserLikes.Any(l => l.FromId == userIDFrom && l.ToId == userIDTo))
                {
                    db.UserLikes.Add(new UserLike()
                    {
                        FromId = userIDFrom,
                        ToId = userIDTo,
                    });
                    db.SaveChanges();
                }
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
            
            return Ok(true);
        }

        [HttpPost]
        [Route("likeuserstatus")]
        public ActionResult LikeUserStatus([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
        {
            var usernameFrom = userParams.GetProperty("usernamefrom").ToString();
            var usernameTo = userParams.GetProperty("usernameto").ToString();

            var userIDFrom = (new UserController(new LuvFinderContext(), _config)).UserIDByName(usernameFrom);
            var userIDTo = (new UserController(new LuvFinderContext(), _config)).UserIDByName(usernameTo);

            var hasLiked = false;

            try
            {
                hasLiked = db.UserLikes.Any(l => l.FromId == userIDFrom && l.ToId == userIDTo);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }

            return Ok(hasLiked);
        }

        [HttpPost]
        [Route("activityfriends")]
        public ActionResult ActivityFriends([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
        {
            var usernameTo = userParams.GetProperty("usernameto").ToString();
            var userIDTo = (new UserController(new LuvFinderContext(), _config)).UserIDByName(usernameTo);

            var lst = new List<FriendActivity>();

            try
            {
                lst = db.UserLikes
                          .Where(l => l.ToId == userIDTo)
                          .OrderBy( l => l.Date)
                          .Select(l => new FriendActivity()
                          {
                              FromID = l.FromId,
                              ToID = l.ToId,
                              Date = l.Date,
                              LikeAccepted = l.LikeAccepted??false,
                             LikeAcceptedDate = l.LikeAcceptedDate 

                          }).ToList();

                lst.ForEach(l =>
                {
                    l.FromUserInfo = GetUserInfo(l.FromID);
                    l.ToUserInfo = GetUserInfo(l.ToID);
                 });

            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }

            return Ok(lst);
        }

        [HttpPost]
        [Route("startfriendship")]
        public ActionResult StartFriendShip([Microsoft.AspNetCore.Mvc.FromBody] System.Text.Json.JsonElement userParams)
        {
            var usernameFrom = userParams.GetProperty("usernamefrom").ToString();
            var usernameTo = userParams.GetProperty("usernameto").ToString();

            var userIDFrom = (new UserController(new LuvFinderContext(), _config)).UserIDByName(usernameFrom);
            var userIDTo = (new UserController(new LuvFinderContext(), _config)).UserIDByName(usernameTo);
            
            var activity = new FriendActivity();
            
            try
            {
                var entry  = db.UserLikes.Where(l => l.FromId == userIDFrom && l.ToId == userIDTo)
                            .SingleOrDefault();

                if (entry != null)
                {
                    entry.LikeAccepted = true;
                    entry.LikeAcceptedDate = DateTime.Now;
                    db.SaveChanges();
                }

                  activity = db.UserLikes.Where(l => l.FromId == userIDFrom && l.ToId == userIDTo)
                        .Select(l => new FriendActivity()
                         {
                             FromID = l.FromId,
                             ToID = l.ToId,
                             Date = l.Date,
                             LikeAccepted = l.LikeAccepted ?? false,
                             LikeAcceptedDate = l.LikeAcceptedDate
                         })
                        .SingleOrDefault();
                
                if(activity!= null)
                    activity.FromUserInfo = GetUserInfo(activity.FromID);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }

            return Ok(activity);
        }

        private int CalculateAge(DateTime birthdate)
        {
            // Save today's date.
            var today = DateTime.Today;
            // Calculate the age.
            var age = today.Year - birthdate.Year;
            // Go back to the year in which the person was born in case of a leap year
            if (birthdate.Date > today.AddYears(-age)) age--;
            return age;

        }
    }
}
