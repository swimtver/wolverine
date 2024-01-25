using Microsoft.Extensions.Logging;
using TIBCO.EMS;
using Wolverine.Transports;

namespace Wolverine.Tibco.Internals;

internal class TibcoListener : IListener, IDisposable
{
    private readonly TopicConnection _connection;
    private readonly IReceiver _receiver;
    private readonly ILogger<TibcoListener> _logger;
    private readonly TopicSession _session;
    private readonly TopicSubscriber _consumer;

    public Uri Address { get; }

    public TibcoListener(
        TopicConnection connection,
        TibcoEndpoint endpoint,
        IReceiver receiver,
        ILogger<TibcoListener> logger)
    {
        _connection = connection;
        _receiver   = receiver;
        _logger     = logger;

        _session = _connection.CreateTopicSession(transacted: false, acknowledgeMode: endpoint.SessionMode);
        var topicName = TibcoEndpoint.TopicNameForUri(endpoint.Uri);
        var topic = _session.CreateTopic(topicName);

        _consumer = endpoint.IsDurable
            ? _session.CreateDurableSubscriber(topic, name: _connection.ClientID, endpoint.Selector, noLocal: false)
            : _session.CreateSubscriber(topic, endpoint.Selector, noLocal: false);

        _consumer.MessageListener = new MessageListener(endpoint, this, receiver, logger);
        Address = endpoint.Uri;
    }

    public void Dispose()
    {
        _receiver.Dispose();
        _session.Close();
    }

    public ValueTask CompleteAsync(Envelope envelope)
    {
        if(_session.AcknowledgeMode == Session.NO_ACKNOWLEDGE || _session.AcknowledgeMode == Session.AUTO_ACKNOWLEDGE)
        {
            return ValueTask.CompletedTask;
        }
        
        if (envelope is TibcoEnvelope tibcoEnvelope)
        {
            try
            {
                tibcoEnvelope.OriginalMessage.Acknowledge();
                _logger.LogDebug(
                    "[{MessageId}] Message from {TopicName} has been acked",
                    tibcoEnvelope.MessageId,
                    tibcoEnvelope.TopicName
                );
            }
            catch(Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{MessageId}] Can't send acknowledge to EMS server",
                    tibcoEnvelope.MessageId
                );
            }
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DeferAsync(Envelope envelope)
    {
        // Really just a retry
        return _receiver.ReceivedAsync(this, envelope);
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask StopAsync()
    {
        _consumer.Close();
        return ValueTask.CompletedTask;
    }
}