namespace LuvFinder.ViewModels
{
    public class FriendActivity
    {
        public int FromID { get; set; } 
        public UserInfo? FromUserInfo { get; set; }

        public int ToID { get; set; } 
        public UserInfo? ToUserInfo { get; set; } 

        public DateTime? Date { get; set; } 

        public bool? LikeAccepted { get; set; }
        public DateTime? LikeAcceptedDate { get; set; }
    }
}
