// filepath: d:\Projects\CodeCamp\Assignment 1\Operations\InvoiceOperations.cs
namespace BusTicketingSystem;

public static class InvoiceOperations
{
    public static void ShowInvoicesOfUser(
        InvoiceService invoiceService, 
        UserService userService,
        TicketService ticketService,
        ScheduleService scheduleService)
    {
        // Show available users
        var users = userService.GetAllUsers();
        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
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

        var invoices = invoiceService.GetInvoicesByUserId(userId);
        if (invoices.Count == 0)
        {
            Console.WriteLine($"No invoices found for {user.UserName}.");
            return;
        }

        Console.WriteLine($"\n--- Invoices for {user.UserName} ---");
        foreach (var invoice in invoices)
        {
            var ticket = ticketService.GetTicketById(invoice.TicketId);
            var schedule = ticket != null ? scheduleService.GetScheduleById(ticket.ScheduleId) : null;

            string status = invoice.IsPaid ? "PAID" : "UNPAID";

            Console.WriteLine($"Invoice ID: {invoice.InvoiceId}");
            Console.WriteLine($"Route: {schedule?.DepartureCity ?? "Unknown"} → {schedule?.ArrivalCity ?? "Unknown"}");
            Console.WriteLine($"Seats: {(ticket != null ? string.Join(", ", ticket.SeatNumbers) : "Unknown")}");
            Console.WriteLine($"Amount: ${invoice.Amount}");
            Console.WriteLine($"Status: {status}");
            Console.WriteLine($"Created: {invoice.CreatedDate}");
            if (invoice.IsPaid)
            {
                Console.WriteLine($"Paid On: {invoice.PaidDate}");
            }
            Console.WriteLine("----------------------------");
        }
    }

    public static void PayInvoice(
        InvoiceService invoiceService, 
        UserService userService,
        TicketService ticketService,
        ScheduleService scheduleService)
    {
        // Show available users
        var users = userService.GetAllUsers();
        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
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

        // Show unpaid invoices
        var unpaidInvoices = invoiceService.GetUnpaidInvoicesByUserId(userId);
        if (unpaidInvoices.Count == 0)
        {
            Console.WriteLine($"No unpaid invoices found for {user.UserName}.");
            return;
        }

        Console.WriteLine($"\n--- Unpaid Invoices for {user.UserName} ---");
        foreach (var invoice in unpaidInvoices)
        {
            var ticket = ticketService.GetTicketById(invoice.TicketId);
            var schedule = ticket != null ? scheduleService.GetScheduleById(ticket.ScheduleId) : null;

            Console.WriteLine($"Invoice ID: {invoice.InvoiceId}");
            Console.WriteLine($"Route: {schedule?.DepartureCity ?? "Unknown"} → {schedule?.ArrivalCity ?? "Unknown"}");
            Console.WriteLine($"Seats: {(ticket != null ? string.Join(", ", ticket.SeatNumbers) : "Unknown")}");
            Console.WriteLine($"Amount: ${invoice.Amount}");
            Console.WriteLine("----------------------------");
        }

        Console.WriteLine("\nEnter Invoice ID to Pay:");
        string invoiceIdInput = Console.ReadLine() ?? string.Empty;

        if (!Guid.TryParse(invoiceIdInput, out Guid invoiceId))
        {
            Console.WriteLine("Invalid Invoice ID format.");
            return;
        }

        Invoice? invoiceToPay = invoiceService.GetInvoiceById(invoiceId);
        if (invoiceToPay == null)
        {
            Console.WriteLine("Invoice not found!");
            return;
        }

        if (invoiceToPay.IsPaid)
        {
            Console.WriteLine("This invoice is already paid!");
            return;
        }

        // Mark invoice as paid
        invoiceToPay.IsPaid = true;
        invoiceToPay.PaidDate = DateTime.Now;

        // Mark ticket as paid
        var paidTicket = ticketService.GetTicketById(invoiceToPay.TicketId);
        if (paidTicket != null)
        {
            paidTicket.IsPaid = true;

            // Move seats from Held to Booked
            var schedule = scheduleService.GetScheduleById(paidTicket.ScheduleId);
            if (schedule != null)
            {
                foreach (var seat in paidTicket.SeatNumbers)
                {
                    schedule.HeldSeats.Remove(seat);
                    schedule.BookedSeats.Add(seat);
                }
            }
        }

        Console.WriteLine("\n========= Payment Successful =========");
        Console.WriteLine($"Invoice ID: {invoiceToPay.InvoiceId}");
        Console.WriteLine($"Amount Paid: ${invoiceToPay.Amount}");
        Console.WriteLine($"Paid On: {invoiceToPay.PaidDate}");
        Console.WriteLine("Seats are now confirmed (X)!");
        Console.WriteLine("======================================");
    }
}