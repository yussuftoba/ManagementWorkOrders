using ClosedXML.Excel;
using DTO;
using IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Models;

namespace ManagementWorkOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController : APIBaseController
    {
        public WorkOrdersController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Authorize]
        [HttpGet("GetAllWorkOrders")]
        public async Task<IActionResult> GetAllWorkOrders()
        {
            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var workOrders = await _unitOfWork.WorkOrders.GetAllAsync();

            var workOrdersDTO = new List<WorkOrderDTO>();

            foreach (var workOrder in workOrders)
            {
                var workOrderDTO = new WorkOrderDTO();

                workOrderDTO.Id = workOrder.Id;
                workOrderDTO.WorkOrderNumber = workOrder.WorkOrderNumber;
                workOrderDTO.Type = workOrder.Type;
                workOrderDTO.AssignedDate = workOrder.AssignmentDate;
                workOrderDTO.SubscriberName =culture.StartsWith("ar")? workOrder.SubscriberNameAr: workOrder.SubscriberName;
                workOrderDTO.Address = culture.StartsWith("ar")? workOrder.AddressAr: workOrder.Address;
                workOrderDTO.OrderValue = workOrder.Value;

                workOrdersDTO.Add(workOrderDTO);
            }

            return Ok(workOrdersDTO);
        }


        [Authorize]
        [HttpGet("GetWorkOrderById/{id:int}")]
        public async Task<IActionResult> GetWorkOrderById(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"{id} Id must be more than 0 ");
            }

            var workOrder = await _unitOfWork.WorkOrders.GetByIdAsync(id);

            if (workOrder == null)
            {
                return NotFound($"Id {id} doesn't exist in the system!");
            }

            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var workOrderDto = new WorkOrderDTO()
            {
                Id = workOrder.Id,
                WorkOrderNumber = workOrder.WorkOrderNumber,
                Type = workOrder.Type,
                AssignedDate = workOrder.AssignmentDate,
                SubscriberName = culture.StartsWith("ar") ? workOrder.SubscriberNameAr : workOrder.SubscriberName,
                Address = culture.StartsWith("ar") ? workOrder.AddressAr : workOrder.Address,
                OrderValue = workOrder.Value
            };

            return Ok(workOrderDto);

        }

        [Authorize(Roles ="admin")]
        [HttpPost("CreateWorkOrder")]
        public async Task<IActionResult> CreateWorkOrder(CreateWorkOrderDTO workOrderDTO)
        {
            if (ModelState.IsValid)
            {
                var tempWorkOrder1 = _unitOfWork.WorkOrders.FindOneItem(w => w.WorkOrderNumber == workOrderDTO.WorkOrderNumber && w.Type == workOrderDTO.Type);

                if(tempWorkOrder1 !=null)
                {
                    return BadRequest($"WorkOrderNumber and Type exist in the system, they must be unique!");
                }
                var workOrder = new WorkOrder()
                {
                    WorkOrderNumber = workOrderDTO.WorkOrderNumber,
                    Type = workOrderDTO.Type,
                    AssignmentDate = workOrderDTO.AssignedDate,
                    SubscriberName = workOrderDTO.SubscriberName,
                    SubscriberNameAr = workOrderDTO.SubscriberNameAr,
                    Address = workOrderDTO.Address,
                    AddressAr = workOrderDTO.AddressAr,
                    Value = workOrderDTO.OrderValue
                };

                await _unitOfWork.WorkOrders.AddAsync(workOrder);
                _unitOfWork.save();

                return Created();
            }
            return BadRequest(ModelState);
        }


        [Authorize(Roles = "admin")]
        [HttpPost("UploadWorkOrder")]
        public async Task<IActionResult> UploadWorkOrder(IFormFile file)
        {
            if(file ==null || file.Length == 0)
            {
                return BadRequest("Please upload a valid Excel file.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);

            var workOrderSheet = workbook.Worksheet(1);

            foreach (var row in workOrderSheet.RowsUsed().Skip(1))
            {
                var workOrder = new WorkOrder()
                {
                    WorkOrderNumber = row.Cell(1).GetString(),
                    Type = row.Cell(2).GetString(),
                    AssignmentDate = row.Cell(3).GetDateTime(),
                    SubscriberName = row.Cell(4).GetString(),
                    SubscriberNameAr = row.Cell(5).GetString(),
                    Address = row.Cell(6).GetString(),
                    AddressAr = row.Cell(7).GetString(),
                    Value = row.Cell(8).GetDouble()
                };

                if (_unitOfWork.WorkOrders.FindOneItem(w => w.WorkOrderNumber == workOrder.WorkOrderNumber && w.Type == workOrder.Type) !=null)
                {
                    return BadRequest("This WorkOrder is already exist with the same workOrder and Type");
                }

                await _unitOfWork.WorkOrders.AddAsync(workOrder);
            }
            _unitOfWork.save();

            return Created();
        }


        [Authorize(Roles = "admin")]
        [HttpPut("UpdateWorkOrder/{id:int}")]
        public async Task<IActionResult> UpdateWorkOrder(int id, CreateWorkOrderDTO workOrderDTO)
        {
            if (ModelState.IsValid)
            {
                if (id <= 0)
                {
                    return BadRequest($"{id} Id must be more than 0 ");
                }

                var workOrder = await _unitOfWork.WorkOrders.GetByIdAsync(id);

                if (workOrder == null)
                {
                    return NotFound($"Id {id} doesn't exist in the system!");
                }

                workOrder.WorkOrderNumber = workOrderDTO.WorkOrderNumber;
                workOrder.Type = workOrderDTO.Type;
                workOrder.AssignmentDate = workOrderDTO.AssignedDate;
                workOrder.SubscriberName = workOrderDTO.SubscriberName;
                workOrder.SubscriberNameAr = workOrderDTO.SubscriberNameAr;
                workOrder.Address = workOrderDTO.Address;
                workOrder.AddressAr = workOrderDTO.AddressAr;
                workOrder.Value = workOrderDTO.OrderValue;

                _unitOfWork.WorkOrders.Update(workOrder);
                _unitOfWork.save();

                return Ok("Updated Successfully");
            }
            return BadRequest(ModelState);
        }


        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteWorkOrder/{id:int}")]
        public async Task<IActionResult> DeleteWorkOrder(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"{id} Id must be more than 0 ");
            }

            var workOrder = await _unitOfWork.WorkOrders.GetByIdAsync(id);

            if(workOrder == null)
            {
                return NotFound($"Id {id} doesn't exist in the system!");
            }
            _unitOfWork.WorkOrders.Delete(workOrder);

            _unitOfWork.save();

            return Ok("Deleted Successfully");
        }

        [Authorize]
        [HttpGet("Search")]    //search by workOrderNumber, type and date
        public async Task<IActionResult> Search([FromQuery] string? workOrderNumber, [FromQuery] string? type, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var workOrders = await _unitOfWork.WorkOrders.GetAllAsync();

            if (workOrderNumber != null)
            {
                workOrders = workOrders.Where(w => w.WorkOrderNumber.ToLower().Contains(workOrderNumber.ToLower()));
            }

            if(type != null)
            {
                workOrders=workOrders.Where(w=>w.Type.ToLower().Contains(type.ToLower()));
            }

            if(fromDate.HasValue &&  toDate.HasValue)
            {
                workOrders = workOrders.Where(w => w.AssignmentDate >= fromDate && w.AssignmentDate <= toDate);
            }

            var culture = HttpContext.Request.Headers["Accept-Language"].ToString();

            var workOrdersDTO = new List<WorkOrderDTO>();

            foreach (var workOrder in workOrders)
            {
                var workOrderDTO = new WorkOrderDTO();

                workOrderDTO.Id = workOrder.Id;
                workOrderDTO.WorkOrderNumber = workOrder.WorkOrderNumber;
                workOrderDTO.Type = workOrder.Type;
                workOrderDTO.AssignedDate = workOrder.AssignmentDate;
                workOrderDTO.SubscriberName = culture.StartsWith("ar") ?workOrder.SubscriberNameAr: workOrder.SubscriberName;
                workOrderDTO.Address = culture.StartsWith("ar") ? workOrder.AddressAr : workOrder.Address;
                workOrderDTO.OrderValue = workOrder.Value;

                workOrdersDTO.Add(workOrderDTO);
            }

            return Ok(workOrdersDTO);
        }

        [Authorize]
        [HttpGet("GetWorkOrdersSummary")]
        public async Task<IActionResult> GetWorkOrdersSummary()
        {
            var totalOrder = await _unitOfWork.WorkOrders.Count(w => 1 == 1);
            var totalValue = await _unitOfWork.WorkOrders.Sum(w => w.Value);

            var response = new
            {
                TotaLOrder = totalOrder,
                TotalValue = totalValue
            };

            return Ok(response);
        }
    }
}
