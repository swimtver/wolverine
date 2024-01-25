using TIBCO.EMS;

namespace Wolverine.Tibco.Internals;

internal class TibcoEnvelope : Envelope
{
    public Message OriginalMessage { get; }
    public string MessageId { get; }
    
    
    public TibcoEnvelope(Message originalMessage)
    {
        OriginalMessage = originalMessage;
        MessageId       = originalMessage.MessageID;
    }
    
    public override string ToString()
    {
        var text = $"Envelope #{MessageId}";

        if (!string.IsNullOrEmpty(CorrelationId))
        {
            text += $"/CorrelationId={CorrelationId}";
        }

        if (Message != null)
        {
            text += $" ({Message.GetType().Name})";
        }

        if (Source != null)
        {
            text += $" from {Source}";
        }

        if (Destination != null)
        {
            text += $" to {Destination}";
        }


        return text;
    }
}