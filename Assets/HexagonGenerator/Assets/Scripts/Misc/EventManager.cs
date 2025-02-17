

namespace Polygen.HexagonGenerator
{
	public class EventManager
	{
		static event System.Action onInitializeOnLaunch;

		public static void OnInitializeOnLaunch()
		{
			onInitializeOnLaunch?.Invoke();
		}

		public static void RegisterInitializeOnLaunch(System.Action cb)
		{
			onInitializeOnLaunch += cb;
		}

		public static void UnregisterInitializeOnLaunch(System.Action cb)
		{
			onInitializeOnLaunch -= cb;
		}


		static event System.Action onEditorWindowOpen;

		public static void OnEditorWindowOpen()
		{
			onEditorWindowOpen?.Invoke();
		}

		public static void RegisterEditorWindowOpen(System.Action cb)
		{
			onEditorWindowOpen += cb;
		}

		public static void UnregisterEditorWindowOpen(System.Action cb)
		{
			onEditorWindowOpen -= cb;
		}



		static event System.Action onValuesUpdated;

		public static void OnValuesUpdated()
		{
			onValuesUpdated?.Invoke();
		}

		public static void RegisterValuesUpdated(System.Action cb)
		{
			onValuesUpdated += cb;
		}

		public static void UnregisterValuesUpdated(System.Action cb)
		{
			onValuesUpdated -= cb;
		}

		static event System.Action onDatabaseRefresh;

		public static void OnDatabaseRefresh()
		{
			onDatabaseRefresh?.Invoke();
		}

		public static void RegisterDatabaseRefresh(System.Action cb)
		{
			onDatabaseRefresh += cb;
		}

		public static void UnregisterDatabaseRefresh(System.Action cb)
		{
			onDatabaseRefresh -= cb;
		}

#if UNITY_EDITOR

		static event System.Action onSceneSaved;

		public static void OnSceneSaved(UnityEngine.SceneManagement.Scene scene)
		{
			onSceneSaved?.Invoke();
		}

		public static void RegisterSceneSaved(System.Action cb)
		{
			onSceneSaved += cb;
		}

		public static void UnregisterSceneSaved(System.Action cb)
		{
			onSceneSaved -= cb;
		}
#endif
	}
}
