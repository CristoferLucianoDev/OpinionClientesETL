using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Models;

namespace OpinionClienteDwh.Data.Load.Services;

public sealed class ServiceMergeDimensiones(
    IStagingReader stagingReader,
    IDaoMergeDimensiones daoMergeDimensiones) : IServiceMergeDimensiones
{
    private readonly IStagingReader _stagingReader = stagingReader;
    private readonly IDaoMergeDimensiones _daoMergeDimensiones = daoMergeDimensiones;

    public async Task<DimensionesValidas> EjecutarAsync(CancellationToken cancellationToken)
    {
        var clientes = await _stagingReader.ReadLastAsync<ClienteDto>("clientes", cancellationToken);
        await _daoMergeDimensiones.MergeClientesAsync(clientes);

        var productos = await _stagingReader.ReadLastAsync<ProductoDto>("productos", cancellationToken);
        await _daoMergeDimensiones.MergeProductosAsync(productos);

        return new DimensionesValidas
        {
            IdsClientesValidos = clientes.Select(c => c.IdCliente).ToHashSet(),
            IdsProductosValidos = productos.Select(p => p.IdProducto).ToHashSet()
        };
    }
}