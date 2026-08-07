using Catalyst;
using Mosaik.Core;
using OpinionClienteDwh.Data.Interfaces;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace OpinionClienteDwh.Data.Nlp;

public sealed class TokenizadorComentarios : ITokenizadorComentarios
{
    private Pipeline? _nlp;
    private HashSet<string>? _stopWords;

    public async Task InicializarAsync()
    {
        if (_nlp != null)
            return;

        Catalyst.Models.Spanish.Register();
        Mosaik.Core.Storage.Current = new DiskStorage("catalyst-models");

        _nlp = await Pipeline.ForAsync(Language.Spanish);
        _stopWords = CargarStopWords();
    }

    public Dictionary<string, int> TokenizarComentario(string comentario)
    {
        if (_nlp == null || _stopWords == null)
            throw new InvalidOperationException("InicializarAsync() debe llamarse antes de tokenizar.");

        var doc = new Catalyst.Document(comentario, Language.Spanish);
        _nlp.ProcessSingle(doc);

        var tokens = doc.ToTokenList();
        Dictionary<string, int> palabrasClave = [];

        foreach (var token in tokens)
        {
            var palabra = NormalizarPalabra(token.Value);

            if (string.IsNullOrWhiteSpace(palabra))
                continue;

            if (_stopWords.Contains(palabra))
                continue;

            palabrasClave[palabra] = palabrasClave.GetValueOrDefault(palabra, 0) + 1;
        }

        return palabrasClave;
    }

    private static string NormalizarPalabra(string valor)
    {
        var palabra = valor.ToLowerInvariant();
        palabra = new string([.. palabra.Where(char.IsLetter)]);
        palabra = palabra.Normalize(NormalizationForm.FormD);
        palabra = new string([.. palabra.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)]);
        return palabra;
    }

    private static HashSet<string> CargarStopWords()
    {
        var ensamblado = Assembly.GetExecutingAssembly();
        const string recurso = "OpinionClienteDwh.Data.Nlp.Resources.stopwords-es.txt";

        using var stream = ensamblado.GetManifestResourceStream(recurso)
            ?? throw new InvalidOperationException($"No se encontro el recurso embebido '{recurso}'.");
        using var lector = new StreamReader(stream);

        var stopWords = new HashSet<string>();
        string? linea;
        while ((linea = lector.ReadLine()) != null)
        {
            var normalizada = NormalizarPalabra(linea);
            if (!string.IsNullOrWhiteSpace(normalizada))
                stopWords.Add(normalizada);
        }

        return stopWords;
    }
}