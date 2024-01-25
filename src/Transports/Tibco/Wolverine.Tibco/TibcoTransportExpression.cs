using Wolverine.Tibco.Internals;
using Wolverine.Transports;

namespace Wolverine.Tibco;

public class TibcoTransportExpression: BrokerExpression<TibcoTransport, TibcoEndpoint, TibcoEndpoint,
    TibcoListenerConfiguration, TibcoSubscriberConfiguration, TibcoTransportExpression>
{
    public TibcoTransportExpression(TibcoTransport transport, WolverineOptions options) : base(transport, options)
    {
    }

    protected override TibcoListenerConfiguration createListenerExpression(TibcoEndpoint listenerEndpoint)
    {
        return new TibcoListenerConfiguration(listenerEndpoint);
    }

    protected override TibcoSubscriberConfiguration createSubscriberExpression(TibcoEndpoint subscriberEndpoint)
    {
        return new TibcoSubscriberConfiguration(subscriberEndpoint);
    }
    
    // /// <summary>
    // /// Turn on sender connection only in case if you only need to send messages
    // /// The sender connection won't be created in this case
    // /// </summary>
    // /// <returns></returns>
    // public TibcoTransportExpression UseListenerConnectionOnly()
    // {
    //     Transport.UseListenerConnectionOnly = true;
    //     Transport.UseSenderConnectionOnly   = false;
    //
    //     return this;
    // }
    //
    // /// <summary>
    // /// Turn on sender connection only in case if you only need to send messages
    // /// The listener connection won't be created in this case
    // /// </summary>
    // /// <returns></returns>
    // public TibcoTransportExpression UseSenderConnectionOnly()
    // {
    //     Transport.UseSenderConnectionOnly   = true;
    //     Transport.UseListenerConnectionOnly = false;
    //
    //     return this;
    // }
}