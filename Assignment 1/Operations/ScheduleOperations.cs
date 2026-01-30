
using Microsoft.VisualBasic;

namespace BusTicketingSystem;
public static class ScheduleOperations
{
    public static void CreateSchedule(ScheduleService scheduleService, BusService busService)
    {   
        var buses = busService.GetAllBuses();
        if(buses.Count == 0)
        {
            Console.WriteLine("No buses available. Please create a bus first.");
            return;
        }
        Console.WriteLine("====Available Buses====");
        foreach (var bus in buses)
        {
            Console.WriteLine($"CoachNumber: {bus.CoachNumber}, BusType: {bus.BusType}, NumberOfSeats: {bus.NumberOfSeats}");
        }

        Console.WriteLine("=======================");
        Console.WriteLine("Enter a CoachNumber:"); 
        string coachNumberInput = Console.ReadLine() ?? string.Empty;

        var selectedBus = busService.GetBusByCoachNumber(coachNumberInput);
        if (selectedBus == null)
        {
            Console.WriteLine("Bus not found.");
            return;
        } 



        Console.WriteLine("Enter Departure City:");
        string departureCity = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Arrival City:");
        string arrivalCity = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Departure Time:(yyyy-MM-dd HH:mm)");
        string departureTime = Console.ReadLine() ?? string.Empty;

        if (!DateTime.TryParse(departureTime, out DateTime departureTimeParsed))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }


        Console.WriteLine("Enter Ticket Price:");
        string ticketPrice = Console.ReadLine() ?? string.Empty;

        if (!decimal.TryParse(ticketPrice, out decimal ticketPriceParsed))
        {
            Console.WriteLine("Invalid price.");
            return;
        }


        Schedule newSchedule = new Schedule
        {
            BusId = selectedBus.BusId,
            DepartureCity = departureCity,
            ArrivalCity = arrivalCity,
            DepartureTime = departureTimeParsed,
            TicketPrice = (int)ticketPriceParsed,
            AvailableSeats = selectedBus.NumberOfSeats
        };

        scheduleService.AddSchedule(newSchedule); 
        Console.WriteLine("Schedule created successfully!");
    }

    public static void ShowSchedules(ScheduleService scheduleService, BusService busService)
    {
        Console.WriteLine("\nEnter Coach Number:");
        string coachNumberInput = Console.ReadLine() ?? string.Empty;

        var selectedBus = busService.GetBusByCoachNumber(coachNumberInput);
        if (selectedBus == null)
        {
            Console.WriteLine("Bus not found.");
            return;
        }

        var schedules = scheduleService.GetSchedulesByBusId(selectedBus.BusId);
        if (schedules.Count == 0)
        {
            Console.WriteLine("No schedules found for this bus.");
            return;
        }

        Console.WriteLine($"\n--- Schedules for Bus {selectedBus.CoachNumber} ---");
        foreach (var schedule in schedules)
        {
            Console.WriteLine($"Schedule ID: {schedule.ScheduleId}");
            Console.WriteLine($"Route: {schedule.DepartureCity} → {schedule.ArrivalCity}");
            Console.WriteLine($"Departure: {schedule.DepartureTime}");
            Console.WriteLine($"Ticket Price: ${schedule.TicketPrice}");
            Console.WriteLine($"Available Seats: {schedule.AvailableSeats}");
            
            // Show seat layout
            Console.WriteLine("\n--- Seat Layout ---");
            Console.WriteLine("[X] = Booked    [ ] = Available\n");

            int totalSeats = selectedBus.NumberOfSeats;
            int seatsPerRow = 4;  // 4 seats per row (2 + aisle + 2)

            for (int seat = 1; seat <= totalSeats; seat++)
            {
                bool isBooked = schedule.BookedSeats.Contains(seat);

                if (isBooked)
                {
                    Console.Write($"[X{seat:D2}]");  // Booked seat
                }
                else
                {
                    Console.Write($"[ {seat:D2}]");  // Available seat
                }

                // Add aisle after 2 seats
                if (seat % seatsPerRow == 2)
                {
                    Console.Write("   ");  // Aisle
                }

                // New row after 4 seats
                if (seat % seatsPerRow == 0)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\n----------------------------");
        }
    }
}