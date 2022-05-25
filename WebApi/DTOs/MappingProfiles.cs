using AutoMapper;
using Core.Entities;
using Core.Entities.OrdenCompra;

namespace WebApi.DTOs
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Producto, ProductoDTO>()
                .ForMember(dto => dto.MarcaNombre, options => options.MapFrom(p => p.Marca.Nombre))
                .ForMember(dto => dto.CategoriaNombre, options => options.MapFrom(p => p.Categoria.Nombre));
            CreateMap<Core.Entities.Direccion, DireccionDTO>().ReverseMap();
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();

            CreateMap<DireccionDTO, Core.Entities.OrdenCompra.Direccion>();
            CreateMap<OrdenCompras, OrdenCompraResponseDTO>()
                .ForMember(o => o.TipoEnvio, x => x.MapFrom(y => y.TipoEnvio.Nombre))
                .ForMember(o => o.TipoEnvioPrecio, x => x.MapFrom(y => y.TipoEnvio.Precio));

            CreateMap<OrdenItem, OrdenItemResponseDTO>()
                .ForMember(oi => oi.ProductoId, x => x.MapFrom(y => y.ItemOrdenado.ProductoItemId))
                .ForMember(o => o.ProductoNombre, x => x.MapFrom(y => y.ItemOrdenado.ProductoNombre))
                .ForMember(o => o.ProductoImagen, x => x.MapFrom(y => y.ItemOrdenado.ImagenUrl));
        }
    }
}
