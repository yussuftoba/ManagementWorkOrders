using System.ComponentModel.DataAnnotations;

namespace DTO;

public class CreateWorkOrderDTO
{
    [Required, MaxLength(50)]
    public string WorkOrderNumber { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime AssignedDate { get; set; }

    [Required, StringLength(200)]
    public string SubscriberName { get; set; } = string.Empty;
    public string SubscriberNameAr { get; set; }= string.Empty;

    [Required, StringLength(300)]
    public string Address { get; set; } = string.Empty;
    public string AddressAr { get; set; } = string.Empty;

    [Required, Range(0, 999999999999999)]
    public double OrderValue { get; set; }
}
