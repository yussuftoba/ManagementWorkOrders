using Models;

namespace IServices;

public interface IUnitOfWork:IDisposable
{
    public IRepository<WorkOrder> WorkOrders { get; }
    public IRepository<Certificate> Certificates { get; }
    public IRepository<Approval> Approvals { get; }
    public IRepository<Extract> Extracts { get; }

    int save();

}
