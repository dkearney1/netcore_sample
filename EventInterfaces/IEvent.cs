using System;

namespace EventInterfaces
{
	public interface IEvent
	{
		Guid Id {get;}
		Guid? CorrelationId {get;}
		DateTimeOffset Created{get;}
	}
}