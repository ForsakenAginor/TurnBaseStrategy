using UnityEngine;
using System.Collections;
using System;

namespace Polygen.HexagonGenerator
{
	public class UpdatableData : ScriptableObject
	{
		public event System.Action onNotifyUpdatedValues;

		public void ForceNotifyUpdatedValues()
		{
			onNotifyUpdatedValues?.Invoke();
		}

		public void RegisterUpdatedValues(System.Action cb)
		{
			onNotifyUpdatedValues += cb;
		}

		public void UnregisterUpdatedValues(System.Action cb)
		{
			onNotifyUpdatedValues -= cb;
		}

		public void UnregisterAllUpdatedValues()
		{
			if (onNotifyUpdatedValues != null)
				foreach (var d in onNotifyUpdatedValues.GetInvocationList())
					onNotifyUpdatedValues -= (d as System.Action);
		}

	}
}
