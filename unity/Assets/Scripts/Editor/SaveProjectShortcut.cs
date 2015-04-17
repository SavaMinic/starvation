using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class SaveProjectShortcut
{
	private const float TimeBetweenSaves = 180f;

	private static float lastSaveTime = 0f;

	[MenuItem("File/Super Save Project %`")]
	public static void SaveProject()
	{
		if (Application.isPlaying) return;

		Debug.Log("Saving everything");
		EditorApplication.SaveScene();
		EditorApplication.SaveAssets();
	}

	static SaveProjectShortcut()
	{
		lastSaveTime = 0f;
		EditorApplication.update -= Update;
		EditorApplication.update += Update;
	}

	public static void Update()
	{
		if (Application.isPlaying) return;

		if (Time.realtimeSinceStartup > lastSaveTime + TimeBetweenSaves)
		{
			Debug.Log("Auto save");
			EditorApplication.SaveAssets();
			lastSaveTime = Time.realtimeSinceStartup;
		}
	}
}
