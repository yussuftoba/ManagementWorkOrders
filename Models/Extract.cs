using Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class Extract
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string ExtractNumber { get; set; }

    [Required, MaxLength(30)]
    public string Type { get; set; } // Final - Partial - FinalForPartial

    [Required, MaxLength(30)]
    public string TypeAr { get; set; } // نهائي - جزئي - نهائي للجزئي

    [Required, DataType(DataType.Date)]
    public DateTime ExtractDate { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public double ExtractValue { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public double PenaltyValue { get; set; }

    [NotMapped]
    public double NetValue => ExtractValue - PenaltyValue;

    [NotMapped]
    public double Tax => NetValue * 0.15;

    [NotMapped]
    public double TotalWithTax => NetValue + Tax;

    [Required, MaxLength(50)]
    public string InvoiceNumber { get; set; }

    [Required, MaxLength(50)]
    public string Department { get; set; } // Projects - Connections - Maintenance

    [Required, MaxLength(50)]
    public string DepartmentAr { get; set; } // المشاريع - التوصيلات - الصيانة

    [ForeignKey("WorkOrder")]
    public int WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }
}
