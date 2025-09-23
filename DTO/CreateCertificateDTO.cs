using System.ComponentModel.DataAnnotations;

namespace DTO;

public class CreateCertificateDTO
{
    [Required]
    public string WorkOrderNumber { get; set; }

    [Required]
    public string BasketName { get; set; }

    [Required]
    public string BasketNameAr {  get; set; }
    public bool Form190 { get; set; }
    public bool Form200 { get; set; }
    public bool Procedure211 { get; set; }
    public bool DrillingForm { get; set; }
    public bool ScrapingForm { get; set; }
    public DateTime? ContractorReceivedDate { get; set; }
    public bool AchievementCertificateAttached { get; set; }

    [Required, Range(0, 9999999999999999)]
    public double? CertificateValue { get; set; }

    // ✅ Localized fields

    [Required]
    public string PaymentStatus { get; set; }

    [Required]
    public string PaymentStatusAr { get; set; }


    [Required, MaxLength(50)]
    public string ReturnStatus { get; set; }

    [Required]
    public string ReturnStatusAr { get; set; }


    [Required, MaxLength(50)]
    public string DisposalStatus { get; set; }

    [Required]
    public string DisposalStatusAr { get; set; }


    [Required, MaxLength(50)]
    public string WasteStatus { get; set; }

    [Required]
    public string WasteStatusAr { get; set; }

    
    public DateTime? CertificateAcceptedDate { get; set; }
    public DateTime? CertificateConfirmedDate { get; set; }
    public DateTime? GISApprovalDate { get; set; }

    [Required]
    public string Notes { get; set; }

    [Required]
    public string NotesAr { get; set; }
}
