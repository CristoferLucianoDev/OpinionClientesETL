# OpinionClientes

Proceso **ETL** (Extract, Transform, Load) para el análisis de opiniones de clientes. Extrae datos desde tres fuentes heterogéneas (encuestas en CSV, reseñas web en base de datos, comentarios sociales vía API), los valida y transforma (resolución de claves de dimensión, tokenización NLP en español), y los carga en un data warehouse (`OpinionesOLAP`).

Proyecto académico desarrollado para la Electiva 1 (ITLA).

## Estructura de la solución

```
OpinionClientesETL.slnx
│
├── OpinionClienteDwh.Data/      Class library: toda la lógica de negocio (Extract, Transform, Load)
├── OpinionClienteDwh.Worker/    Worker Service: host y composition root del proceso ETL
└── OpinionClienteDwh.Api/       Web API: simula una red social externa (fuente mock de comentarios)
```

### `OpinionClienteDwh.Data`

Organizado por responsabilidad, sin jerarquía de capas formales:

| Carpeta | Contenido |
|---|---|
| `Interfaces/` | Contratos (`IExtractor<T>`, `IValidator<T>`, `IDao<T>`, etc.) |
| `Dtos/` | Records de transferencia de datos, inmutables |
| `Extractors/` | Implementaciones genéricas de `IExtractor<T>` (CSV, base de datos, API) |
| `Validators/` | Implementaciones de `IValidator<T>` |
| `Dao/` | Acceso a `OpinionesOLTP` (fuentes de Extract) |
| `Services/` | Orquestador de extracción, `DataLoader`, `ExtractResult` |
| `Cache/` | `DimensionKeyCache` |
| `Nlp/` | Tokenizador de comentarios (Catalyst, modelo en español) |
| `Staging/` | Lectura de los JSON intermedios entre Extract y Load |
| `Load/` | Todo lo que escribe hacia `OpinionesOLAP` (conexión base, DAOs, orquestación de Transform/Load) |
| `Common/` | Utilidades genéricas de ADO.NET (`SqlServerConnection`, `TvpBuilder`, `EsquemaTablaStaging`) |
| `Models/` | Modelos internos que no son DTOs de fuente |
| `Persistence/` | `DbContext` de solo lectura y sus entidades (EF Core) |

## Arquitectura: contratos + inyección de dependencias

Cada responsabilidad vive detrás de una interfaz genérica (`IExtractor<T>`, `IValidator<T>`, `IDao<T>`) resuelta por inyección de dependencias nativa de .NET — no hay capas formales (`Domain/`, `Application/`, `Infrastructure/`) ni microservicios distribuidos.

- **Extractores genéricos**: `CsvExtractor<T>`, `DatabaseExtractor<T>`, `ApiExtractor<T>` no conocen el tipo de dato, la fuente ni la ruta/endpoint específicos — todo se configura por registro de DI en `Program.cs`. Agregar una fuente nueva no requiere una clase nueva.
- **Acceso a datos**: por defecto ADO.NET + Stored Procedures (`SqlServerConnection`); EF Core solo para lectura de datos de referencia (`ClienteDao`, `ProductoDao`) y en la API mock. Las escrituras (staging, MERGE, bulk copy) son siempre ADO.NET puro.
- **Resiliencia**: `DatabaseExtractor<T>` y `ApiExtractor<T>` usan Polly con reintento y backoff exponencial (3 intentos). El orquestador aísla el fallo de cada fuente — si una falla, las demás continúan.
- **Staging intermedio**: `DataLoader` escribe cada fuente extraída como un JSON independiente en una carpeta de Staging (con timestamp de corrida). `Load` lee exclusivamente de esos archivos, nunca de `OpinionesOLTP` directamente.

Más detalle de las decisiones de arquitectura confirmadas está en `ARQUITECTURA.md` (no versionado en este repositorio).

## Stack técnico

- .NET 8 (C# 12)
- Entity Framework Core 8 (solo lectura de referencias / API mock)
- ADO.NET + Stored Procedures (Extract/Load principal)
- CsvHelper — extracción de encuestas en CSV
- Catalyst + Catalyst.Models.Spanish — tokenización NLP de comentarios
- Polly — reintentos con backoff exponencial
- Swashbuckle — documentación Swagger de la API mock

## Requisitos

- .NET 8 SDK
- SQL Server con las bases `OpinionesOLTP` y `OpinionesOLAP`

## Configuración

Cada proyecto ejecutable (`OpinionClienteDwh.Worker`, `OpinionClienteDwh.Api`) requiere su propio `appsettings.json` local (no versionado) con, como mínimo:

```json
{
  "ConnectionStrings": {
    "OpinionesOltp": "Server=.;Database=OpinionesOLTP;Trusted_Connection=True;TrustServerCertificate=True;",
    "OpinionesOlap": "Server=.;Database=OpinionesOLAP;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "ApiMock": {
    "BaseUrl": "https://localhost:7277/"
  },
  "RutasArchivos": {
    "Surveys": "ruta\\al\\archivo\\surveys.csv",
    "Staging": ".\\Staging"
  }
}
```

## Ejecución

```bash
# Levantar la API mock (fuente de comentarios sociales)
dotnet run --project OpinionClienteDwh.Api

# Ejecutar el proceso ETL completo (Extract -> Staging -> Transform/Load a OLAP)
dotnet run --project OpinionClienteDwh.Worker
```

El `Worker` extrae de las tres fuentes en paralelo, valida cada registro, escribe el resultado en `OpinionClienteDwh.Worker/Staging/` y luego ejecuta la carga a `OpinionesOLAP` leyendo desde esos archivos de staging.
