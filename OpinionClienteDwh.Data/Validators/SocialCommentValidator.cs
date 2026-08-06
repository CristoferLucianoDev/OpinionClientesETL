using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces;

namespace OpinionClienteDwh.Data.Validators;

public sealed class SocialCommentValidator : IValidator<SocialCommentDto>
{
    public bool EsValido(SocialCommentDto dto, out string? motivoRechazo)
    {
        if (string.IsNullOrWhiteSpace(dto.IdOriginal))
        {
            motivoRechazo = "IdOriginal vacio.";
            return false;
        }

        if (dto.IdCliente <= 0)
        {
            motivoRechazo = "IdCliente invalido.";
            return false;
        }

        if (dto.IdProducto <= 0)
        {
            motivoRechazo = "IdProducto invalido.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(dto.Comentario))
        {
            motivoRechazo = "Comentario vacio.";
            return false;
        }

        if (dto.Fecha > DateTime.Now)
        {
            motivoRechazo = "Fecha en el futuro.";
            return false;
        }

        motivoRechazo = null;
        return true;
    }
}