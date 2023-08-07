using System.Runtime.CompilerServices;

namespace LuvFinder.ViewModels
{
    
    
    public enum QuestionType
    {
        Check = 1,
        Radio = 2,
        OpenText = 3
    }

    public class Question
    {
        public int QuestionID { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ShortDesc { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public int QuestionTypeID { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }
    
    public class Answer
    {
        public int ID { get; set; }
        public string Text { get; set; } = string.Empty;
    }
    
    public class ProfileQuestion
    {
        public int ID { get; set; }
        public Question Question { get; set; } = new Question();
        public string AnswerText { get; set; } = string.Empty;
    }
}
