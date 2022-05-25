using System.Runtime.Serialization;

namespace Core.Entities.OrdenCompra
{
    public enum OrdenStatus
    {
        [EnumMember(Value = "Pendiente")]
        Pendiente,
        [EnumMember(Value = "El pago fue recibido")]
        PagoRecibido,
        [EnumMember(Value = "El pago tuvo errores")]
        PagoFallo
    }
}
