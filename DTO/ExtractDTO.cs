namespace DTO;

public class ExtractDTO
{
    public int Id { get; set; }
    public string ExtractNumber { get; set; }
    public string Type { get; set; } // Final - Partial - FinalForPartial
    public DateTime ExtractDate { get; set; }
    public double ExtractValue { get; set; }
    public double PenaltyValue { get; set; }

    // Calculated fields included
    public double NetValue { get; set; }
    public double Tax { get; set; }
    public double TotalWithTax { get; set; }

    public string InvoiceNumber { get; set; }
    public string Department { get; set; }

    public string WorkOrderNumber { get; set; }
    public string WorkOrderType { get; set; }
}
