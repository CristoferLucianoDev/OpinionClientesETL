using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces;

namespace OpinionClienteDwh.Data.Validators;

public sealed class ProductoValidator : IValidator<ProductoDto>
{
    public bool EsValido(ProductoDto dto, out string? motivoRechazo)
    {
        if (dto.IdProducto <= 0)
        {
            motivoRechazo = "IdProducto invalido.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            motivoRechazo = "Nombre vacio.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(dto.Categoria))
        {
            motivoRechazo = "Categoria vacia.";
            return false;
        }

        motivoRechazo = null;
        return true;
    }
}