namespace Task_Manage.Models
{
  public class tblUser
  {
    public int Id { get; set; }
    public string ?Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AccessToken { get; set; }

  }
}
