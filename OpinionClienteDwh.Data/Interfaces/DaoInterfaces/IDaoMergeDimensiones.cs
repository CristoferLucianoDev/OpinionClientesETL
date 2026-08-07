using OpinionClienteDwh.Data.Dtos;

namespace OpinionClienteDwh.Data.Interfaces.DaoInterfaces;

public interface IDaoMergeDimensiones
{
    Task MergeClientesAsync(IReadOnlyCollection<ClienteDto> clientes);
    Task MergeProductosAsync(IReadOnlyCollection<ProductoDto> productos);
}