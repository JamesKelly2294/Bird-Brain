using System;
using System.Collections.Generic;
using ShittyWizzard.Utilities;

namespace ShittyWizzard.Model
{
	public abstract class Manager<T> : GenericEventDispatcher<T>
	{
		public Manager ()
		{
			SetupEvents ();
		}

		public void RegisterEvent (GenericEventType eventType, GenericEventHandler<T> callback, T caller = default(T))
		{
			if (caller == null) {
				// universal callback
				UniversalEvents [eventType] += callback;
			} else {
				// dependent callback
				if (!DependentEvents [eventType].ContainsKey (caller)) {
					DependentEvents [eventType].Add (caller, null);
				}
				DependentEvents [eventType] [caller] += callback;
			}
		}

		public void UnregisterEvent (GenericEventType eventType, GenericEventHandler<T> callback, T caller = default(T))
		{
			UniversalEvents [eventType] -= callback;
			if (caller != null) {
				// dependent callback
				if (DependentEvents [eventType].ContainsKey (caller)) {
					DependentEvents [eventType] [caller] -= callback;
				}
			}
		}

		public void EmitEvent (GenericEventType eventType, T caller)
		{
			if (UniversalEvents [eventType] != null) {
				UniversalEvents [eventType] (caller);
			}
			if (caller != null && DependentEvents [eventType].ContainsKey (caller)) {
				DependentEvents [eventType] [caller] (caller);
			}

		}

		public void UnregisterEventForInstance(T t) {
			if (t != null) {
				// dependent callback
				foreach (var e in Enum.GetValues(typeof(GenericEventType))) {
					if (DependentEvents [(GenericEventType)e].ContainsKey (t)) {
						DependentEvents [(GenericEventType)e].Remove (t);
					}
				}
			}
		}

		public Dictionary<GenericEventType, GenericEventHandler<T>> UniversalEvents {
			get;
			protected set;
		}

		public Dictionary<GenericEventType, Dictionary<T, GenericEventHandler<T>>> DependentEvents {
			get;
			protected set;
		}

		void SetupEvents ()
		{
			UniversalEvents = new Dictionary<GenericEventType, GenericEventHandler<T>> ();
			foreach (var e in Enum.GetValues(typeof(GenericEventType))) {
				UniversalEvents [(GenericEventType)e] = null;
			}

			DependentEvents = new Dictionary<GenericEventType, Dictionary<T, GenericEventHandler<T>>> ();
			foreach (var e in Enum.GetValues(typeof(GenericEventType))) {
				DependentEvents [(GenericEventType)e] = new Dictionary<T, GenericEventHandler<T>> ();
			}
		}
	}
}
