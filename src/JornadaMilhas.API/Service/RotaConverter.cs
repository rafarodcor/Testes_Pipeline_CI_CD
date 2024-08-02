using JornadaMilhas.API.DTO.Request;
using JornadaMilhas.API.DTO.Response;
using JornadaMilhas.Dominio.Entidades;

namespace JornadaMilhas.API.Service;

public class RotaConverter
{
    public Rota RequestToEntity(RotaRequest rotaRequest)
    {
        if (string.IsNullOrEmpty(rotaRequest.origem) || string.IsNullOrEmpty(rotaRequest.destino))
            return new Rota(null, null);
        return new Rota(rotaRequest.origem, rotaRequest.destino);
    }

    public RotaResponse EntityToResponse(Rota rota)
        => new RotaResponse(rota.Id, rota.Origem!, rota.Destino!);

    public ICollection<RotaResponse> EntityListToResponseList(IEnumerable<Rota> rotas)
        => rotas.Select(a => EntityToResponse(a)).ToList();

    public ICollection<Rota> RequestListToEntityList(IEnumerable<RotaRequest> rotas)
        => rotas.Select(a => RequestToEntity(a)).ToList();
}