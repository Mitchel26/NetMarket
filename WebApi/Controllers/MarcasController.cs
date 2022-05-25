using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
 
    public class MarcasController : BaseApiController
    {
        private readonly IGenericRepository<Marca> genericRepository;

        public MarcasController(IGenericRepository<Marca> genericRepository)
        {
            this.genericRepository = genericRepository;
        }


        [HttpGet()]
        public async Task<ActionResult<IReadOnlyList<Marca>>> GetMarcas()
        {
            var marcas = await genericRepository.GetAllAsync();
            return Ok(marcas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Marca>> GetMarca(int id)
        {
            var marca = await genericRepository.GetByIdAsync(id);
            return marca;
        }
    }
}
