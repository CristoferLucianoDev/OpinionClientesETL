using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces;

namespace OpinionClienteDwh.Data.Validators;

public sealed class ClienteValidator : IValidator<ClienteDto>
{
    public bool EsValido(ClienteDto dto, out string? motivoRechazo)
    {
        if (dto.IdCliente <= 0)
        {
            motivoRechazo = "IdCliente invalido.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            motivoRechazo = "Nombre vacio.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
        {
            motivoRechazo = "Email invalido.";
            return false;
        }

        motivoRechazo = null;
        return true;
    }
}