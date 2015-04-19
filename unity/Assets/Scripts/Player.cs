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
	#endregion

	private GameManager gameManager;

	public Player(Text nameLabel, Text foodLabel, Text bugsLabel, Image moveImage)
	{
		this.nameLabel = nameLabel;
		this.foodLabel = foodLabel;
		this.bugsLabel = bugsLabel;
		this.moveImage = moveImage;
		CurrentMove = GameManager.Move.None;
		gameManager = GameObject.FindObjectOfType<GameManager>();
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
			if (currentMove == GameManager.Move.None && value != GameManager.Move.None)
			{
				gameManager.StartTimer();
			}
			currentMove = value;

			// refresh the image
			if (currentMove != GameManager.Move.None)
			{
				moveImage.sprite = gameManager.sprites[(int) currentMove];
			}
			moveImage.enabled = currentMove != GameManager.Move.None;
		}
	}
}