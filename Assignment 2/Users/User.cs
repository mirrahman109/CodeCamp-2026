using RideSharing.Users;

namespace RideSharing.Users
{
public abstract class User
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }

    protected User(string id, string name, string phone)
    {
        ID = id;
        Name = name;
        Phone = phone;
    }

    public abstract void DisplayInfo();
    public abstract string GetRole();
}

}