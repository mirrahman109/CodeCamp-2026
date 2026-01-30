// filepath: d:\Projects\CodeCamp\Assignment 1\Service\InvoiceService.cs
namespace BusTicketingSystem;

public class InvoiceService
{
    private List<Invoice> _invoices = new List<Invoice>();

    public void AddInvoice(Invoice invoice)
    {
        _invoices.Add(invoice);
    }

    public List<Invoice> GetAllInvoices()
    {
        return _invoices;
    }

    public Invoice? GetInvoiceById(Guid invoiceId)
    {
        return _invoices.FirstOrDefault(i => i.InvoiceId == invoiceId);
    }

    public List<Invoice> GetInvoicesByUserId(Guid userId)
    {
        return _invoices.Where(i => i.UserId == userId).ToList();
    }

    public Invoice? GetInvoiceByTicketId(Guid ticketId)
    {
        return _invoices.FirstOrDefault(i => i.TicketId == ticketId);
    }

    public List<Invoice> GetUnpaidInvoicesByUserId(Guid userId)
    {
        return _invoices.Where(i => i.UserId == userId && !i.IsPaid).ToList();
    }
}