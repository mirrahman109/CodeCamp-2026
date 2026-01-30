namespace BusTicketingSystem;

public static class TicketOperations
{
    public static void BookTicket(
        TicketService ticketService, 
        UserService userService, 
        ScheduleService scheduleService,
        BusService busService,
        InvoiceService invoiceService)
    {
        // Show available users
        var users = userService.GetAllUsers();
        if (users.Count == 0)
        {
            Console.WriteLine("No users found. Please create a user first.");
            return;
        }

        Console.WriteLine("\n--- Available Users ---");
        foreach (var u in users)
        {
            Console.WriteLine($"ID: {u.UserId} | Name: {u.UserName}");
        }

        Console.WriteLine("\nEnter User ID:");
        string userIdInput = Console.ReadLine() ?? string.Empty;

        if (!Guid.TryParse(userIdInput, out Guid userId))
        {
            Console.WriteLine("Invalid User ID format.");
            return;
        }

        User? user = userService.GetUserById(userId);
        if (user == null)
        {
            Console.WriteLine("User not found!");
            return;
        }

        // Show available schedules
        var schedules = scheduleService.GetAllSchedules();
        if (schedules.Count == 0)
        {
            Console.WriteLine("No schedules found. Please create a schedule first.");
            return;
        }

        Console.WriteLine("\n--- Available Schedules ---");
        foreach (var s in schedules)
        {
            var bus = busService.GetBusById(s.BusId);
            Console.WriteLine($"ID: {s.ScheduleId}");
            Console.WriteLine($"Bus: {bus?.CoachNumber ?? "Unknown"}");
            Console.WriteLine($"Route: {s.DepartureCity} → {s.ArrivalCity}");
            Console.WriteLine($"Departure: {s.DepartureTime}");
            Console.WriteLine($"Price: ${s.TicketPrice}");
            Console.WriteLine($"Available Seats: {s.AvailableSeats}");
            Console.WriteLine("----------------------------");
        }

        Console.WriteLine("\nEnter Schedule ID:");
        string scheduleIdInput = Console.ReadLine() ?? string.Empty;

        if (!Guid.TryParse(scheduleIdInput, out Guid scheduleId))
        {
            Console.WriteLine("Invalid Schedule ID format.");
            return;
        }

        Schedule? schedule = scheduleService.GetScheduleById(scheduleId);
        if (schedule == null)
        {
            Console.WriteLine("Schedule not found!");
            return;
        }

        // Show seat layout
        var selectedBus = busService.GetBusById(schedule.BusId);
        if (selectedBus == null)
        {
            Console.WriteLine("Bus not found!");
            return;
        }

        ShowSeatLayout(schedule, selectedBus.NumberOfSeats);

        // Ask user to select specific seat(s)
        Console.WriteLine("\nEnter Seat Number(s) to Book:");
        Console.WriteLine("(For single seat: 5)");
        Console.WriteLine("(For multiple seats: 1,2,5)");
        string seatInput = Console.ReadLine() ?? string.Empty;

        string[] seatStrings = seatInput.Split(',');
        List<int> selectedSeats = new List<int>();

        foreach (var seatStr in seatStrings)
        {
            if (int.TryParse(seatStr.Trim(), out int seatNumber))
            {
                // Check if seat is valid
                if (seatNumber < 1 || seatNumber > selectedBus.NumberOfSeats)
                {
                    Console.WriteLine($"Seat {seatNumber} is invalid.");
                    return;
                }

                // Check if seat is already booked
                if (schedule.BookedSeats.Contains(seatNumber))
                {
                    Console.WriteLine($"Seat {seatNumber} is already booked.");
                    return;
                }

                // Check if seat is already held
                if (schedule.HeldSeats.Contains(seatNumber))
                {
                    Console.WriteLine($"Seat {seatNumber} is already held.");
                    return;
                }

                selectedSeats.Add(seatNumber);
            }
            else
            {
                Console.WriteLine($"Invalid seat number: {seatStr}");
                return;
            }
        }

        if (selectedSeats.Count == 0)
        {
            Console.WriteLine("No seats selected.");
            return;
        }

        // Hold the seats
        foreach (var seat in selectedSeats)
        {
            schedule.HeldSeats.Add(seat);
        }
        schedule.AvailableSeats -= selectedSeats.Count;

        // Calculate total price
        decimal totalPrice = selectedSeats.Count * schedule.TicketPrice;

        // Create ticket
        Ticket newTicket = new Ticket
        {
            UserId = userId,
            ScheduleId = scheduleId,
            SeatNumbers = selectedSeats,
            TotalPrice = totalPrice,
            IsPaid = false
        };

        ticketService.AddTicket(newTicket);

        Invoice newInvoice = new Invoice
        {
            UserId = user.UserId,
            TicketId = newTicket.TicketId,
            Amount = totalPrice
        };

        invoiceService.AddInvoice(newInvoice);



        Console.WriteLine("\n========= Booking Held =========");
        Console.WriteLine($"Ticket ID: {newTicket.TicketId}");
        Console.WriteLine($"Invoice ID: {newInvoice.InvoiceId}");  // Show Invoice ID
        Console.WriteLine($"User: {user.UserName}");
        Console.WriteLine($"Route: {schedule.DepartureCity} → {schedule.ArrivalCity}");
        Console.WriteLine($"Seat(s): {string.Join(", ", selectedSeats)}");
        Console.WriteLine($"Total Price: ${totalPrice}");
        Console.WriteLine($"Status: HELD (Pay invoice to confirm)");
        Console.WriteLine("=================================");
    }

