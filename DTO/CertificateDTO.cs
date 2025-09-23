using System.ComponentModel.DataAnnotations;

namespace DTO;

public class CertificateDTO
{
    public int Id {  get; set; }
    
    public string WorkOrderNumber { get; set; }

    public string WorkOrderType {  get; set; }

    public string BasketName { get; set; }
    public bool Form190 { get; set; }
    public bool Form200 { get; set; }
    public bool Procedure211 { get; set; }
    public bool DrillingForm { get; set; }
    public bool ScrapingForm { get; set; }
    public DateTime? ContractorReceivedDate { get; set; }

    public int? WorkOrderDuration {  get; set; }

    public bool AchievementCertificateAttached { get; set; }

    public double? CertificateValue { get; set; }


    // Localized fields
    public string PaymentStatus { get; set; }

    public string ReturnStatus { get; set; }

    public string DisposalStatus { get; set; }

    public string WasteStatus { get; set; }

    public DateTime? CertificateAcceptedDate { get; set; }
    public DateTime? CertificateConfirmedDate { get; set; }
    public DateTime? GISApprovalDate { get; set; }

    public string Notes { get; set; }
}
