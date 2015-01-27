using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{

 	public bool isOptions = false;
	public bool isGame = false;

	GameObject optionsArea;
	GameObject gameArea;

	void Start ()
	{
		if(isOptions && isGame)
			isOptions = false;
		gameArea = GameObject.Find ("Canvas/GameArea");
		gameArea.SetActive(isGame);
		optionsArea = GameObject.Find("/Canvas/OptionsArea");
		optionsArea.SetActive (isOptions);
	}


	public void GameSH()
	{
		if(isGame)
		{
			isGame = false;
			gameArea.SetActive (false);
		}
		else
		{
			if(isOptions)
				this.OptionsSH();
			isGame = true;
			gameArea.SetActive(true);
		}
	}

	public void OptionsSH ()
	{
		if(isOptions)
		{
			optionsArea.SetActive(false);
			isOptions = false;
		}
		else
		{
			if(isGame)
				this.GameSH();
			optionsArea.SetActive(true);
			isOptions = true;		
		}
	}

	public void AppQuit()
	{
		Application.Quit ();
	}
}

