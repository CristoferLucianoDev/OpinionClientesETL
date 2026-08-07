using Microsoft.Extensions.Configuration;
using OpinionClienteDwh.Data.Common;

namespace OpinionClienteDwh.Data.Load.Base;

public abstract class OpinionOlapConnection(IConfiguration configuration)
    : SqlServerConnection(configuration, "OpinionesOlap")
{
}