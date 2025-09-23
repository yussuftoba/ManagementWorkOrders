using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DTO;
using IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query;
using Models;

namespace ManagementWorkOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtractController : APIBaseController
    {
        public ExtractController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [Authorize]
        [HttpGet("GetAllExtracts")]
        public async Task<IActionResult> GetAllExtracts()
        {
            var extracts = await _unitOfWork.Extracts.FindAllAsync(e => 1 == 1, new string[] { "WorkOrder" });

            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var extractsDTO = extracts.Select(e => new ExtractDTO
            {
                Id = e.Id,
                ExtractNumber = e.ExtractNumber,
                Type =culture.StartsWith("ar")? e.TypeAr: e.Type,
                ExtractDate = e.ExtractDate,
                ExtractValue = e.ExtractValue,
                PenaltyValue = e.PenaltyValue,
                NetValue = e.NetValue,
                Tax = e.Tax,
                TotalWithTax = e.TotalWithTax,
                InvoiceNumber = e.InvoiceNumber,
                Department =culture.StartsWith("ar")? e.DepartmentAr: e.Department,
                WorkOrderNumber = e.WorkOrder.WorkOrderNumber,
                WorkOrderType = e.WorkOrder.Type
            });

            return Ok(extractsDTO);
        }

        [Authorize]
        [HttpGet("GetExtractById/{id:int}")]
        public IActionResult GetExtractById(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"{id} Id must be more than 0 ");
            }

            var extract = _unitOfWork.Extracts.FindOneItem(e => e.Id == id, new string[] { "WorkOrder" });

            if(extract == null)
            {
                return NotFound($"Id {id} doesn't exist in the system!");
            }

            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var extractDTO = new ExtractDTO()
            {
                Id = extract.Id,
                ExtractNumber = extract.ExtractNumber,
                Type = culture.StartsWith("ar")? extract.TypeAr:extract.Type,
                ExtractDate = extract.ExtractDate,
                ExtractValue = extract.ExtractValue,
                PenaltyValue = extract.PenaltyValue,
                NetValue = extract.NetValue,
                Tax = extract.Tax,
                TotalWithTax = extract.TotalWithTax,
                InvoiceNumber = extract.InvoiceNumber,
                Department =culture.StartsWith("ar")? extract.DepartmentAr: extract.Department,
                WorkOrderNumber = extract.WorkOrder.WorkOrderNumber,
                WorkOrderType = extract.WorkOrder.Type
            };
            return Ok(extractDTO);
        }


        [Authorize(Roles = "admin")]
        [HttpPost("CreateExtract")]
        public async Task<IActionResult> CreateExtract(CreateExtractDTO createDTO)
        {
            if (ModelState.IsValid)
            {
                var workOrder = _unitOfWork.WorkOrders.FindOneItem(w => w.WorkOrderNumber == createDTO.WorkOrderNumber);
                if (workOrder == null)
                {
                    return BadRequest("WorkOrderNumber doesn't exist in the system");
                }

                var extract = new Extract()
                {
                    ExtractNumber = createDTO.ExtractNumber,
                    Type = createDTO.Type,
                    TypeAr = createDTO.TypeAr,
                    ExtractDate = createDTO.ExtractDate,
                    ExtractValue = createDTO.ExtractValue,
                    PenaltyValue = createDTO.PenaltyValue,
                    InvoiceNumber = createDTO.InvoiceNumber,
                    Department = createDTO.Department,
                    DepartmentAr = createDTO.DepartmentAr,
                    WorkOrderId = workOrder.Id
                };

                try
                {
                    await _unitOfWork.Extracts.AddAsync(extract);
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
        [HttpPost("UploadExtract")]
        public async Task<IActionResult> UploadExtract(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a valid Excel file.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);

            var extractSheet = workbook.Worksheet(3);

            foreach (var row in extractSheet.RowsUsed().Skip(1))
            {
                var workOrder = _unitOfWork.WorkOrders.FindOneItem(w => w.WorkOrderNumber == row.Cell(10).GetString());

                if (workOrder == null)
                {
                    return BadRequest("WorkOrderNumber doesn't exist in the system");
                }

                try
                {
                    var extract = new Extract()
                    {
                        ExtractNumber = row.Cell(1).GetString(),
                        Type = row.Cell(2).GetString(),
                        TypeAr=row.Cell(3).GetString(),
                        ExtractDate = row.Cell(4).GetDateTime(),
                        ExtractValue = row.Cell(5).GetDouble(),
                        PenaltyValue = row.Cell(6).GetDouble(),
                        InvoiceNumber = row.Cell(7).GetString(),
                        Department = row.Cell(8).GetString(),
                        DepartmentAr=row.Cell(9).GetString(),
                        WorkOrderId = workOrder.Id
                    };

                    await _unitOfWork.Extracts.AddAsync(extract);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            _unitOfWork.save();
            return Created();
        }


        [Authorize(Roles = "admin")]
        [HttpPut("UpdateExtract/{id:int}")]
        public IActionResult UpdateExtract(int id, CreateExtractDTO createDTO)
        {
            if (ModelState.IsValid)
            {
                if (id <= 0)
                {
                    return BadRequest($"{id} Id must be more than 0 ");
                }

                var extract = _unitOfWork.Extracts.FindOneItem(e => e.Id == id, new string[] {"WorkOrder"});

                if (extract == null)
                {
                    return NotFound($"Id {id} doesn't exist in the system!");
                }

                var workOrder = _unitOfWork.WorkOrders.FindOneItem(w => w.WorkOrderNumber == createDTO.WorkOrderNumber);

                if (workOrder == null)
                {
                    return BadRequest("WorkOrderNumber doesn't exist in the system");
                }

                extract.ExtractNumber = createDTO.ExtractNumber;
                extract.Type = createDTO.Type;
                extract.TypeAr = createDTO.TypeAr;
                extract.ExtractDate = createDTO.ExtractDate;
                extract.ExtractValue = createDTO.ExtractValue;
                extract.PenaltyValue = createDTO.PenaltyValue;
                extract.InvoiceNumber = createDTO.InvoiceNumber;
                extract.Department = createDTO.Department;
                extract.DepartmentAr = createDTO.DepartmentAr;
                extract.WorkOrderId = workOrder.Id;

                _unitOfWork.Extracts.Update(extract);
                _unitOfWork.save();

                return Ok("Updated Successfully");
            }

            return BadRequest(ModelState);
        }


        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteExtract/{id:int}")]
        public IActionResult DeleteExtract(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"{id} Id must be more than 0 ");
            }

            var extract = _unitOfWork.Extracts.FindOneItem(e => e.Id == id);

            if (extract == null)
            {
                return NotFound($"Id {id} doesn't exist in the system!");
            }

            _unitOfWork.Extracts.Delete(extract);
            _unitOfWork.save();

            return Ok("Deleted Successfully");
        }

        [Authorize]
        [HttpGet("Summary")]
        public async Task<IActionResult> Summary()
        {
            var total = await _unitOfWork.Extracts.Count(e => 1 == 1);
            var totalNet = await _unitOfWork.Extracts.Sum(e => e.ExtractValue - e.PenaltyValue);
            var totalWithTax = await _unitOfWork.Extracts.Sum(e => (e.ExtractValue - e.PenaltyValue) + ((e.ExtractValue - e.PenaltyValue) * 0.15));

            return Ok(new
            {
                TotalExtracts = total,
                TotalNet = totalNet,
                totalWithTax = totalWithTax
            });
        }
    }
}
