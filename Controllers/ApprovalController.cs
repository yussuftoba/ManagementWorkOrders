using ClosedXML.Excel;
using DTO;
using IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ManagementWorkOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalController : APIBaseController
    {
        public ApprovalController(IUnitOfWork unitOfWork):base(unitOfWork)
        {
        }

        [Authorize]
        [HttpGet("GetAllApproval")]
        public async Task<IActionResult> GetAllApproval()
        {
            var approvals = await _unitOfWork.Approvals.FindAllAsync(a => 1 == 1, new string[] { "WorkOrder" });

            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var approvalsDTO = approvals.Select(a => new ApprovalDTO()
            {
                Id = a.Id,
                WorkOrderNumber = a.WorkOrder.WorkOrderNumber,
                WorkOrderType = a.WorkOrder.Type,
                SupportHeadApproval = a.SupportHeadApproval,
                ExecutorHeadApproval = a.ExecutorHeadApproval,
                CircleDirectorApproval = a.CircleDirectorApproval,
                DepartmentDirectorApproval = a.DepartmentDirectorApproval,
                FinanceApproval = a.FinanceApproval,
                DeputyApproval = a.DeputyApproval,
                CurrentDepartment = culture.StartsWith("ar") ? a.CurrentDepartmentAr : a.CurrentDepartment,
                Notes = culture.StartsWith("ar") ? a.NotesAr : a.Notes
            });

            return Ok(approvalsDTO);    
        }


        [Authorize(Roles ="admin")]
        [HttpPost("UploadApproval")]
        public async Task<IActionResult> UploadApproval(IFormFile file)
        {
            if(file==null || file.Length == 0)
            {
                return BadRequest("Please upload a valid Excel file.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);

            var approvalSheet = workbook.Worksheet(4);

            foreach(var row in approvalSheet.RowsUsed().Skip(1))
            {
                var workOrder = _unitOfWork.WorkOrders.FindOneItem(w => w.WorkOrderNumber == row.Cell(1).GetString());

                if (workOrder == null)
                {
                    return BadRequest("This workorderNumber doesn't exist in the system");
                }

                var approval = new Approval()
                {
                    WorkOrderId = workOrder.Id,
                    SupportHeadApproval = row.Cell(2).TryGetValue<DateTime>(out var support) ? support : null,
                    ExecutorHeadApproval = row.Cell(3).TryGetValue<DateTime>(out var execute) ? execute : null,
                    CircleDirectorApproval = row.Cell(4).TryGetValue<DateTime>(out var circle) ? circle : null,
                    DepartmentDirectorApproval = row.Cell(5).TryGetValue<DateTime>(out var deptDirect) ? deptDirect : null,
                    FinanceApproval = row.Cell(6).TryGetValue<DateTime>(out var finance) ? finance : null,
                    DeputyApproval = row.Cell(7).TryGetValue<DateTime>(out var deputy) ? deputy : null,
                    CurrentDepartment = row.Cell(8).GetString(),
                    CurrentDepartmentAr = row.Cell(9).GetString(),
                    Notes = row.Cell(10).GetString(),
                    NotesAr = row.Cell(11).GetString()
                };

                await _unitOfWork.Approvals.AddAsync(approval);
            }

            try
            {
                _unitOfWork.save();
                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
