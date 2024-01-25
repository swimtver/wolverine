using Wolverine.Configuration;
using Wolverine.Tibco.Internals;

namespace Wolverine.Tibco;

public class TibcoSubscriberConfiguration : SubscriberConfiguration<TibcoSubscriberConfiguration, TibcoEndpoint>
{
    public TibcoSubscriberConfiguration(TibcoEndpoint endpoint) : base(endpoint)
    {
    }
}