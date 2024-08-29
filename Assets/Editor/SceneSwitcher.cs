using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityToolbarExtender.Examples
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		private static string[]         _scenePaths;
		private static string[]         _sceneNames;

		private static string _currentSceneName = "Choose Scene";

		private static GenericMenu sceneCollectionsMenu;
		
		static SceneSwitchLeftButton()
		{
			if (_scenePaths == null || _scenePaths.Length != EditorBuildSettings.scenes.Length) 
			{
				List<string> scenePaths = new List<string>();
				List<string> sceneNames = new List<string>();

				foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) 
				{
					if (scene.path == null || scene.path.StartsWith("Assets") == false)
						continue;

					string scenePath = Application.dataPath + scene.path.Substring(6);

					scenePaths.Add(scenePath);
					sceneNames.Add(Path.GetFileNameWithoutExtension(scenePath));
				}

				_scenePaths = scenePaths.ToArray();
				_sceneNames = sceneNames.ToArray();
			}
			
			
			sceneCollectionsMenu = new GenericMenu();

			foreach (var sceneName in _sceneNames)
			{
				sceneCollectionsMenu.AddItem(new GUIContent(sceneName), false,
					() =>
					{
						_currentSceneName = sceneName;
						SceneHelper.StartScene(sceneName);
					});
			} 
			
			ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI()
		{
//			GUILayout.FlexibleSpace();
			
			if(GUILayout.Button("Choose Scene", EditorStyles.miniPullDown, GUILayout.Width(120)))
			{
				sceneCollectionsMenu.ShowAsContext();
			}

//			if(GUILayout.Button(new GUIContent("1", "Start Scene 1"), ToolbarStyles.commandButtonStyle))
//			{
//				SceneHelper.StartScene("ToolbarExtenderExampleScene1");
//			}
//
//			if(GUILayout.Button(new GUIContent("2", "Start Scene 2"), ToolbarStyles.commandButtonStyle))
//			{
//				SceneHelper.StartScene("ToolbarExtenderExampleScene2");
//			}
		}
	}

	static class SceneHelper
	{
		static string sceneToOpen;

		public static void StartScene(string sceneName)
		{
			if(EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

			sceneToOpen = sceneName;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			if (sceneToOpen == null ||
			    EditorApplication.isPlaying || EditorApplication.isPaused ||
			    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			EditorApplication.update -= OnUpdate;

			if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				// need to get scene via search because the path to the scene
				// file contains the package version so it'll change over time
				string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
				if (guids.Length == 0)
				{
					Debug.LogWarning("Couldn't find scene file");
				}
				else
				{
					string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
					EditorSceneManager.OpenScene(scenePath);
				}
			}
			sceneToOpen = null;
		}
	}
}