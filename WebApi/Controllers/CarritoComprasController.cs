using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class CarritoComprasController : BaseApiController
    {
        private readonly ICarritoCompraRepository carritoCompraRepository;

        public CarritoComprasController(ICarritoCompraRepository carritoCompraRepository)
        {
            this.carritoCompraRepository = carritoCompraRepository;
        }

        [HttpGet]
        public async Task<ActionResult<CarritoCompra>> GetCarritoById(string id)
        {
            var carrito = await carritoCompraRepository.GetCarritoCompraAsync(id);
            return Ok(carrito ?? new CarritoCompra(id));
        }

        [HttpPost]
        public async Task<ActionResult<CarritoCompra>> UpdateCarritoCompra(CarritoCompra carritoCompra)
        {
            var carritoActualizado = await carritoCompraRepository.UpdateCarritoCompraAsync(carritoCompra);
            return carritoActualizado;
        }

        [HttpDelete]
        public async Task DeleteCarritoCompra(string id)
        {
            await carritoCompraRepository.DeleteCarritoCompraAsync(id);
        }
    }
}
