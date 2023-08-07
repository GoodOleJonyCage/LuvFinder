using LuvFinder.Models;
using LuvFinder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

        //[Route("profilequestionnaire")]
        //public ActionResult SaveProfile([FromBody] System.Text.Json.JsonElement param)
        //{

        //    var vm =  (object)param.GetProperty("vm") as List<ViewModels.ProfileQuestion>;

        //    return View();
        //}
    }
}
