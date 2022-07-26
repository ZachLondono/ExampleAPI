using System.Text.Json.Serialization;

namespace ExampleAPI.Common.Domain;

/// <summary>
/// A correlated event is an special type of domain event which was triggered by another event (Hence one event is correlated with the other)
/// </summary>
/// <param name="AggregateId">The id of the aggregate to which this event happened</param>
/// <param name="CorrelatedId">The id of the event to which this event is correlated</param>
public abstract record CorrelatedDomainEvent(Guid AggregateId, [property: JsonIgnore] Guid CorrelatedId) : DomainEvent(AggregateId);