using System.Linq.Expressions;
using TIBCO.EMS;
using Wolverine.Transports;

namespace Wolverine.Tibco;

public interface ITibcoEnvelopeMapper : IEnvelopeMapper<Message, Message>
{
    void MapProperty(
        Expression<Func<Envelope, object>> property,
        Action<Envelope, Message> readFromIncoming,
        Action<Envelope, Message> writeToOutgoing);

    public void MapOutgoingProperty(
        Expression<Func<Envelope, object>> property,
        Action<Envelope, Message> writeToOutgoing);
    
    void MapPropertyToHeader(Expression<Func<Envelope, object>> property, string headerKey);
}