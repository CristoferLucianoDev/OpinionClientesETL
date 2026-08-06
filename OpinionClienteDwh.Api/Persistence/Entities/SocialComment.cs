namespace OpinionClienteDwh.Api.Persistence.Entities;

public sealed class SocialComment
{
    public int IdComment { get; set; }
    public string IdOriginal { get; set; } = string.Empty;
    public int IdCliente { get; set; }
    public int IdProducto { get; set; }
    public int IdRedSocial { get; set; }
    public int IdFuenteDatos { get; set; }
    public DateTime Fecha { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
}