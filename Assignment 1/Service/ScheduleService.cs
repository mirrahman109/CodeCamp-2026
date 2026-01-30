namespace BusTicketingSystem;

public class ScheduleService
{
    private readonly List<Schedule>_schedules = new List<Schedule>();
    public void AddSchedule(Schedule schedule)
    {
        _schedules.Add(schedule);
    }
    public List<Schedule> GetAllSchedules()
    {
        return _schedules;
    }
    public Schedule? GetScheduleById(Guid scheduleId)
    {
        return _schedules.FirstOrDefault(s => s.ScheduleId == scheduleId);
    }

    public List<Schedule> GetSchedulesByBusId(Guid busId)
    {
        return _schedules.Where(s => s.BusId == busId).ToList();
    }
    public void ShowSchedules()
    {
        foreach (var schedule in _schedules) 
        {
            Console.WriteLine($"DepartureCity: {schedule.DepartureCity}, ArrivalCity: {schedule.ArrivalCity}, DepartureTime: {schedule.DepartureTime}, TicketPrice: {schedule.TicketPrice}");
        }
    }
}