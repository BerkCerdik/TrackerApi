namespace TrackerApi.Data.Entity
{
    public class Users : BaseEntity
    {
        public string NameSurname { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }


    }
}
