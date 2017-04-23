using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public Canvas zoomedOutGUI;
	public GameObject level;

	private Button generateButton;
	public Slider widthSlider;
	public Slider heightSlider;

	private int newLevelWidth;
	private int newLevelHeight;

	private LevelGenerator levelGen;

	// Use this for initialization
	void Start () 
	{
		levelGen = level.GetComponent<LevelGenerator> ();
		generateButton = zoomedOutGUI.GetComponentInChildren<Button> ();
		generateButton.onClick.AddListener (OnGenerateClick);

		widthSlider.value = levelGen.GetLevelWidth ();
		heightSlider.value = levelGen.GetLevelHeight ();

		//Slider[] sliders = zoomedOutGUI.GetComponentsInChildren<Slider> ();
		//widthSlider = sliders [0];
		//widthSlider.OnDrag (UpdateText);
	}

	public void OnWidthSliderChanged(float newValue)
	{
		widthSlider.GetComponentInChildren<Text> ().text = "" + newValue;
		newLevelWidth = (int)widthSlider.value;
	}

	public void OnHeightSliderChanged(float newValue)
	{
		heightSlider.GetComponentInChildren<Text> ().text = "" + newValue;
		newLevelHeight = (int)heightSlider.value;
	}

	void UpdateText()
	{
		widthSlider.GetComponentInChildren<Text> ().text = "" + widthSlider.value;
	}

	/// <summary>
	/// Generates a new level with the values of the sliders
	/// </summary>
	void OnGenerateClick()
	{
		levelGen.GenerateLevel (newLevelWidth, newLevelHeight);
		Camera.main.GetComponent<CameraController> ().ToggleCameraPos (false);
		//Camera.main.GetComponent<CameraController> ().CenterCamera ();
	}
}
