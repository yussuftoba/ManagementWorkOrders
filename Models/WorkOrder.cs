using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class WorkOrder
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string WorkOrderNumber { get; set; }

    [Required, MaxLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string Type { get; set; }

    [NotMapped] // Derived field: WorkOrderNumber-Type
    public string WorkOrderCode => $"{WorkOrderNumber}-{Type}";

    [Required, DataType(DataType.Date)]
    public DateTime AssignmentDate { get; set; }

    // 🔹 English

    [Required, StringLength(200)]
    public string SubscriberName { get; set; }

    [Required, StringLength(300)]
    public string Address { get; set; }

    // 🔹 Arabic
    [Required, StringLength(200)]
    public string SubscriberNameAr { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string AddressAr { get; set; } = string.Empty;

    [Required, Column(TypeName = "decimal(18,2)")]
    public double Value { get; set; }
}

