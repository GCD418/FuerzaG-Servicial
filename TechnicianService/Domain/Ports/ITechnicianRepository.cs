using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicianService.Domain.Entities;

namespace TechnicianService.Domain.Ports
{
    public interface ITechnicianRepository
    {
        Task<IEnumerable<Technician>> GetAllAsync();
        Task<Technician?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Technician technician, int userId);
        Task<bool> UpdateAsync(Technician technician, int userId);
        Task<bool> DeleteByIdAsync(int id, int userId);
    }
}
