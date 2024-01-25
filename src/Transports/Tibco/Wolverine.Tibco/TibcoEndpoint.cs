using Microsoft.Extensions.Logging;
using TIBCO.EMS;
using Wolverine.Configuration;
using Wolverine.Runtime;
using Wolverine.Tibco.Internals;
using Wolverine.Transports;
using Wolverine.Transports.Sending;

namespace Wolverine.Tibco;



public class TibcoEndpoint : Endpoint, IBrokerEndpoint, ITibcoTopic
{
    private Action<TibcoEnvelopeMapper> _customizeMapping = m => { };
    
    public TibcoTransport Parent { get; }
    public string Selector { get; set; }
    public SessionMode SessionMode { get; set; }
    public bool IsDurable { get; set; }

    public TibcoEndpoint(TibcoTransport transport, string topicName, EndpointRole role) 
        : base(new Uri($"tibco://topic/{topicName}"), role)
    {
        Parent = transport;
    }
    
    public static string TopicNameForUri(Uri uri)
    {
        return uri.Segments.Last().Trim('/');
    }

    public override ValueTask<IListener> BuildListenerAsync(IWolverineRuntime runtime, IReceiver receiver)
    {
        var listener = new TibcoListener(
            Parent.ListeningConnection,
            this,
            receiver,
            runtime.LoggerFactory.CreateLogger<TibcoListener>()
        );
        return ValueTask.FromResult((IListener)listener);
    }
    
    public void ConfigureMapping(Action<ITibcoEnvelopeMapper> configure)
    {
        _customizeMapping = configure;
    }

    protected override ISender CreateSender(IWolverineRuntime runtime)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> CheckAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask TeardownAsync(ILogger logger)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetupAsync(ILogger logger)
    {
        throw new NotImplementedException();
    }

    internal ITibcoEnvelopeMapper? BuildMapper()
    {
        var mapper = new TibcoEnvelopeMapper(this);
        _customizeMapping.Invoke(mapper);
        return mapper;
    }
}