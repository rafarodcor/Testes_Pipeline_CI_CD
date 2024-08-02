using JornadaMilhas.API.DTO.Request;
using JornadaMilhas.API.DTO.Response;
using JornadaMilhas.Dominio.Entidades;

namespace JornadaMilhas.API.Service;

public class OfertaViagemConverter
{
    private readonly RotaConverter _rotaConverter;
    private readonly PeriodoConverter _periodoConverter;

    public OfertaViagemConverter(RotaConverter rotaConverter, PeriodoConverter periodoConverter)
    {
        _rotaConverter = rotaConverter;
        _periodoConverter = periodoConverter;
    }

    public OfertaViagem RequestToEntity(OfertaViagemRequest ofertaViagemRequest)
    {
        if (ofertaViagemRequest.rota is null)
            return new OfertaViagem(null, _periodoConverter.RequestToEntity(ofertaViagemRequest.periodo), ofertaViagemRequest.preco);

        if (ofertaViagemRequest.periodo is null)
            return new OfertaViagem(_rotaConverter.RequestToEntity(ofertaViagemRequest.rota), null, ofertaViagemRequest.preco);

        if (ofertaViagemRequest.preco <= 0)
            return new OfertaViagem(_rotaConverter.RequestToEntity(ofertaViagemRequest.rota), _periodoConverter.RequestToEntity(ofertaViagemRequest.periodo), ofertaViagemRequest.preco);

        return new OfertaViagem(_rotaConverter.RequestToEntity(ofertaViagemRequest.rota), _periodoConverter.RequestToEntity(ofertaViagemRequest.periodo), ofertaViagemRequest.preco);
    }

    public OfertaViagemResponse EntityToResponse(OfertaViagem ofertaViagem)
    {
        return new OfertaViagemResponse(ofertaViagem.Id, _rotaConverter.EntityToResponse(ofertaViagem.Rota), _periodoConverter.EntityToResponse(ofertaViagem.Periodo), ofertaViagem.Preco);
    }

    public ICollection<OfertaViagemResponse> EntityListToResponseList(IEnumerable<OfertaViagem> ofertas)
    {
        return ofertas.Select(a => EntityToResponse(a)).ToList();
    }

    public ICollection<OfertaViagem> RequestListToEntityList(IEnumerable<OfertaViagemRequest> ofertas)
    {
        return ofertas.Select(a => RequestToEntity(a)).ToList();
    }
}