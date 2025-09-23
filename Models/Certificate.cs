using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class Certificate
{
    [Key]
    public int Id { get; set; }

    public bool Form190 { get; set; }
    public bool Form200 { get; set; }
    public bool Procedure211 { get; set; }
    public bool DrillingForm { get; set; }
    public bool ScrapingForm { get; set; }

    public DateTime? ContractorReceivedDate { get; set; }

    [NotMapped]
    public int? WorkOrderDuration
    {
        get
        {
            if (ContractorReceivedDate.HasValue && WorkOrder != null)
            {
                return (ContractorReceivedDate.Value - WorkOrder.AssignmentDate).Days;
            }
            return null;
        }
    }

    public bool AchievementCertificateAttached { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public double? CertificateValue { get; set; }

    // ✅ English + Arabic for PaymentStatus
    [Required, MaxLength(50)]
    public string PaymentStatus { get; set; }

    [Required, MaxLength(50)]
    public string PaymentStatusAr { get; set; }

    // ✅ English + Arabic for ReturnStatus
    [Required, MaxLength(50)]
    public string ReturnStatus { get; set; }

    [Required, MaxLength(50)]
    public string ReturnStatusAr { get; set; }

    // ✅ English + Arabic for DisposalStatus
    [Required, MaxLength(50)]
    public string DisposalStatus { get; set; }
    [Required, MaxLength(50)]
    public string DisposalStatusAr { get; set; }

    // ✅ English + Arabic for WasteStatus
    [Required, MaxLength(50)]
    public string WasteStatus { get; set; }

    [Required, MaxLength(50)]
    public string WasteStatusAr { get; set; }

    public DateTime? CertificateAcceptedDate { get; set; }
    public DateTime? CertificateConfirmedDate { get; set; }
    public DateTime? GISApprovalDate { get; set; }

    [Required,  MaxLength(50)]
    public string BasketName {  get; set; }

    [Required, MaxLength(50)]
    public string BasketNameAr { get; set; }

    // ✅ Notes in Arabic + English
    [Required, MaxLength(500)]
    public string Notes { get; set; }

    [Required, MaxLength(500)]
    public string NotesAr { get; set; }

    [ForeignKey("WorkOrder")]
    public int WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }
}