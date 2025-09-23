using IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;

namespace ManagementWorkOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : APIBaseController
    {
        public ReportController(IUnitOfWork unitOfWork):base(unitOfWork)
        {
        }

        //WorkOrders Report

        [Authorize]
        [HttpGet("WorkOrderReportByDate")]
        public async Task<IActionResult> WorkOrderReportByDate([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var workOrders = await _unitOfWork.WorkOrders.GetAllAsync();

            if (fromDate.HasValue && toDate.HasValue)
            {
                workOrders=workOrders.Where(w=>w.AssignmentDate>=fromDate && w.AssignmentDate<=toDate);
            }

            var totalOrders=workOrders.Count();
            var totalValue=workOrders.Sum(w=>w.Value);

            return Ok(new
            {
                TotalOrders = totalOrders,
                TotalValue = totalValue
            });
        }


        [Authorize]
        [HttpGet("GetWorkOrdersByType")]
        public async Task<IActionResult> GetWorkOrdersByType()
        {
            var workOrders = await _unitOfWork.WorkOrders.GetAllAsync();

            var GetWorkOrdersByType = workOrders.GroupBy(w => w.Type).Select(g => new
            {
                Type = g.Key,
                Count = g.Count(),
                TotalValue = g.Sum(s => s.Value)
            });

            return Ok(GetWorkOrdersByType);
        }

        [Authorize]
        [HttpGet("GetWorkOrdersAllMonthsPerYear")]
        public async Task<IActionResult> GetWorkOrdersAllMonthsPerYear([FromQuery] int year)
        {
            if (year < 0)
            {
                return BadRequest("Year must be more than 0");
            }
            var workOrders = await _unitOfWork.WorkOrders.GetAllAsync();

            var workOrdersByMonth = workOrders.Where(w => w.AssignmentDate.Year == year).GroupBy(w => w.AssignmentDate.Month).
                                               Select(g => new
                                               {
                                                   Month = g.Key,
                                                   Count = g.Count(),
                                                   TotalValue = g.Sum(s => s.Value)
                                               });

            return Ok(workOrdersByMonth);
        }

        [Authorize]
        [HttpGet("GetWorkOrdersByDepartment")]
        public async Task<IActionResult> GetWorkOrdersByDepartment([FromQuery]string department)
        {
            var extracts = await _unitOfWork.Extracts.FindAllAsync(x =>x.Department.ToLower().Contains(department.ToLower()), new string[] { "WorkOrder" });

            if(extracts == null)
            {
                return NotFound();
            }

            var totalCount = extracts.Count();

            double totalValue = 0;

            foreach(var extract in extracts)
            {
                totalValue += extract.WorkOrder.Value;
            }

            return Ok(new
            {
                TotalCount = totalCount,
                TotalValue = totalValue
            });
        }
        //Certificates Report
        [Authorize]
        [HttpGet("CertificateReportByPaymentStatus")]
        public async Task<IActionResult> CertificateReportByPaymentStatus()
        {
            var certificates = await _unitOfWork.Certificates.FindAllAsync(c => 1 == 1, new string[] {"WorkOrder"});
            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var finalResult = certificates.GroupBy(c => culture.StartsWith("ar")? c.PaymentStatusAr:c.PaymentStatus).Select(g => new
            {
                Status = g.Key,
                count = g.Count(),
                totalValue = g.Sum(c => c.CertificateValue)
            });

            return Ok(finalResult);
        }

        [Authorize]
        [HttpGet("CertificateReportByReturnStatus")]
        public async Task<IActionResult> CertificateReportByReturnStatus()
        {
            var certificates = await _unitOfWork.Certificates.FindAllAsync(c => 1 == 1, new string[] { "WorkOrder" });
            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var finalResult = certificates.GroupBy(c => culture.StartsWith("ar") ? c.ReturnStatusAr : c.ReturnStatus).Select(g => new
            {
                Status = g.Key,
                count = g.Count(),
                totalValue = g.Sum(c => c.CertificateValue)
            });

            return Ok(finalResult);
        }

        [Authorize]
        [HttpGet("CertificateReportByDisposalStatus")]
        public async Task<IActionResult> CertificateReportByDisposalStatus()
        {
            var certificates = await _unitOfWork.Certificates.FindAllAsync(c => 1 == 1, new string[] { "WorkOrder" });
            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var finalResult = certificates.GroupBy(c => culture.StartsWith("ar") ? c.DisposalStatusAr : c.DisposalStatus).Select(g => new
            {
                Status = g.Key,
                count = g.Count(),
                totalValue = g.Sum(c => c.CertificateValue)
            });

            return Ok(finalResult);
        }

        [Authorize]
        [HttpGet("CertificateReportByWasteStatus")]
        public async Task<IActionResult> CertificateReportByWasteStatus()
        {
            var certificates = await _unitOfWork.Certificates.FindAllAsync(c => 1 == 1, new string[] { "WorkOrder" });
            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var finalResult = certificates.GroupBy(c => culture.StartsWith("ar") ? c.WasteStatusAr : c.WasteStatus).Select(g => new
            {
                Status = g.Key,
                count = g.Count(),
                totalValue = g.Sum(c => c.CertificateValue)
            });

            return Ok(finalResult);
        }

        //Extracts Report
        [Authorize]
        [HttpGet("ExtractsReportByType")]
        public async Task<IActionResult> ExtractsReportByType()
        {
            var extracts = await _unitOfWork.Extracts.FindAllAsync(e => 1 == 1, new string[] { "WorkOrder" });

            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var finalResult = extracts.GroupBy(e => culture.StartsWith("ar") ? e.TypeAr : e.Type).Select(g => new
            {
                Type = g.Key,
                Count = g.Count(),
                TotalNet = g.Sum(x => x.NetValue),
                TotalWithTax = g.Sum(x => x.TotalWithTax)
            });

            return Ok(finalResult);
        }

        [Authorize]
        [HttpGet("ExtractsReportByDepartment")]
        public async Task<IActionResult> ExtractsReportByDepartment()
        {
            var extracts = await _unitOfWork.Extracts.FindAllAsync(e => 1 == 1, new string[] { "WorkOrder" });

            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var finalResult = extracts.GroupBy(e => culture.StartsWith("ar") ? e.DepartmentAr : e.Department).Select(g => new
            {
                Type = g.Key,
                Count = g.Count(),
                TotalNet = g.Sum(x => x.NetValue),
                TotalWithTax = g.Sum(x => x.TotalWithTax)
            });

            return Ok(finalResult);
        }

        //Approvals Report
        [Authorize]
        [HttpGet("ApprovalReport")]
        public async Task<IActionResult> ApprovalReport()
        {
            var total = await _unitOfWork.Approvals.Count(a=>1==1);
            var approvedSupport = await _unitOfWork.Approvals.Count(a => a.SupportHeadApproval != null);
            var approvedFinance = await _unitOfWork.Approvals.Count(a => a.FinanceApproval != null);

            return Ok(new
            {
                TotalApprovals = total,
                ApprovedSupport = approvedSupport,
                ApprovedFinance = approvedFinance
            });
        }
    }
}
