using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces;

namespace OpinionClienteDwh.Data.Validators;

public sealed class SurveyValidator : IValidator<SurveyDto>
{
    public bool EsValido(SurveyDto dto, out string? motivoRechazo)
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

        if (string.IsNullOrWhiteSpace(dto.Clasificacion))
        {
            motivoRechazo = "Clasificacion vacia.";
            return false;
        }

        if (dto.PuntajeSatisfaccion < 0 || dto.PuntajeSatisfaccion > 5)
        {
            motivoRechazo = $"PuntajeSatisfaccion fuera de rango: {dto.PuntajeSatisfaccion}.";
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