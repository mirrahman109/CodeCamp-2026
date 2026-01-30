
namespace BusTicketingSystem;
public static class UserOperations
{
    public static void CreateUser(UserService userService)
    {
        Console.WriteLine("Enter User Name:");
        string userName = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Email Address:");
        string emailAddress = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Mobile Number:");
        string mobileNumber = Console.ReadLine() ?? string.Empty;

        User newUser = new User
        {
            UserName = userName,
            EmailAddress = emailAddress,
            MobileNumber = mobileNumber
        };

        userService.AddUser(newUser);
        Console.WriteLine("User created successfully!");
    }

    public static void ShowUsers(UserService userService)
    {
        Console.WriteLine("List of Users:");
        userService.ShowUsers();
    }
}