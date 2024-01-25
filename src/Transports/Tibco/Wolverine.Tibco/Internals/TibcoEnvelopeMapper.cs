using TIBCO.EMS;
using Wolverine.Configuration;
using Wolverine.Transports;

namespace Wolverine.Tibco.Internals;

internal class TibcoEnvelopeMapper : EnvelopeMapper<Message, Message>, ITibcoEnvelopeMapper
{
    public TibcoEnvelopeMapper(Endpoint endpoint) : base(endpoint)
    {
    }

    protected override void writeIncomingHeaders(Message incoming, Envelope envelope)
    {
        var propertyNames = incoming.PropertyNames;

        if (propertyNames == null) return;

        while (propertyNames.MoveNext())
        {
            if (propertyNames.Current is string name)
            {
                if (tryReadIncomingHeader(incoming, name, out var value))
                {
                    envelope.Headers[name] = value;
                }
            }
        }

        envelope.Headers[TibcoEnvelopeConstants.MessageIdKey] = incoming.MessageID;
        envelope.Headers[TibcoEnvelopeConstants.TimestampKey] = incoming.Timestamp.ToString();
    }

    protected override void writeOutgoingHeader(Message outgoing, string key, string value)
    {
        outgoing.SetStringProperty(key, value);
    }

    protected override bool tryReadIncomingHeader(Message incoming, string key, out string? value)
    {
        if (incoming.PropertyExists(key))
        {
            value = incoming.GetStringProperty(key);
            return true;
        }

        value = default!;
        return false;
    }
}