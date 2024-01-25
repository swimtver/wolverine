using TIBCO.EMS;
using Wolverine.Tibco.Internals;

namespace Wolverine.Tibco;

public static class TibcoTransportExtensions
{
    internal static TibcoTransport TibcoTransport(this WolverineOptions endpoints)
    {
        return endpoints.Transports.GetOrCreate<TibcoTransport>();
    }
    
    public static TibcoTransportExpression UseTibco(
        this WolverineOptions options,
        Action<ConnectionFactory> configure)
    {
        var transport = options.TibcoTransport();
        configure(transport.ConnectionFactory);
        return new TibcoTransportExpression(transport, options);
    }
    
    public static TibcoTransportExpression UseTibco(this WolverineOptions options, string tibcoUri) 
        => options.UseTibco(factory => factory.SetServerUrl(tibcoUri));
    
    public static TibcoListenerConfiguration ListenToTibcoTopic(
        this WolverineOptions endpoints,
        string topicName,
        Action<ITibcoTopic> configure = null)
    {
        var transport = endpoints.TibcoTransport();

        var endpoint = transport.Topics[topicName];
        endpoint.EndpointName = topicName;
        endpoint.IsListener   = true;
        configure?.Invoke(endpoint);

        return new TibcoListenerConfiguration(endpoint);
    }
}