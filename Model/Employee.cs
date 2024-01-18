using Newtonsoft.Json;

namespace WebAPI_CRUD.Model
{
    public class Employee
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public int Age { get; set; }
        public string HobbiesJson { get; set; }
        public List<string> Hobbies {
            get
            {
                return string.IsNullOrEmpty(HobbiesJson)
                    ? new List<string>()
                    : JsonConvert.DeserializeObject<List<string>>(HobbiesJson);
            }
            set
            {
                HobbiesJson = JsonConvert.SerializeObject(value);
            }
        }
    }
}
