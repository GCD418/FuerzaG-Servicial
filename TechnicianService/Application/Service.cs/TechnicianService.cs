using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicianService.Domain.Entities;
using TechnicianService.Domain.Ports;

namespace TechnicianService.Application.Services
{
    public class TechnicianService
    {
        private readonly ITechnicianRepository _repository;

        public TechnicianService(ITechnicianRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Technician>> GetAll()        => _repository.GetAllAsync();
        public Task<Technician?> GetById(int id)             => _repository.GetByIdAsync(id);
        public Task<bool> Create(Technician t)               => _repository.CreateAsync(t);
        public Task<bool> Update(Technician t)               => _repository.UpdateAsync(t);
        public Task<bool> DeleteById(int id)                 => _repository.DeleteByIdAsync(id);
    }
}
