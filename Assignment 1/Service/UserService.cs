namespace BusTicketingSystem;

public class UserService
{
    private readonly List<User>_users = new List<User>();
    public void AddUser(User user)
    {
        _users.Add(user);
    }
    public List<User> GetAllUsers()
    {
        return _users;
    }

    public User? GetUserById(Guid userId)
    {
        return _users.FirstOrDefault(u => u.UserId == userId);
    }

    public void ShowUsers()
    {
        foreach (var user in _users)
        {
            Console.WriteLine($"UserName: {user.UserName}, Email: {user.EmailAddress}, Mobile: {user.MobileNumber}");
        }
    }
}