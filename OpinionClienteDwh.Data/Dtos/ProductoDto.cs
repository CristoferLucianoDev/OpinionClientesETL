namespace OpinionClienteDwh.Data.Dtos
{
    public sealed record ProductoDto
    {
        public required int IdProducto { get; init; }
        public required string Nombre { get; init; }
        public required string Categoria { get; init; }
    }
}
