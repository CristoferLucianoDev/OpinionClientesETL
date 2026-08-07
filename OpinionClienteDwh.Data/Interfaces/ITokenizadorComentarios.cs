namespace OpinionClienteDwh.Data.Interfaces;

public interface ITokenizadorComentarios
{
    Task InicializarAsync();

    Dictionary<string, int> TokenizarComentario(string comentario);
}