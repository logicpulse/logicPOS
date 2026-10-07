namespace LogicPOS.Domain.Entities.Common;

public interface IRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<string> GetNextCodeAsync(CancellationToken ct = default);
    Task<uint> GetNextOrderAsync(CancellationToken ct = default);
    Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default);
    
    public interface IWithDesignation : IRepository
    {
        Task<bool> DesignationExistsAsync(string designation, CancellationToken cancellationToken = default);
    }

    public interface IWithName : IRepository
    {
        Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    }
}