namespace BaSyx.ServiceProvider.EventDriven;

/// <summary>
/// Publishes lifecycle events for a submodel
/// </summary>
public interface ISubmodelEventPublisher
{
    /// <summary>
    /// Observabl that allows watching the events emitted by the submodel
    /// </summary>
    IObservable<SubmodelEventData> SubmodelEventObservable { get; }
}