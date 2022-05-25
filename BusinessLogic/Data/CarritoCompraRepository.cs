using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace BusinessLogic.Data
{
    public class CarritoCompraRepository : ICarritoCompraRepository
    {
        private readonly IDatabase database;
        public CarritoCompraRepository(IConnectionMultiplexer redis)
        {
            database = redis.GetDatabase();
        }

        public async Task<CarritoCompra> GetCarritoCompraAsync(string carritoId)
        {
            var data = await database.StringGetAsync(carritoId);
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CarritoCompra>(data);

        }

        public async Task<bool> DeleteCarritoCompraAsync(string carritoId)
        {
            return await database.KeyDeleteAsync(carritoId);
        }

        public async Task<CarritoCompra> UpdateCarritoCompraAsync(CarritoCompra carritoCompra)
        {
            var status = await database.StringSetAsync(carritoCompra.Id, JsonSerializer.Serialize(carritoCompra), TimeSpan.FromDays(30));
            if (!status)
            {
                return null;
            }
            return await GetCarritoCompraAsync(carritoCompra.Id);

        }
    }
}
