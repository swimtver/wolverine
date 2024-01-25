using Wolverine.Configuration;

namespace Wolverine.Tibco;

public class TibcoListenerConfiguration : ListenerConfiguration<TibcoListenerConfiguration, TibcoEndpoint>
{
    public TibcoListenerConfiguration(TibcoEndpoint endpoint) : base(endpoint)
    {
    }

    public TibcoListenerConfiguration ConfigureMapping(Action<ITibcoEnvelopeMapper> configure)
    {
        add(e => e.ConfigureMapping(configure));
        return this;
    }
}