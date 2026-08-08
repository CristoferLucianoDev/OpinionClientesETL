using Microsoft.Extensions.Logging;

namespace OpinionClienteDwh.Data.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _rutaArchivo;
    private readonly object _bloqueo = new();

    public FileLoggerProvider(string rutaCarpeta, string timestamp)
    {
        Directory.CreateDirectory(rutaCarpeta);
        _rutaArchivo = Path.Combine(rutaCarpeta, $"log_{timestamp}.txt");
    }

    public ILogger CreateLogger(string categoryName)
        => new FileLogger(categoryName, _rutaArchivo, _bloqueo);

    public void Dispose() { }
}