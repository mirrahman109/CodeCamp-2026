
public class User
{
  public Guid UserId{ get; set; } = Guid.NewGuid();

  public string UserName{ get; set; } = string.Empty;
  public string EmailAddress{ get; set; } = string.Empty;

  public string MobileNumber{ get; set; } = string.Empty;

}

