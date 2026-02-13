using System;

namespace RideSharing.Users
{
public class Rider : User
{
    public decimal WalletBalance { get; set; }

    public Rider(string id, string name, string phone, decimal walletBalance = 0.0m)
        : base(id, name, phone)
    {
        ID = id;
        Name = name;
        Phone = phone;
        WalletBalance = walletBalance;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Rider ID: {ID}, Name: {Name}, Phone: {Phone}, Wallet Balance: {WalletBalance}");
    }

    public override string GetRole()
    {
        return "Rider";
    }
}
}