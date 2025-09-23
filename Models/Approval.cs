using Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class Approval
{
    [Key]
    public int Id { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? SupportHeadApproval { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? ExecutorHeadApproval { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? CircleDirectorApproval { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? DepartmentDirectorApproval { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? FinanceApproval { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? DeputyApproval { get; set; }

    [Required, MaxLength(50)]
    public string CurrentDepartment { get; set; }

    [Required, MaxLength(50)]
    public string CurrentDepartmentAr { get; set; }

    [Required, MaxLength(500)]
    public string Notes { get; set; }

    [Required, MaxLength(500)]
    public string NotesAr { get; set; }

    [ForeignKey("WorkOrder")]
    public int WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }
}