    public static void ShowUserTickets(TicketService ticketService, UserService userService, ScheduleService scheduleService)
    {
        Console.WriteLine("\nEnter User ID:");
        string userIdInput = Console.ReadLine() ?? string.Empty;

        if (!Guid.TryParse(userIdInput, out Guid userId))
        {
            Console.WriteLine("Invalid User ID format.");
            return;
        }

        User? user = userService.GetUserById(userId);
        if (user == null)
        {
            Console.WriteLine("User not found!");
            return;
        }

        var tickets = ticketService.GetTicketsByUserId(userId);
        if (tickets.Count == 0)
        {
            Console.WriteLine($"No tickets found for {user.UserName}.");
            return;
        }

        Console.WriteLine($"\n--- Tickets for {user.UserName} ---");
        foreach (var ticket in tickets)
        {
            var schedule = scheduleService.GetScheduleById(ticket.ScheduleId);

            Console.WriteLine($"Ticket ID: {ticket.TicketId}");
            Console.WriteLine($"Route: {schedule?.DepartureCity ?? "Unknown"} → {schedule?.ArrivalCity ?? "Unknown"}");
            Console.WriteLine($"Departure: {schedule?.DepartureTime}");
            Console.WriteLine($"Seats: {string.Join(", ", ticket.SeatNumbers)}");
            Console.WriteLine($"Total Price: ${ticket.TotalPrice}");
            Console.WriteLine($"Booking Date: {ticket.BookingDate}");
            Console.WriteLine("----------------------------");
        }
    }

    private static void ShowSeatLayout(Schedule schedule, int totalSeats)
    {
        Console.WriteLine("\n--- Seat Layout ---");
        Console.WriteLine("[X] = Booked (Paid)    [H] = Held    [ ] = Available\n");

        int seatsPerRow = 4;

        for (int seat = 1; seat <= totalSeats; seat++)
        {
            bool isBooked = schedule.BookedSeats.Contains(seat);
            bool isHeld = schedule.HeldSeats.Contains(seat);

            if (isBooked)
            {
                Console.Write($"[X{seat:D2}]");
            }
            else if (isHeld)
            {
                Console.Write($"[H{seat:D2}]");
            }
            else
            {
                Console.Write($"[ {seat:D2}]");
            }

            if (seat % seatsPerRow == 2)
            {
                Console.Write("   ");
            }

            if (seat % seatsPerRow == 0)
            {
                Console.WriteLine();
            }
        }
        Console.WriteLine();
    }
}