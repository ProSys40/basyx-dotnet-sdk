using BaSyx.Models.Communication;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.ServiceProvider.EventDriven;

/// <summary>
/// Data for the events produced by a submodel
/// </summary>
public record SubmodelEventData
{
    /// <summary>
    /// Data for the events produced by a submodel
    /// </summary>
    public SubmodelEventData(string submodelShortPath, DateTime? timestamp = null)
    {
        SubmodelShortPath = submodelShortPath;

        if (timestamp.HasValue)
        {
            Timestamp = timestamp;
        }
        else
        {
            Timestamp = DateTime.UtcNow;
        }
    }

    public string SubmodelShortPath { get; init; }
    public DateTime? Timestamp { get; init; }

    public void Deconstruct(out string submodelShortPath, out DateTime? timestamp)
    {
        submodelShortPath = this.SubmodelShortPath;
        timestamp = this.Timestamp;
    }
}

/// <summary>
/// Record for changes or updates on submodel element events
/// </summary>
public record CreateOrUpdateSubmodelEventData : SubmodelEventData
{
    public CreateOrUpdateSubmodelEventData(string submodelShortPath, DateTime? timestamp = null) 
        : base(submodelShortPath, timestamp)
    {
        
    }
}

/// <summary>
/// Event for update of the value of submodel element
/// </summary>
public record UpdatedValueSubmodelEventData : SubmodelEventData
{
    public UpdatedValueSubmodelEventData(string submodelShortPath, string submodelElementValue, 
        DateTime? timestamp = null) 
        : base(submodelShortPath, timestamp)
    {
        
    }
}

/// <summary>
/// Event data for a deleted submodel element
/// </summary>
public record DeleteSubmodelElementEventData : SubmodelEventData
{
    public DeleteSubmodelElementEventData(string submodelShortPath, DateTime? timestamp = null) : 
        base(submodelShortPath, timestamp)
    {
    }
}

public record SubmodelInvokedOperationEventDataBase : SubmodelEventData
{
    public SubmodelInvokedOperationEventDataBase(string submodelShortPath, string operationIdShort, 
        InvocationRequest request, DateTime? timestamp = null) 
        : base(submodelShortPath, timestamp)
    {
        OperationIdShort = operationIdShort;
        RequestId = request.RequestId;
        Request = request;
    }

    /// <summary>
    /// Short id of the invoked operation
    /// </summary>
    public string OperationIdShort { get; }

    /// <summary>
    /// Id of the request
    /// </summary>
    public string RequestId { get; }

    /// <summary>
    /// Request for the method invocation
    /// </summary>
    public InvocationRequest Request { get; set; }
}

/// <summary>
/// Event data for synchronous invoked operations
/// </summary>
public record SubmodelInvokedOperationEventData: SubmodelInvokedOperationEventDataBase
{
    /// <summary>
    /// Response to the method invocation
    /// </summary>
    public InvocationResponse? Response { get; }

    public SubmodelInvokedOperationEventData(string submodelShortPath, string operationIdShort, 
        InvocationRequest request, InvocationResponse? response = null, DateTime? timestamp = null) 
        : base(submodelShortPath, operationIdShort, request, timestamp)
    {
        Response = response;
    }
}

/// <summary>
/// Event data for asynchronously invoked operations on submodels
/// </summary>
public record SubmodelInvokedOperationAsyncEventData : SubmodelInvokedOperationEventDataBase
{
    public SubmodelInvokedOperationAsyncEventData(string submodelShortPath, string operationIdShort, 
        InvocationRequest request, CallbackResponse? response = null, DateTime? timestamp = null) : 
        base(submodelShortPath, operationIdShort, request, timestamp)
    {
        Callback = response;
    }

    /// <summary>
    /// Callback response by the asynchronous invocation
    /// </summary>
    public CallbackResponse? Callback { get; }
}

/// <summary>
/// Published event envelope
/// </summary>
public record SubmodelEventInvokedEventData : SubmodelEventData
{
    public SubmodelEventInvokedEventData(string submodelShortPath, IEventMessage eventMessage, 
        DateTime? timestamp = null) : base(submodelShortPath, timestamp)
    {
        EventMessage = eventMessage;
    }

    /// <summary>
    /// Event message given to the published event
    /// </summary>
    public IEventMessage EventMessage { get; }
}