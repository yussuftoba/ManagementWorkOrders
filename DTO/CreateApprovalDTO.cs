using System.ComponentModel.DataAnnotations;

namespace DTO;

public class CreateApprovalDTO
{
    public string WorkOrderNumber { get; set; }
    public DateTime? SupportHeadApproval { get; set; }
    public DateTime? ExecutorHeadApproval { get; set; }
    public DateTime? CircleDirectorApproval { get; set; }
    public DateTime? DepartmentDirectorApproval { get; set; }
    public DateTime? FinanceApproval { get; set; }
    public DateTime? DeputyApproval { get; set; }

    [Required, MaxLength(50)]
    public string CurrentDepartment { get; set; }

    [Required, MaxLength(50)]
    public string CurrentDepartmentAr {  get; set; }

    [Required, MaxLength(500)]
    public string Notes { get; set; }

    [Required, MaxLength(500)]
    public string NotesAr { get; set; }
}
