using System.Collections.Generic;
using Bimicore.Lib.ScriptableEventSystem;
using UnityEditor;
using UnityEngine;

namespace Bimicore.Lib
{
	/// <summary>
	/// This class makes Editor display all listeners of each GameEvent
	/// </summary>
	[CustomEditor(typeof(GameEvent), true)]
	public class GameEventEditor : Editor
	{
		private List<GameEventListener> _listenersEditor = new List<GameEventListener>();

		private void OnEnable()
		{
			DetectListenersAndAddToList();
		}

		// Detect all GameEventListeners and add to list the ones who correspond to this event
		private void DetectListenersAndAddToList()
		{
			GameEvent thisEvent = target as GameEvent;
			_listenersEditor.Clear();

			foreach (GameEventListener allListeners in Resources.FindObjectsOfTypeAll(typeof(GameEventListener)))
			{
				if (allListeners.IsSubscribedOnEvent(thisEvent))
					_listenersEditor.Add(allListeners);
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			DisplayListeners();
		}

		private void DisplayListeners()
		{
			GUILayout.BeginVertical("box");
			GUILayout.Label("Listeners");

			foreach (var i in _listenersEditor)
				EditorGUILayout.ObjectField(i, typeof(GameEvent), true);
		
			GUILayout.EndVertical();
		}
	}
}