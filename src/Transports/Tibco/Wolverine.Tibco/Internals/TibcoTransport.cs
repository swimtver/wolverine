using JasperFx.Core;
using Microsoft.Extensions.Logging;
using TIBCO.EMS;
using Wolverine.Configuration;
using Wolverine.Runtime;
using Wolverine.Transports;

namespace Wolverine.Tibco.Internals;

public class TibcoTransport : BrokerTransport<TibcoEndpoint>, IDisposable
{
    private TopicConnection? _listenerConnection;
    private Connection? _sendingConnection;
    
    public Cache<string, TibcoEndpoint> Topics { get; }
    
    public ConnectionFactory ConnectionFactory { get; } = new TopicConnectionFactory();

    internal TopicConnection ListeningConnection => _listenerConnection;
    internal Connection SendingConnection => _sendingConnection;
    internal bool UseSenderConnectionOnly { get; set; }
    internal bool UseListenerConnectionOnly { get; set; }
    
    public TibcoTransport() : base("tibco", "Tibco EMS")
    {
        Topics = new Cache<string, TibcoEndpoint>(
            topicName => new TibcoEndpoint(this, topicName, EndpointRole.Application)
        );
    }
    
    public void Dispose()
    {
        _listenerConnection?.Close();
        _sendingConnection?.Close();
    }

    protected override IEnumerable<TibcoEndpoint> endpoints() => Topics;

    protected override TibcoEndpoint findEndpointByUri(Uri uri)
    {
        var topicName = TibcoEndpoint.TopicNameForUri(uri);
        return Topics[topicName];
    }

    public override ValueTask ConnectAsync(IWolverineRuntime runtime)
    {
        static void listenToEvents(string connectionName, Connection connection, ILogger logger)
        {
            logger.LogInformation("Opened new '{Name}' connection to {Server}", connectionName, connection.ActiveURL);
            
            connection.ExceptionHandler += (_, args) =>
            {
                logger.LogError(
                    args.Exception,
                    "Tibco connection callback exception on {Name} connection",
                    connectionName
                );
            };
        }
        
        var logger = runtime.LoggerFactory.CreateLogger<TibcoTransport>();
        
        if (_listenerConnection == null && !UseSenderConnectionOnly)
        {
            _listenerConnection = (TopicConnection)ConnectionFactory.CreateConnection();
            _listenerConnection.Start();
            listenToEvents("Listener", _listenerConnection, logger);
        }

        // if (_sendingConnection == null && !UseListenerConnectionOnly)
        // {
        //     _sendingConnection = ConnectionFactory.CreateConnection();
        //     _sendingConnection.Start();
        //     listenToEvents("Sender", _sendingConnection, logger);
        // }

        return ValueTask.CompletedTask;
    }

    public override IEnumerable<PropertyColumn> DiagnosticColumns()
    {
        yield break;
    }
}