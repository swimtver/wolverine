using System.Text;
using Microsoft.Extensions.Logging;
using TIBCO.EMS;
using Wolverine.Transports;
using Wolverine.Util;

namespace Wolverine.Tibco.Internals;

internal class MessageListener : IMessageListener
{
    private readonly string _topicName;
    private readonly string? _defaultMessageType;
    private readonly IListener _listener;
    private readonly ITibcoEnvelopeMapper _mapper;
    private readonly IReceiver _receiver;
    private readonly ILogger _logger;

    public MessageListener(TibcoEndpoint endpoint, IListener listener, IReceiver receiver, ILogger logger)
    {
        _topicName          = TibcoEndpoint.TopicNameForUri(endpoint.Uri);
        _defaultMessageType = endpoint.MessageType?.ToMessageTypeName();
        _listener           = listener;
        _mapper             = endpoint.BuildMapper();
        _receiver           = receiver;
        _logger             = logger;
    }

    public async void OnMessage(Message message)
    {
        var envelope = new TibcoEnvelope(message)
        {
            TopicName = _topicName,
            Data = GetData(message),
            CorrelationId = message.CorrelationID
        };

        try
        {
            _mapper.MapIncomingToEnvelope(envelope, message);
            envelope.MessageType ??= _defaultMessageType;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failure to envelope an incoming message with {Id}",
                envelope.Id
            );
        }

        try
        {
            await _receiver.ReceivedAsync(_listener, envelope);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failure to receive an incoming message with {Id}",
                envelope.Id
            );
        }
    }

    private byte[] GetData(Message message)
    {
        return message switch
        {
            TextMessage text => Encoding.UTF8.GetBytes(text.Text),
            _ => throw new ArgumentException("Do not support any type of Message except TextMessage")
        };
    }
}