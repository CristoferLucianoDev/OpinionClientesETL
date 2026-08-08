using Microsoft.Extensions.Logging;

namespace OpinionClienteDwh.Data.Logging;

public sealed class FileLogger(string categoryName, string rutaArchivo, object bloqueo) : ILogger
{
    private readonly string _categoryName = categoryName;
    private readonly string _rutaArchivo = rutaArchivo;
    private readonly object _bloqueo = bloqueo;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var linea = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName}: {formatter(state, exception)}";

        if (exception != null)
            linea += Environment.NewLine + exception;

        lock (_bloqueo)
        {
            File.AppendAllText(_rutaArchivo, linea + Environment.NewLine);
        }
    }
}