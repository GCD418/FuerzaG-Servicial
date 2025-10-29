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

        public async Task<IEnumerable<Technician>> GetAll() => await _repository.GetAllAsync();
        public async Task<Technician?> GetById(int id) => await _repository.GetByIdAsync(id);
        public async Task<bool> Create(Technician t) => await _repository.CreateAsync(t);
        public async Task<bool> Update(Technician t, int userId) => await _repository.UpdateAsync(t, userId);
        public async Task<bool> DeleteById(int id, int userId) => await _repository.DeleteByIdAsync(id, userId);
    }
}