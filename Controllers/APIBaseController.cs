using IServices;
using Microsoft.AspNetCore.Mvc;

namespace ManagementWorkOrdersAPI.Controllers
{
    public class APIBaseController: ControllerBase
    {
        protected readonly IUnitOfWork _unitOfWork;

        public APIBaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
