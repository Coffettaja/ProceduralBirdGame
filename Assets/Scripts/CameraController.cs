using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	public GameObject player;
	public GameObject level;
	public Canvas zoomedOutGUI;

	private float initialSize;
	private LevelGenerator levelGen;

	private GameObject gameControllerObject;

	private int newLevelWidth;
	private int newLevelHeight;

	//Whether or not the camera is focused on the player
	private bool cameraOnPlayer;

	void Start () 
	{
		levelGen = level.GetComponent<LevelGenerator> ();
		initialSize = Camera.main.orthographicSize;
		cameraOnPlayer = false;
		zoomedOutGUI.gameObject.SetActive (true);
		ToggleCameraPos (false);
	}
		
	void Update ()
	{
		//'Esc' toggles the camera view between the player and the level
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			ToggleCameraPos (!cameraOnPlayer);
			zoomedOutGUI.gameObject.SetActive (!zoomedOutGUI.gameObject.activeSelf);
		}

		//Camera can be moved up or down when in zoomed-out mode, using arrow keys
		if (!cameraOnPlayer && Input.GetKey (KeyCode.DownArrow))
		{
			transform.position = transform.position + Vector3.down * 10 * Time.deltaTime;
		}

		if (!cameraOnPlayer && Input.GetKey (KeyCode.UpArrow))
		{
			transform.position = transform.position + Vector3.up * 10 * Time.deltaTime;
		}
	}

	/// <summary>
	/// Toggles the camera position between player focused and level focused
	/// </summary>
	/// <param name="toPlayer">Wether the resulting state should focus on the player or not</param>
	public void ToggleCameraPos(bool toPlayer)
	{
		if (!toPlayer)
		{
			CenterCamera ();
		} else 
		{
			Camera.main.orthographicSize = initialSize;
			cameraOnPlayer = true;
		}
	}

	/// <summary>
	/// Positions the camera to the center and resizes it so that the whole width of the level is visible
	/// </summary>
	public void CenterCamera()
	{
		Camera.main.orthographicSize = levelGen.GetActualWidth () / (2 * Camera.main.aspect);

		//for some reason the camera doesn't position correctly to the middle of the level, so x-coordinate needs a minor fix...
		Vector3 zoomedOutPos = new Vector3 (levelGen.GetActualWidth () / 2 - 0.25f, 
			levelGen.GetActualHeight () - Camera.main.orthographicSize, -10);
		transform.position = zoomedOutPos;
		cameraOnPlayer = false;
	}

	void LateUpdate () 
	{
		//Camera follows the player
		if (cameraOnPlayer) 
		{
			transform.position = player.transform.position + new Vector3 (0, -0.5f, -10);
		}
	}
}
