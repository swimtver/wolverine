using TIBCO.EMS;

namespace Wolverine.Tibco;

public interface ITibcoTopic
{
    public string Selector { get; set; }
    public SessionMode SessionMode { get; set; }
    public bool IsDurable { get; set; }
}