using LuvFinder.Models;
using LuvFinder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Collections.Generic;
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
            //var lstRegions = db.Regions.Where(r => r.CountryId == countryid).ToList();
            //var region = lstRegions.Where(r => r.Id == regionid).SingleOrDefault();
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

            var userinfo = db.UserInfos.Where(u => u.UserId == userID)
                .Select(info => new ViewModels.UserInfo()
                {
                    LastName = info.LastName ?? string.Empty,
                    FirstName = info.FirstName ?? string.Empty,
                    GenderID = info.GenderId ?? 0,
                    MaritalStatusID = info.MaritalStatusId ?? 0,
                    UserID = info.UserId ?? 0,
                    DOB = info.Dob ?? DateTime.MinValue,
                    SeekingGenderID = info.SeekingGenderId ?? 0,
                    CountryID = info.CountryId,
                    CountryName = "Pakistan",
                    CityID = info.CityId,
                    RegionID = info.RegionId,
                    
                }).SingleOrDefault();


            return Ok(userinfo);
        }

        [HttpPost]
        [Route("profilesaved")]
        public ActionResult SaveProfile([FromBody] System.Text.Json.JsonElement param)
        {
            var username = param.GetProperty("username").ToString();
            var vm = JsonConvert.DeserializeObject<List<ViewModels.ProfileQuestion>>(param.GetProperty("vm").ToString());
            var vminfo = JsonConvert.DeserializeObject<ViewModels.UserInfo>(param.GetProperty("info").ToString());
            var userID = (new UserController(new LuvFinderContext(), _config)).UserIDByName(username);

            List<string> lstErrors = new List<string>();
            
            if (userID == 0 )
                return BadRequest("User Not found");

            try
            {
                if (vminfo != null)
                {
                    var infotodelete = db.UserInfos.Where(u => u.UserId == userID).SingleOrDefault();
                    if (infotodelete != null)
                        db.UserInfos.Remove(infotodelete);

                    db.UserInfos.Add(new Models.UserInfo()
                    {
                        FirstName = vminfo.FirstName,
                        LastName = vminfo.LastName,
                        CityId = vminfo.CityID,
                        CountryId = short.Parse(vminfo.CountryID.ToString()),
                        RegionId = vminfo.RegionID,
                        Dob = DateTime.Now,
                        GenderId = vminfo.GenderID,
                        SeekingGenderId = vminfo.SeekingGenderID,
                        MaritalStatusId = vminfo.MaritalStatusID,
                        UserId = userID
                    });
                }
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
            var lst = db.Users.Select(x => x.Username).ToList();
            lst.AddRange(db.Users.Select(x => x.Username).ToList());
            lst.AddRange(db.Users.Select(x => x.Username).ToList());
            return Ok(lst);

        }
    }
}
