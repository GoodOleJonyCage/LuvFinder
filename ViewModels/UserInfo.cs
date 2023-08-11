using LuvFinder.Models;

namespace LuvFinder.ViewModels
{
    public class UserInfo
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public int GenderID { get; set; }
        public int MaritalStatusID { get; set; }
        public int UserID { get; set; }
        public DateTime DOB { get; set; }
        public int SeekingGenderID { get; set; }
        public int CountryID { get; set; }
        public int CityID { get; set; }
        public int RegionID { get; set; }

    }
}
