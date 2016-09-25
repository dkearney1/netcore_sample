using System;
using EventInterfaces;

namespace FileEvents
{
	public sealed class FileRemoved : IEvent
	{
		public Guid Id {get;set;}
		public Guid? CorrelationId {get;set;}
		public DateTimeOffset Created {get;set;}
		public string FullPath {get;set;}

		public FileRemoved()
		{
			Id = Guid.NewGuid();
			Created = DateTimeOffset.Now;
		}

		public FileRemoved(string fullPath)
			: this()
		{
			FullPath = fullPath;
		}
	}
}