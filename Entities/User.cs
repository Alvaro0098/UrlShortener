using SQLite;
using SQLiteNetExtensions.Attributes;

namespace url_shortener.Entities;

public class User{ 
        [PrimaryKey, AutoIncrement]
        public int Id {get; set;} 
        public String Username {get; set;}
        public String FirstName {get; set;}

        public int RemainingUrls { get; set; } = 10;
        public String LastName {get; set;}
        public String Email {get; set;}
        
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<XYZ> Urls { get; set; }
}