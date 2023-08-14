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
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public int GenderID { get; set; }
        public int MaritalStatusID { get; set; }
        public int UserID { get; set; }
        public DateTime DOB { get; set; }
        public int SeekingGenderID { get; set; }
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        
        public int CityID { get; set; }
        public string CityName { get; set; }
        
        public int RegionID { get; set; }
        public string RegionName { get; set; }

        public List<Country> Countries = new List<Country>();
        public List<Region> Regions = new List<Region>();
        public List<City> Cities = new List<City>();

    }
}
