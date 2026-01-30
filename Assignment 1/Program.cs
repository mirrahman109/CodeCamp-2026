namespace BusTicketingSystem;
public  class Program
  
{   static UserService UserService = new UserService();
    static BusService BusService = new BusService();
    static ScheduleService ScheduleService = new ScheduleService();
    static TicketService TicketService = new TicketService();
    static InvoiceService InvoiceService = new InvoiceService();
    
    public static void Main(string[] args)
    {
        bool Running = true;
        

        while (Running)
        {   
            Console.Clear();
            Console.WriteLine("================Bus Ticketing System====================");
            Console.WriteLine("1.Create User");
            Console.WriteLine("2.Show Users");
            Console.WriteLine("3.Create Bus");
            Console.WriteLine("4.Show Buses");
            Console.WriteLine("5.Create Schedule");
            Console.WriteLine("6.Show Schedules");
            Console.WriteLine("7.Book Ticket");
            Console.WriteLine("8.Show Invoices of a user");
            Console.WriteLine("9.Pay Invoice");
            Console.WriteLine("10.Show Tickets of a User");
            Console.WriteLine("0.Exit");
            Console.WriteLine("=======================================================");
            Console.Write("Enter your choice: ");

            string ? input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    UserOperations.CreateUser(UserService);
                    break;
                case "2":
                    UserOperations.ShowUsers(UserService);
                    break; 
                case "3":
                    BusOperations.CreateBus(BusService);
                    break;
                case "4":
                    BusOperations.ShowBuses(BusService);
                    break; 
                case "5":
                    ScheduleOperations.CreateSchedule(ScheduleService, BusService);
                    break;
                case "6":
                    ScheduleOperations.ShowSchedules(ScheduleService, BusService);
                    break;
                
                case "7":
                    TicketOperations.BookTicket( TicketService, UserService , ScheduleService, BusService, InvoiceService);
                    break; 
                case "8":
                    InvoiceOperations.ShowInvoicesOfUser(InvoiceService, UserService, TicketService, ScheduleService);
                    break;
                case "9":
                    InvoiceOperations.PayInvoice(InvoiceService, UserService, TicketService, ScheduleService);
                    break;
                case "10":
                    TicketOperations.ShowUserTickets(TicketService, UserService, ScheduleService);
                    break; 
                case "0":
                    Running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            if(Running)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
       }
   }

}