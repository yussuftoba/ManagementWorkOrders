using DatabaseContext;
using IServices;
using Models;

namespace Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        WorkOrders = new Repository<WorkOrder>(context);
        Certificates = new Repository<Certificate>(context);
        Approvals = new Repository<Approval>(context);
        Extracts=new Repository<Extract>(context);
    }
    public IRepository<WorkOrder> WorkOrders { get; }

    public IRepository<Certificate> Certificates { get; }

    public IRepository<Approval> Approvals { get; }

    public IRepository<Extract> Extracts { get; }

    public void Dispose()
    {
        _context.Dispose();
    }

    public int save()
    {
        return _context.SaveChanges();
    }
}
