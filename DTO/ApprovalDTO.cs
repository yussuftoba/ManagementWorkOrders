namespace DTO;

public class ApprovalDTO
{
    public int Id { get; set; }
    public string WorkOrderNumber { get; set; }
    public string WorkOrderType {  get; set; }
    public DateTime? SupportHeadApproval { get; set; }
    public DateTime? ExecutorHeadApproval { get; set; }
    public DateTime? CircleDirectorApproval { get; set; }
    public DateTime? DepartmentDirectorApproval { get; set; }
    public DateTime? FinanceApproval { get; set; }
    public DateTime? DeputyApproval { get; set; }
    public string CurrentDepartment { get; set; }
    public string Notes { get; set; }
}
