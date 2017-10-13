using System;
using System.Collections.Generic;

namespace ShittyWizzard.Utilities
{
	public enum GenericEventType
	{
		CREATED,
		CHANGED,
		COMPLETED,
		FINALIZED,
		CANCELED,
		INITIALIZED
	};

	public class Event {
		public Dictionary<string, object> payload;

		public Event(Dictionary<string, object> payload) {
			this.payload = payload;
		}
	}

	public delegate void GenericEventHandler<T> (T sender);

	public interface GenericEventDispatcher<T>
	{
		void RegisterEvent (GenericEventType eventType, GenericEventHandler<T> callback, T caller);

		void UnregisterEvent (GenericEventType eventType, GenericEventHandler<T> callback, T caller);

		void EmitEvent (GenericEventType eventType, T caller);

		Dictionary<GenericEventType, GenericEventHandler<T>> UniversalEvents { get; }

		Dictionary<GenericEventType, Dictionary<T, GenericEventHandler<T>>> DependentEvents { get; }
	}
}
