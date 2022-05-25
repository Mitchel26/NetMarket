using Core.Entities;
using Core.Entities.OrdenCompra;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BusinessLogic.Data
{
    public class MarketDbContextData
    {
        public static async Task CargarDataAsync(MarketDbContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                if (!context.Marcas.Any())
                {
                    var marcaData = File.ReadAllText("../BusinessLogic/CargarData/marca.json");
                    var marcas = JsonSerializer.Deserialize<List<Marca>>(marcaData);
                    foreach (var marca in marcas)
                    {
                        context.Marcas.Add(marca);
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.Categorias.Any())
                {
                    var categoriaData = File.ReadAllText("../BusinessLogic/CargarData/categoria.json");
                    var categorias = JsonSerializer.Deserialize<List<Categoria>>(categoriaData);
                    foreach (var categoria in categorias)
                    {
                        context.Categorias.Add(categoria);
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.Productos.Any())
                {
                    var productoData = File.ReadAllText("../BusinessLogic/CargarData/producto.json");
                    var productos = JsonSerializer.Deserialize<List<Producto>>(productoData);
                    foreach (var producto in productos)
                    {
                        context.Productos.Add(producto);
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.TipoEnvios.Any())
                {
                    var tipoEnviosData = File.ReadAllText("../BusinessLogic/CargarData/tipoenvio.json");
                    var tipoEnvios = JsonSerializer.Deserialize<List<TipoEnvio>>(tipoEnviosData);
                    foreach (var tipoEnvio in tipoEnvios)
                    {
                        context.TipoEnvios.Add(tipoEnvio);
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<MarketDbContextData>();
                logger.LogError(e.Message);

            }
        }
    }
}
