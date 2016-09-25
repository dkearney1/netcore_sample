using System;
using EventInterfaces;

namespace FileEvents
{
	public sealed class FileReceived : IEvent
	{
		public Guid Id {get;set;}
		public Guid? CorrelationId {get;set;}
		public DateTimeOffset Created {get;set;}
		public string FullPath {get;set;}

		public FileReceived()
		{
			Id = Guid.NewGuid();
			Created = DateTimeOffset.Now;
		}

		public FileReceived(string fullPath)
			: this()
		{
			FullPath = fullPath;
		}
	}
}