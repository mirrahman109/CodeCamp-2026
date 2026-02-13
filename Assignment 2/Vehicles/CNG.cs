namespace RideSharing.Vehicles
{
public class CNG : IVehicle
{
    public string GetVehicleType()
    {
        return "CNG";
    }

    public decimal GetBaseFare()
    {
        return 50.00m; // Base fare for CNG
    }
}
}