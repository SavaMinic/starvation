using System;
using UnityEngine;
using UnityEngine.UI;

public class Player
{
	private int food;
	private int bugs;
	private string name;
	private GameManager.Move currentMove;

	#region gui
	private Text nameLabel;
	private Text foodLabel;
	private Text bugsLabel;
	private Image moveImage;

	public Image foodStatusImage;
	public Text foodStatus;
	public Image bugsStatusImage;
	public Text bugsStatus;
	#endregion

	private GameManager gameManager;

	public Player(Text nameLabel, Text foodLabel, Text bugsLabel, Image moveImage, Image foodStatusImage, Text foodStatus, Image bugsStatusImage, Text bugsStatus)
	{
		gameManager = GameObject.FindObjectOfType<GameManager>();
		this.nameLabel = nameLabel;
		this.foodLabel = foodLabel;
		this.bugsLabel = bugsLabel;
		this.moveImage = moveImage;
		this.foodStatusImage = foodStatusImage;
		this.foodStatus = foodStatus;
		this.bugsStatusImage = bugsStatusImage;
		this.bugsStatus = bugsStatus;
		CurrentMove = GameManager.Move.None;
	}

	public int Food
	{
		get { return food; }
		set
		{
			food = Mathf.Max(value, 0);
			foodLabel.text = food.ToString();
		}
	}

	public int Bugs
	{
		get { return bugs; }
		set
		{
			bugs = Mathf.Clamp(value, 0, 3);
			bugsLabel.text = bugs + "/3";
		}
	}

	public string Name
	{
		get { return name; }
		set
		{
			name = value;
			nameLabel.text = name;
		}
	}

	public GameManager.Move CurrentMove
	{
		get { return currentMove; }
		set
		{
			// if this is first selected move, start timer
			if (this == gameManager.firstPlayer && currentMove == GameManager.Move.None && value != GameManager.Move.None)
			{
				gameManager.StartTimer(2f);
			}
			currentMove = value;
			// refresh the image
			if (currentMove != GameManager.Move.None)
			{
				// for first player, and for oponnent if selecting is done
				if (this == gameManager.firstPlayer)
				{
					moveImage.sprite = gameManager.sprites[(int)currentMove];
				}
				else
				{
					// show the question mark
					moveImage.sprite = gameManager.sprites[gameManager.sprites.Count - 1];
				}
			}
			moveImage.enabled = currentMove != GameManager.Move.None;
		}
	}

	public GameManager.Move GetNextMoveEasy()
	{
		Array values = Enum.GetValues(typeof(GameManager.Move));
		// last is None, and check if we can use bugs sprite
		int len = bugs > 0 ? values.Length - 1 : values.Length - 2;
		return (GameManager.Move)values.GetValue(GameManager.rand.Next(len));
	}

	public void RefreshMoveImage()
	{
		moveImage.sprite = gameManager.sprites[(int)currentMove];
	}
}