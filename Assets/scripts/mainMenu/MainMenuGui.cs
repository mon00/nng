using UnityEngine;
using System.Collections;

public class MainMenuGui : MonoBehaviour {

	public bool staticContainer = false;
	public int containerX = 0;
	public int containerY = 0;

	public int containerWidth = 200;
	public int containerHeight = 600;

	void OnGUI()
	{
		if (!staticContainer) {
						containerX = (Screen.width - containerWidth) / 2;
						containerY = (Screen.height - containerHeight) / 2;
				}

		GUI.Box(new Rect(containerX,containerY,containerWidth,containerHeight), "Loader Menu");

		if (GUI.Button (new Rect (20, 40, 80, 20), "Load test scene")) {
						Application.LoadLevel (1);
				}
		if (GUI.Button (new Rect (20, 90, 80, 20), "Exit prog")) {
						Application.Quit ();
				}
	}
}
