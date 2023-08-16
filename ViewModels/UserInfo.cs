using LuvFinder.Models;

namespace LuvFinder.ViewModels
{
    
    public class Country
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Region 
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class City
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class UserInfo
    {

        public string UserName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        
        public int GenderID { get; set; }
        public string Gender { get; set; } = string.Empty;

        public int SeekingGenderID { get; set; }
        public string SeekingGender { get; set; } = string.Empty;

        public int MaritalStatusID { get; set; }
        public string MaritalStatus { get; set; } = string.Empty;

        public int Age {  get; set; }   
        public int UserID { get; set; }
        
        public DateTime DOB { get; set; }
        
        public int CountryID { get; set; }
        public string CountryName { get; set; } = string.Empty;

        public int CityID { get; set; }
        public string CityName { get; set; } = string.Empty;

        public int RegionID { get; set; }
        public string RegionName { get; set; } = string.Empty;

        public List<Country> Countries = new List<Country>();
        public List<Region> Regions = new List<Region>();
        public List<City> Cities = new List<City>();

    }
}
