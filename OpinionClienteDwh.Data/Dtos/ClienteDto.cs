
namespace OpinionClienteDwh.Data.Dtos
{
    public sealed record ClienteDto
    {
        public required int IdCliente { get; init; }
        public required string Nombre { get; init; }
        public required string Email { get; init; }
    }
}
