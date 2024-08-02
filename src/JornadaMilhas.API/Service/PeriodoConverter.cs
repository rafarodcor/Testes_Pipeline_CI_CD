using JornadaMilhas.API.DTO.Request;
using JornadaMilhas.API.DTO.Response;
using JornadaMilhas.Dominio.ValueObjects;

namespace JornadaMilhas.API.Service;

public class PeriodoConverter
{
    public Periodo RequestToEntity(PeriodoRequest periodoRequest)
        => new Periodo(periodoRequest.dataInicial, periodoRequest.dataFinal);

    public PeriodoResponse EntityToResponse(Periodo periodo)
        => new PeriodoResponse(periodo.DataInicial, periodo.DataFinal);
}