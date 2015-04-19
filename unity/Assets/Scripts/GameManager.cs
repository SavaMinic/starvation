using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{

	public static Random rand = new Random();

	#region gui

	[SerializeField]
	private Button cropsButton;
	[SerializeField]
	private Button bugsButton;
	[SerializeField]
	private Button rainButton;


	[SerializeField]
	private Text player1Name;
	[SerializeField]
	private Text player1Food;
	[SerializeField]
	private Text player1Bugs;
	[SerializeField]
	private Image player1Move;

	[SerializeField]
	private Text player2Name;
	[SerializeField]
	private Text player2Food;
	[SerializeField]
	private Text player2Bugs;
	[SerializeField]
	private Image player2Move;

	[SerializeField]
	private Text timerLabel;

	[SerializeField]
	public List<Sprite> sprites;

	#endregion

	public enum Move
	{
		Crops,
		Bugs,
		Rain,
		None
	};

	public enum GameState
	{
		Start,
		Selecting,
		Finish,
		End
	};

	private Player firstPlayer, secondPlayer;
	private DateTime timeToStart;
	private GameState gameState;

	public GameState CurrGameState
	{
		get { return gameState; }
		set
		{
			gameState = value;

			// timer label
			timerLabel.enabled = gameState == GameState.Selecting;

			// buttons
			cropsButton.enabled = gameState == GameState.Start || gameState == GameState.Selecting;
			bugsButton.enabled = gameState == GameState.Start || gameState == GameState.Selecting;
			rainButton.enabled = gameState == GameState.Start || gameState == GameState.Selecting;
		}
	}

	public void Start()
	{
		timerLabel.enabled = false;
		NewGame();
	}

	private void NewGame()
	{
		firstPlayer = new Player(player1Name, player1Food, player1Bugs, player1Move);
		secondPlayer = new Player(player2Name, player2Food, player2Bugs, player2Move);

		// initialize starting properties
		firstPlayer.Name = "You";
		secondPlayer.Name = "DR.DOOM";
		firstPlayer.Food = secondPlayer.Food = 2;
		firstPlayer.Bugs = secondPlayer.Bugs = 3;

		CurrGameState = GameState.Start;
	}

	public void StartTimer(float time = 5f)
	{
		timeToStart = DateTime.Now.AddSeconds(time);
		CurrGameState = GameState.Selecting;
		Debug.Log("time started");
	}

	public void Update()
	{
		// refresh timer time
		if (CurrGameState == GameState.Selecting)
		{
			timerLabel.text = (timeToStart - DateTime.Now).Seconds + "s";// check timer
			if (timeToStart <= DateTime.Now)
			{
				timerLabel.enabled = false;
				// check result
				CurrGameState = GameState.Finish;
				// show status
				// do animation
			}
		}
	}

	private void CalculateResult(Move move1, Move move2)
	{
		switch (move1)
		{
			case Move.Crops:
				switch (move2)
				{
					case Move.Crops: CoopAction(); break;
					case Move.Bugs: BugInvasionAction(firstPlayer, secondPlayer); break;
					case Move.Rain: HelpAction(firstPlayer, secondPlayer); break;
				}
				break;
			case Move.Bugs:
				switch (move2)
				{
					case Move.Crops: BugInvasionAction(secondPlayer, firstPlayer); break;
					case Move.Bugs: BugOverpopulationAction(); break;
					case Move.Rain: ExterminationAction(firstPlayer, secondPlayer); break;
				}
				break;
			case Move.Rain:
				switch (move2)
				{
					case Move.Crops: HelpAction(secondPlayer, firstPlayer); break;
					case Move.Bugs: ExterminationAction(secondPlayer, firstPlayer); break;
					case Move.Rain: FloodAction(); break;
				}
				break;
		}
	}

	private void CoopAction()
	{
		Debug.Log("Coop");
		firstPlayer.Food += 1;
		secondPlayer.Food += 1;
	}

	private void BugInvasionAction(Player cropPlayer, Player bugPlayer)
	{
		Debug.Log("bug invasion");
		cropPlayer.Food -= 2;
	}

	private void HelpAction(Player cropPlayer, Player rainPlayer)
	{
		Debug.Log("help");
		cropPlayer.Food += 2;
		rainPlayer.Food += 1;
	}

	private void ExterminationAction(Player bugPlayer, Player rainPlayer)
	{
		Debug.Log("extermination");
		bugPlayer.Bugs -= 1;
		rainPlayer.Food += 1;
	}

	private void BugOverpopulationAction()
	{
		Debug.Log("overpopulation");
		firstPlayer.Bugs -= 1;
		secondPlayer.Bugs -= 1;
	}

	private void FloodAction()
	{
		Debug.Log("flood");
		firstPlayer.Food -= 2;
		secondPlayer.Food -= 2;
	}

	#region input handlers

	public void OnCropsButtonPress()
	{
		firstPlayer.CurrentMove = Move.Crops;
	}

	public void OnBugsButtonPress()
	{
		firstPlayer.CurrentMove = Move.Bugs;
	}

	public void OnRainButtonPress()
	{
		firstPlayer.CurrentMove = Move.Rain;
	}

	#endregion
}
