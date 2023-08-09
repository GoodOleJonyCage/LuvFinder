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
        public ProfileController(LuvFinderContext _db) {

            db = _db;
        }  

        [HttpGet]
        [Route("profilequestionnaire")]
        public ActionResult ProfileQuestionnaire()
        {
            List <ViewModels.ProfileQuestion> lst = new List<ViewModels.ProfileQuestion>();
            using (LuvFinderContext db = new LuvFinderContext())
            {
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
            }
            return Ok(lst);
        }
        
        [HttpPost]
        [Route("profilesaved")]
        public ActionResult SaveProfile([FromBody] System.Text.Json.JsonElement param)
        {
            var username = param.GetProperty("username").ToString();
            var vm = JsonConvert.DeserializeObject<List<ViewModels.ProfileQuestion>>(param.GetProperty("vm").ToString());
            var userID = (new UserController(new LuvFinderContext())).UserIDByName(username);

            List<string> lstErrors = new List<string>();
            
            if (userID == 0 )
                return BadRequest("User Not found");
            
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
    }
}
