namespace TrackerApi.Common.Model.RequestModels.User
{
    public class CreateUserRequestModel
    {
        public string NameSurname { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
    }
}
