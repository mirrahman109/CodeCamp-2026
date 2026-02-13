namespace RideSharing.Vehicles
{
  public class Car : IVehicle
{
    public string GetVehicleType()
    {
        return "Car";
    }

    public decimal GetBaseFare()
    {
        return 10.0m; // Base fare for Car
    }
}

}
