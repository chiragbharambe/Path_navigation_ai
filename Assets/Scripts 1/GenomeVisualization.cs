﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GenomeVisualization : MonoBehaviour {

	public GameController gameController;
	private int startingX = 50;
	private int startingY = 370;
	List<List<Image>> paintedNodes = new List<List<Image>> ();
	List<List<List<Image>>> paintedWeights = new List<List<List<Image>>> ();
	Sprite nodeSprite;
	Sprite lineSprite;

	// Use this for initialization
	void Start () {
		nodeSprite = Resources.Load ("Assets/Resources/circle2.png", typeof(Sprite)) as Sprite;
		lineSprite = Resources.Load ("Assets/Resources/line.png", typeof(Sprite)) as Sprite;
		SetUpNodes ();
	}

	private void SetUpNodes() {
		Genome genome = gameController.GetCurrentGenome ();
		int[] neuronsPerLayer = genome.getNeuralNetwork ().getNumberOfNeuronsPerLayer ();

		for (int i = 0; i < neuronsPerLayer.Length; i++) {
			paintedNodes.Add (new List<Image> ());

			if (i > 0) {
				paintedWeights.Add (new List<List<Image>> ());
			}

			int yMultiplier = 0;


			for (int j = 0; j < neuronsPerLayer [i]; j++) {


				if (i > 0 && neuronsPerLayer [i] < neuronsPerLayer [i - 1]) {
					yMultiplier = (int)Mathf.Floor ((float)((neuronsPerLayer [i - 1] - neuronsPerLayer [i]) / 2));
				}

				PaintNode (startingX + (150 * i), startingY - (30 * j) - (yMultiplier * 30), i);

				if (i > 0) {
					List<Image> newLines = PaintLines (startingX + (150 * i), startingY - (30 * j) - (yMultiplier * 30), i);
					paintedWeights[i - 1].Add (newLines);
				}

			}

		
		}
	}

	private List<Image> PaintLines(int x, int y, int layerIndex) {

		List<Image> previousLayerNodes = paintedNodes [layerIndex - 1];

		List<Image> paintedWeights = new List<Image>();

		previousLayerNodes.ForEach ((Image image) => {

			GameObject NewObj = new GameObject ();
			Image NewImage = NewObj.AddComponent<Image>();
			NewImage.sprite = lineSprite;

			Color color = Color.white;
			NewImage.color = color;

			RectTransform rect = NewObj.GetComponent<RectTransform> ();
			rect.SetParent (gameObject.transform);

			rect.anchorMin = new Vector2(0,0);
			rect.anchorMax = new Vector2(0,0);
			rect.pivot = new Vector2(0,0);

			NewObj.SetActive (true);

			RectTransform prevRect = image.GetComponentInParent<RectTransform> ();

			Vector3 pos = new Vector3 (x, y, 0);

			float angle = Vector3.Angle(pos, prevRect.transform.position);

			Vector3 targetDir = pos - prevRect.transform.position;
			float angle2 = Vector3.Angle(targetDir, Vector3.down);

			rect.sizeDelta = new Vector2 (6, (int)targetDir.magnitude);

			rect.SetPositionAndRotation (pos, Quaternion.AngleAxis(angle2, Vector3.forward));

			paintedWeights.Add(NewImage);
		});

		return paintedWeights;

	}

	private void PaintNode(int x, int y, int layerIndex) {
		GameObject NewObj = new GameObject ();
		Image NewImage = NewObj.AddComponent<Image>();
		NewImage.sprite = nodeSprite;

		Color color = Color.green;
		NewImage.color = color;

		RectTransform rect = NewObj.GetComponent<RectTransform> ();
		rect.SetParent (gameObject.transform);

		rect.SetPositionAndRotation (new Vector3 (x, y, 0), new Quaternion (0, 0, 0, 0));
		rect.sizeDelta = new Vector2 (25, 25);

		paintedNodes[layerIndex].Add (NewImage);

		NewObj.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
		Genome genome = gameController.GetCurrentGenome ();

		int[] neuronsPerLayer = genome.getNeuralNetwork ().getNumberOfNeuronsPerLayer ();

		for (int layerIndex = 0; layerIndex < neuronsPerLayer.Length; layerIndex++) {
			for (int neuronIndex = 0; neuronIndex < neuronsPerLayer [layerIndex]; neuronIndex++) {
				Neuron neuron = genome.getNeuralNetwork ().GetNeuronInLayer (layerIndex, neuronIndex);
				UpdateNodeValue (layerIndex, neuronIndex, neuron.getValue());

				if (layerIndex > 0) {
					 UpdateWeightsValue (layerIndex - 1, neuronIndex, neuron.getWeights());
				}

			}
		}
	}

	private void UpdateNodeValue(int layerIndex, int neuronIndex, double value) {
		Image neuronImage = paintedNodes [layerIndex][neuronIndex];

		Color color = neuronImage.color;

		if (layerIndex == 0) {
			value = value + 0.2;
		}

		if (value > 0.5) {
			color = Color.red;
		} else {
			color = Color.yellow;
		}

		color.a = value > 0 ? (float)value : 0.05f;
		neuronImage.color = color;
	}

	private void UpdateWeightsValue(int layerIndex, int neuronIndex, double[] weights) {

		int weightIndex = 0;

		paintedWeights[layerIndex][neuronIndex].ForEach((Image image) => {
			double weight = weights[weightIndex];

			Color color = image.color;

			if (weight > 0.5) {
				color = Color.green;
				color.a = (float)weight;
			} else if (weight > 0) {
				color = Color.white;
				color.a = weight > 0.2 ? (float)weight : 0.2f;
			} else {
				color = Color.red;
				color.a = -weight > 0.2 ? (float)-weight : 0.2f;
			}



			image.color = color;
		});

	}
}
