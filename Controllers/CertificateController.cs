using ClosedXML.Excel;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DTO;
using IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Runtime.ConstrainedExecution;

namespace ManagementWorkOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : APIBaseController
    {
        public CertificateController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        [Authorize]
        [HttpGet("GetAllCertificates")]
        public async Task<IActionResult> GetAllCertificates()
        {
            var certificates = await _unitOfWork.Certificates.FindAllAsync(c => 1 == 1, new string[] { "WorkOrder"});

            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var certificatesDTO = new List<CertificateDTO>();

            foreach (var cert in certificates)
            {
                var certificateDTO = new CertificateDTO()
                {
                    Id = cert.Id,
                    WorkOrderNumber = cert.WorkOrder.WorkOrderNumber,
                    WorkOrderType = cert.WorkOrder.Type,
                    BasketName = culture.StartsWith("ar")? cert.BasketNameAr:cert.BasketName,
                    Form190 = cert.Form190,
                    Form200 = cert.Form200,
                    Procedure211 = cert.Procedure211,
                    DrillingForm = cert.DrillingForm,
                    ScrapingForm = cert.ScrapingForm,
                    ContractorReceivedDate = cert.ContractorReceivedDate,
                    WorkOrderDuration=cert.WorkOrderDuration,
                    AchievementCertificateAttached = cert.AchievementCertificateAttached,
                    CertificateValue = cert.CertificateValue,
                    PaymentStatus = culture.StartsWith("ar")? cert.PaymentStatusAr:cert.PaymentStatus,
                    ReturnStatus = culture.StartsWith("ar")? cert.ReturnStatusAr: cert.ReturnStatus,
                    DisposalStatus =culture.StartsWith("ar")? cert.DisposalStatusAr: cert.DisposalStatus,
                    WasteStatus = culture.StartsWith("ar")? cert.WasteStatusAr : cert.WasteStatus,
                    CertificateAcceptedDate = cert.CertificateAcceptedDate,
                    CertificateConfirmedDate = cert.CertificateConfirmedDate,
                    GISApprovalDate=cert.GISApprovalDate,
                    Notes = culture.StartsWith("ar")? cert.NotesAr: cert.Notes
                };

                certificatesDTO.Add(certificateDTO);
            }

            return Ok(certificatesDTO);
        }

        [Authorize]
        [HttpGet("GetCertificateById/{id:int}")]
        public IActionResult GetCertificateById(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"{id} Id must be more than 0 ");
            }

            var certificate = _unitOfWork.Certificates.FindOneItem(c => c.Id == id, new string[] { "WorkOrder" });

            if (certificate == null)
            {
                return NotFound($"Id {id} doesn't exist in the system!");
            }

            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var certificateDTO = new CertificateDTO()
            {
                Id = certificate.Id,
                WorkOrderNumber = certificate.WorkOrder.WorkOrderNumber,
                WorkOrderType = certificate.WorkOrder.Type,
                BasketName =culture.StartsWith("ar")? certificate.BasketNameAr: certificate.BasketName,
                Form190 = certificate.Form190,
                Form200 = certificate.Form200,
                Procedure211 = certificate.Procedure211,
                DrillingForm = certificate.DrillingForm,
                ScrapingForm = certificate.ScrapingForm,
                ContractorReceivedDate = certificate.ContractorReceivedDate,
                WorkOrderDuration = certificate.WorkOrderDuration,
                AchievementCertificateAttached = certificate.AchievementCertificateAttached,
                CertificateValue = certificate.CertificateValue,
                PaymentStatus = culture.StartsWith("ar") ? certificate.PaymentStatusAr : certificate.PaymentStatus,
                ReturnStatus = culture.StartsWith("ar") ? certificate.ReturnStatusAr : certificate.ReturnStatus,
                DisposalStatus = culture.StartsWith("ar") ? certificate.DisposalStatusAr : certificate.DisposalStatus,
                WasteStatus = culture.StartsWith("ar") ? certificate.WasteStatusAr : certificate.WasteStatus,
                CertificateAcceptedDate = certificate.CertificateAcceptedDate,
                CertificateConfirmedDate = certificate.CertificateConfirmedDate,
                GISApprovalDate=certificate.GISApprovalDate,
                Notes = culture.StartsWith("ar") ? certificate.NotesAr : certificate.Notes
            };

            return Ok(certificateDTO);
        }


        [Authorize(Roles ="admin")]
        [HttpPost("CreateCertificate")]
        public async Task<IActionResult> CreateCertificate(CreateCertificateDTO certificateDTO)
        {
            if (ModelState.IsValid)
            {
                var workOrder = _unitOfWork.WorkOrders.FindOneItem(w => w.WorkOrderNumber == certificateDTO.WorkOrderNumber);

                if (workOrder == null)
                {
                    return BadRequest("WorkOrderNumber doesn't exist in the system");
                }

                var certificate = new Certificate()
                {
                    Form190 = certificateDTO.Form190,
                    Form200 = certificateDTO.Form200,
                    Procedure211 = certificateDTO.Procedure211,
                    DrillingForm = certificateDTO.DrillingForm,
                    ScrapingForm = certificateDTO.ScrapingForm,
                    ContractorReceivedDate = certificateDTO.ContractorReceivedDate,
                    AchievementCertificateAttached = certificateDTO.AchievementCertificateAttached,
                    CertificateValue = certificateDTO.CertificateValue,
                    PaymentStatus = certificateDTO.PaymentStatus,
                    PaymentStatusAr = certificateDTO.PaymentStatusAr,
                    ReturnStatus = certificateDTO.ReturnStatus,
                    ReturnStatusAr = certificateDTO.ReturnStatusAr,
                    DisposalStatus = certificateDTO.DisposalStatus,
                    DisposalStatusAr = certificateDTO.DisposalStatusAr,
                    WasteStatus = certificateDTO.WasteStatus,
                    WasteStatusAr = certificateDTO.WasteStatusAr,
                    CertificateAcceptedDate = certificateDTO.CertificateAcceptedDate,
                    CertificateConfirmedDate = certificateDTO.CertificateConfirmedDate,
                    GISApprovalDate = certificateDTO.GISApprovalDate,
                    Notes = certificateDTO.Notes,
                    NotesAr = certificateDTO.NotesAr,
                    WorkOrderId = workOrder.Id,
                    BasketName = certificateDTO.BasketName,
                    BasketNameAr = certificateDTO.BasketNameAr
                };

                try
                {
                    await _unitOfWork.Certificates.AddAsync(certificate);
                    _unitOfWork.save();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return BadRequest(ModelState);
                }
                return Created();
            }
            return BadRequest(ModelState);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UploadCertificates")]
        public async Task<IActionResult> UploadCertificates(IFormFile file)
        {
            if(file==null || file.Length == 0)
            {
                return BadRequest("Please upload a valid Excel file.");
            }

            using var stream=new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);

            var certificateSheet = workbook.Worksheet(2);

            foreach(var row in certificateSheet.RowsUsed().Skip(1))
            {
                var workOrder = _unitOfWork.WorkOrders.FindOneItem(w => w.WorkOrderNumber == row.Cell(22).GetString());

                if (workOrder == null)
                {
                    return BadRequest("WorkOrderNumber doesn't exist in the system");
                }


                var certificate = new Certificate()
                {
                    Form190 = row.Cell(1).TryGetValue<bool>(out var f190) ? f190 : false,
                    Form200 = row.Cell(2).TryGetValue<bool>(out var f200) ? f200 : false,
                    Procedure211 = row.Cell(3).TryGetValue<bool>(out var procedure) ? procedure : false,
                    DrillingForm = row.Cell(4).TryGetValue<bool>(out var drilling) ? drilling : false,
                    ScrapingForm = row.Cell(5).TryGetValue<bool>(out var scraping) ? scraping : false,
                    ContractorReceivedDate = row.Cell(6).GetDateTime(),
                    AchievementCertificateAttached = row.Cell(7).TryGetValue<bool>(out var attached) ? attached : false,
                    CertificateValue = row.Cell(8).TryGetValue<double>(out var val) ? val : 0,
                    PaymentStatus = row.Cell(9).GetString(),
                    PaymentStatusAr = row.Cell(10).GetString(),
                    ReturnStatus = row.Cell(11).GetString(),
                    ReturnStatusAr = row.Cell(12).GetString(),
                    DisposalStatus = row.Cell(13).GetString(),
                    DisposalStatusAr = row.Cell(14).GetString(),
                    WasteStatus = row.Cell(15).GetString(),
                    WasteStatusAr = row.Cell(16).GetString(),
                    CertificateAcceptedDate = row.Cell(17).TryGetValue<DateTime>(out var acceptance) ? acceptance : null,
                    CertificateConfirmedDate = row.Cell(18).TryGetValue<DateTime>(out var confirmation) ? confirmation : null,
                    GISApprovalDate = row.Cell(19).TryGetValue<DateTime>(out var approval) ? approval : null,
                    Notes = row.Cell(20).GetString(),
                    NotesAr = row.Cell(21).GetString(),
                    WorkOrderId = workOrder.Id,
                    BasketName = row.Cell(23).GetString(),
                    BasketNameAr = row.Cell(24).GetString()
                };

                await _unitOfWork.Certificates.AddAsync(certificate);
            }

            try
            {
                _unitOfWork.save();
                return Created();
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("UpdateCertificate/{id:int}")]
        public IActionResult UpdateCertificate(int id, CreateCertificateDTO certificateDTO)
        {
            if (ModelState.IsValid)
            {
                if (id <= 0)
                {
                    return BadRequest($"{id} Id must be more than 0 ");
                }

                var certificate = _unitOfWork.Certificates.FindOneItem(c => c.Id == id, new string[] { "WorkOrder" });

                if (certificate == null)
                {
                    return NotFound($"Id {id} doesn't exist in the system!");
                }

                var workOrder=_unitOfWork.WorkOrders.FindOneItem(w=>w.WorkOrderNumber==certificateDTO.WorkOrderNumber);

                if(workOrder == null)
                {
                    return BadRequest("WorkOrderNumber doesn't exist in the system");
                }

                certificate.Form190 = certificateDTO.Form190;
                certificate.Form200 = certificateDTO.Form200;
                certificate.Procedure211 = certificateDTO.Procedure211;
                certificate.DrillingForm = certificateDTO.DrillingForm;
                certificate.ScrapingForm = certificateDTO.ScrapingForm;
                certificate.ContractorReceivedDate = certificateDTO.ContractorReceivedDate;
                certificate.AchievementCertificateAttached = certificateDTO.AchievementCertificateAttached;
                certificate.CertificateValue = certificateDTO.CertificateValue;
                certificate.PaymentStatus = certificateDTO.PaymentStatus;
                certificate.PaymentStatusAr = certificateDTO.PaymentStatusAr;
                certificate.ReturnStatus = certificateDTO.ReturnStatus;
                certificate.ReturnStatusAr = certificateDTO.ReturnStatusAr;
                certificate.DisposalStatus = certificateDTO.DisposalStatus;
                certificate.DisposalStatusAr = certificateDTO.DisposalStatusAr;
                certificate.WasteStatus = certificateDTO.WasteStatus;
                certificate.WasteStatusAr = certificateDTO.WasteStatusAr;
                certificate.CertificateAcceptedDate = certificateDTO.CertificateAcceptedDate;
                certificate.CertificateConfirmedDate = certificateDTO.CertificateConfirmedDate;
                certificate.GISApprovalDate = certificateDTO.GISApprovalDate;
                certificate.Notes = certificateDTO.Notes;
                certificate.NotesAr = certificateDTO.NotesAr;
                certificate.WorkOrderId = workOrder.Id;
                certificate.BasketName = certificateDTO.BasketName;
                certificate.BasketNameAr= certificateDTO.BasketNameAr;

                try
                {
                    _unitOfWork.Certificates.Update(certificate);
                    _unitOfWork.save();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return BadRequest(ModelState);
                }
                return Ok("Updated Successfully");
            }
            return BadRequest(ModelState);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteCertificate/{id:int}")]
        public IActionResult DeleteCertificate(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"{id} Id must be more than 0 ");
            }

            var certificate = _unitOfWork.Certificates.FindOneItem(c => c.Id == id, new string[] { "WorkOrder"});

            if (certificate == null)
            {
                return NotFound($"Id {id} doesn't exist in the system!");
            }

            _unitOfWork.Certificates.Delete(certificate);
            _unitOfWork.save();

            return Ok("Deleted Successfully");
        }

        [Authorize]
        [HttpGet("SearchByBasketNameOrWorkOrderNumber")] //Search by basket name(Arabic, English) or workordernumber (English only)
        public async Task<IActionResult> SearchByBasketNameOrWorkOrderNumber([FromQuery]string? searchTerm)
        {
            var certificates = await _unitOfWork.Certificates.FindAllAsync(c => 1 == 1, new string[] { "WorkOrder" });

            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            if (searchTerm != null)
            {
                certificates = certificates.Where(c => c.WorkOrder.WorkOrderNumber.ToLower().Contains(searchTerm.ToLower()) 
                                                        || (culture.StartsWith("ar") ? c.BasketNameAr.Contains(searchTerm) 
                                                            : c.BasketName.ToLower().Contains(searchTerm.ToLower())));
            }

            var certificatesDTO = certificates.Select(c => new CertificateDTO
            {
                Id = c.Id,
                WorkOrderNumber = c.WorkOrder.WorkOrderNumber,
                WorkOrderType = c.WorkOrder.Type,
                BasketName = culture.StartsWith("ar") ? c.BasketNameAr : c.BasketName,
                Form190 = c.Form190,
                Form200 = c.Form200,
                Procedure211 = c.Procedure211,
                DrillingForm = c.DrillingForm,
                ScrapingForm = c.ScrapingForm,
                ContractorReceivedDate = c.ContractorReceivedDate,
                WorkOrderDuration = c.WorkOrderDuration,
                AchievementCertificateAttached = c.AchievementCertificateAttached,
                CertificateValue = c.CertificateValue,
                PaymentStatus = culture.StartsWith("ar") ? c.PaymentStatusAr : c.PaymentStatus,
                ReturnStatus = culture.StartsWith("ar") ? c.ReturnStatusAr : c.ReturnStatus,
                DisposalStatus = culture.StartsWith("ar") ? c.DisposalStatusAr : c.DisposalStatus,
                WasteStatus = culture.StartsWith("ar") ? c.WasteStatusAr : c.WasteStatus,
                CertificateAcceptedDate = c.CertificateAcceptedDate,
                CertificateConfirmedDate = c.CertificateConfirmedDate,
                GISApprovalDate = c.GISApprovalDate,
                Notes = culture.StartsWith("ar") ? c.NotesAr : c.Notes
            });

            return Ok(certificatesDTO);
        }

        [Authorize]
        [HttpGet("Summary")]
        public async Task<IActionResult> Summary()
        {
            var total = await _unitOfWork.Certificates.Count(c => 1 == 1);
            var totalValue = await _unitOfWork.Certificates.Sum(c => c.CertificateValue ?? 0);

            return Ok(new
            {
                TotalCertificates = total,
                TotalValue = totalValue
            });
        }
    }
}

