using System.ComponentModel.DataAnnotations;

namespace DTO;

public class CreateExtractDTO
{
    [Required, MaxLength(50)]
    public string ExtractNumber { get; set; }

    [Required, MaxLength(50)]
    public string Type { get; set; }

    [Required, MaxLength(50)]
    public string TypeAr { get; set; } // نهائي - جزئي - نهائي للجزئي

    [Required]
    public DateTime ExtractDate { get; set; }

    [Required, Range(0,99999999999999)]
    public double ExtractValue { get; set; }

    [Required, Range(0, 99999999999999)]
    public double PenaltyValue { get; set; }

    [Required, MaxLength(50)]
    public string InvoiceNumber { get; set; }

    [Required, MaxLength(50)]
    public string Department { get; set; } // Projects - Connections - Maintenance

    [Required, MaxLength(50)]
    public string DepartmentAr { get; set; } // المشاريع - التوصيلات - الصيانة

    [Required]
    public string WorkOrderNumber { get; set; }
}
