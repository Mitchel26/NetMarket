﻿namespace Core.Entities.OrdenCompra
{
    public class OrdenItem : ClaseBase
    {
        public OrdenItem()
        {

        }
        public OrdenItem(ProductoItemOrdenado itemOrdenado, decimal precio, int cantidad)
        {
            ItemOrdenado = itemOrdenado;
            Precio = precio;
            Cantidad = cantidad;
        }

        public ProductoItemOrdenado ItemOrdenado { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }

    }
}
