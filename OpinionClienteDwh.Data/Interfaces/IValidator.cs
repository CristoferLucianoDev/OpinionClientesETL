namespace OpinionClienteDwh.Data.Interfaces;

public interface IValidator<T>
{
    bool EsValido(T dto, out string? motivoRechazo);
}