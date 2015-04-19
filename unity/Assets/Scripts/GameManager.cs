using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{

	public static Random rand = new Random();

	private const int TargetFood = 7;

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
	private Text statusLabel;
	[SerializeField]
	private Text turnLabel;

	[SerializeField]
	public List<Sprite> sprites;

	[SerializeField]
	private Image player1FoodStatusImage;
	[SerializeField]
	private Text player1FoodStatus;
	[SerializeField]
	private Image player1BugsStatusImage;
	[SerializeField]
	private Text player1BugsStatus;


	[SerializeField]
	private Image player2FoodStatusImage;
	[SerializeField]
	private Text player2FoodStatus;
	[SerializeField]
	private Image player2BugsStatusImage;
	[SerializeField]
	private Text player2BugsStatus;

	[SerializeField]
	private Text finishStatusLabel;

	[SerializeField]
	private Image turnBugsImage;

	#endregion

	public enum Move
	{
		Crops,
		Rain,
		Bugs,
		None
	};

	public enum GameState
	{
		Start,
		Selecting,
		Finish,
		End
	};

	public Player firstPlayer, secondPlayer;
	private DateTime timeToStart, timeToEnd;
	private GameState gameState;
	private int turn;

	public GameState CurrGameState
	{
		get { return gameState; }
		set
		{
			gameState = value;

			// labels
			timerLabel.enabled = gameState == GameState.Selecting;
			statusLabel.enabled = gameState == GameState.Start || gameState == GameState.Finish;

			// buttons
			cropsButton.enabled = gameState == GameState.Start || gameState == GameState.Selecting || gameState == GameState.Finish;
			bugsButton.enabled = firstPlayer.Bugs > 0 && (gameState == GameState.Start || gameState == GameState.Selecting || gameState == GameState.Finish);
			rainButton.enabled = gameState == GameState.Start || gameState == GameState.Selecting || gameState == GameState.Finish;

			if (gameState == GameState.Start)
			{
				firstPlayer.CurrentMove = Move.None;
				secondPlayer.CurrentMove = Move.None;
				statusLabel.text = "Select your move";
				timerLabel.enabled = false;
			}
		}
	}

	public int Turn
	{
		get { return turn; }
		set
		{
			turn = value;
			turnLabel.text = "Turn " + turn;
		}
	}

	public void Start()
	{
		NewGame();
	}

	private void NewGame()
	{
		Turn = 0;

		firstPlayer = new Player(player1Name, player1Food, player1Bugs, player1Move, player1FoodStatusImage, player1FoodStatus, player1BugsStatusImage, player1BugsStatus);
		secondPlayer = new Player(player2Name, player2Food, player2Bugs, player2Move, player2FoodStatusImage, player2FoodStatus, player2BugsStatusImage, player2BugsStatus);

		// initialize starting properties
		firstPlayer.Name = "You";
		secondPlayer.Name = "DR.DOOM";
		firstPlayer.Food = secondPlayer.Food = 2;
		firstPlayer.Bugs = secondPlayer.Bugs = 3;

		CurrGameState = GameState.Start;
		finishStatusLabel.enabled = false;
		timerLabel.enabled = false;
		turnBugsImage.enabled = false;
		HideMovesStatus();
	}

	public void StartTimer(float time = 5f)
	{
		timeToStart = DateTime.Now.AddSeconds(time);
		CurrGameState = GameState.Selecting;
		Turn++;
		if (Turn % 5 == 0)
		{
			firstPlayer.Bugs++;
			secondPlayer.Bugs++;
			turnBugsImage.enabled = true;
		}
		else
		{
			turnBugsImage.enabled = false;
		}
		// choose move for opponent
		secondPlayer.CurrentMove = secondPlayer.GetNextMoveEasy();
	}

	public void EndTimer(float time = 3f)
	{
		timeToEnd = DateTime.Now.AddSeconds(time);
		CurrGameState = GameState.Finish;
	}

	private void HideMovesStatus()
	{
		player1FoodStatusImage.enabled = player1FoodStatus.enabled = player1BugsStatus.enabled = player1BugsStatusImage.enabled = false;
		player2FoodStatusImage.enabled = player2FoodStatus.enabled = player2BugsStatus.enabled = player2BugsStatusImage.enabled = false;
	}

	public void Update()
	{
		// refresh timer time
		if (CurrGameState == GameState.Selecting)
		{
			timerLabel.text =((timeToStart - DateTime.Now).TotalMilliseconds / 1000).ToString("0.0") + "s";
			if (timeToStart <= DateTime.Now)
			{
				// check result
				HideMovesStatus();
				// if nothing is selected, select crops
				if (firstPlayer.CurrentMove == Move.None) firstPlayer.CurrentMove = Move.Crops;

				string actionResult = CalculateResult(firstPlayer.CurrentMove, secondPlayer.CurrentMove);
				statusLabel.text = actionResult;

				// timer is up, so show the opponent move
				secondPlayer.RefreshMoveImage();

				// if there are no bugs and bug is selected, deselect them
				if (firstPlayer.Bugs == 0 && firstPlayer.CurrentMove == Move.Bugs)
				{
					firstPlayer.CurrentMove = Move.None;
				}

				if (firstPlayer.Food >= TargetFood || secondPlayer.Food >= TargetFood)
				{
					finishStatusLabel.enabled = true;
					finishStatusLabel.text = firstPlayer.Food >= TargetFood ? (secondPlayer.Food >= TargetFood ? "It's a tie!" : "You have won!") : "You have lost!";
					finishStatusLabel.color = firstPlayer.Food >= TargetFood ? Color.black : Color.red;
					CurrGameState = GameState.End;
					return;
				}

				// fire up the timer for new turn
				EndTimer();
			}
		} 
		else if (CurrGameState == GameState.Finish)
		{
			timerLabel.enabled = false;
			timerLabel.text = "Start in " + ((timeToEnd - DateTime.Now).TotalMilliseconds / 1000).ToString("0.00") + "s";
			if (timeToEnd <= DateTime.Now)
			{
				StartTimer();
				statusLabel.text = "Select your move";
				HideMovesStatus();
			}
		}
	}

	private string CalculateResult(Move move1, Move move2)
	{
		switch (move1)
		{
			case Move.Crops:
				switch (move2)
				{
					case Move.Crops: return CoopAction();
					case Move.Bugs: return BugInvasionAction(firstPlayer, secondPlayer);
					case Move.Rain: return HelpAction(firstPlayer, secondPlayer);
				}
				break;
			case Move.Bugs:
				switch (move2)
				{
					case Move.Crops: return BugInvasionAction(secondPlayer, firstPlayer);
					case Move.Bugs: return BugOverpopulationAction();
					case Move.Rain: return ExterminationAction(firstPlayer, secondPlayer);
				}
				break;
			case Move.Rain:
				switch (move2)
				{
					case Move.Crops: return HelpAction(secondPlayer, firstPlayer);
					case Move.Bugs: return ExterminationAction(secondPlayer, firstPlayer);
					case Move.Rain: return FloodAction();
				}
				break;
		}
		return "";
	}

	private string CoopAction()
	{
		firstPlayer.Food += 1;
		firstPlayer.foodStatus.enabled = firstPlayer.foodStatusImage.enabled = true;
		firstPlayer.foodStatus.text = "+1";

		secondPlayer.Food += 1;
		secondPlayer.foodStatus.enabled = secondPlayer.foodStatusImage.enabled = true;
		secondPlayer.foodStatus.text = "+1";

		return "We are friends!";
	}

	private string BugInvasionAction(Player cropPlayer, Player bugPlayer)
	{
		cropPlayer.Food -= 2;
		cropPlayer.foodStatus.enabled = cropPlayer.foodStatusImage.enabled = true;
		cropPlayer.foodStatus.text = "-2";

		return cropPlayer == firstPlayer ? "Your crops are attacked!" : "Opponent crops are attacked!";
	}

	private string HelpAction(Player cropPlayer, Player rainPlayer)
	{
		cropPlayer.Food += 2;
		cropPlayer.foodStatus.enabled = cropPlayer.foodStatusImage.enabled = true;
		cropPlayer.foodStatus.text = "+2";

		rainPlayer.Food += 1;
		rainPlayer.foodStatus.enabled = rainPlayer.foodStatusImage.enabled = true;
		rainPlayer.foodStatus.text = "+1";

		return cropPlayer == firstPlayer ? "You grow a lot!" : "You helped opponent!";
	}

	private string ExterminationAction(Player bugPlayer, Player rainPlayer)
	{
		bugPlayer.Bugs -= 1;
		bugPlayer.bugsStatus.enabled = bugPlayer.bugsStatusImage.enabled = true;
		bugPlayer.bugsStatus.text = "-1";

		rainPlayer.Food += 1;
		rainPlayer.foodStatus.enabled = rainPlayer.foodStatusImage.enabled = true;
		rainPlayer.foodStatus.text = "+1";

		return bugPlayer == firstPlayer ? "Oops, our bugs died!" : "You squashed opponent bugs!";
	}

	private string BugOverpopulationAction()
	{
		firstPlayer.Bugs -= 1;
		firstPlayer.bugsStatus.enabled = firstPlayer.bugsStatusImage.enabled = true;
		firstPlayer.bugsStatus.text = "-1";

		secondPlayer.Bugs -= 1;
		secondPlayer.bugsStatus.enabled = secondPlayer.bugsStatusImage.enabled = true;
		secondPlayer.bugsStatus.text = "-1";

		return "Bug overpopulation!";
	}

	private string FloodAction()
	{
		firstPlayer.Food -= 2;
		firstPlayer.foodStatus.enabled = firstPlayer.foodStatusImage.enabled = true;
		firstPlayer.foodStatus.text = "-2";

		secondPlayer.Food -= 2;
		secondPlayer.foodStatus.enabled = secondPlayer.foodStatusImage.enabled = true;
		secondPlayer.foodStatus.text = "-2";

		return "Flood!";
	}

	#region input handlers

	public void OnCropsButtonPress()
	{
		if (CurrGameState == GameState.Start || CurrGameState == GameState.Selecting)
		{
			firstPlayer.CurrentMove = Move.Crops;
		}
	}

	public void OnBugsButtonPress()
	{
		if (firstPlayer.Bugs > 0 && (CurrGameState == GameState.Start || CurrGameState == GameState.Selecting))
		{
			firstPlayer.CurrentMove = Move.Bugs;
			
		}
	}

	public void OnRainButtonPress()
	{
		if (CurrGameState == GameState.Start || CurrGameState == GameState.Selecting)
		{
			firstPlayer.CurrentMove = Move.Rain;
		}
	}

	public void OnNewGameClick()
	{
		NewGame();
	}

	#endregion
}
