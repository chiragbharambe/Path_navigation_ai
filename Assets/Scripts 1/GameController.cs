﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameController : MonoBehaviour {


	public GameObject car;
	public Rigidbody carRigid;
	public CarController carController;

	public double mutationRate = 0.3;
	public double randomBehaviour = 0.2;
	public double elitism = 0.2;


	public Text genomeText;
	public Text generationText;
	public Text bestFitnessText;
	public Text diversityText;
	public Text mutationRateText;
	public Text randomBehaviourText;

	private float distanceTraveled = 0;
	private int generationNumber = 1;

	private UnityAction listener;

	private NeuroEvolution neuro;
	private Genome currentGenome;
	private Generation currentGeneration;
	private List<Genome> currentGenomes = new List<Genome>();
	private Vector3 lastPosition;
	private int currentGenomeIndex = 0;
	private int bestFitness = 0;

	public CarSensors carSensors;

	public GenomeVisualization genomeVisualization;

	// Use this for initialization

	void Awake () {
		neuro = new NeuroEvolution (13, new int[]{8},2);

		NeuroEvolution.elitism = elitism;
		NeuroEvolution.mutationRate = mutationRate;
		NeuroEvolution.randomBehaviour = randomBehaviour;

		Generation nextGeneration = neuro.nextGeneration ();
		currentGeneration = nextGeneration;
		List<Genome> genomes = nextGeneration.getGenomes ();
		currentGenomes = genomes;
		currentGenome = genomes[currentGenomeIndex];

		genomeText.text = "Genome 1";
		generationText.text = "Generation 1";
		diversityText.text = "Diversity: " + currentGeneration.diversity;

		listener = new UnityAction (GameOver);


	}

	void OnEnable() {
		EventManager.StartListening ("gameover", listener);
	}
	
	// Update is called once per frame
	void Update () {
		distanceTraveled += Vector3.Distance (car.transform.position, lastPosition);
		lastPosition = car.transform.position;
	}

	private void FixedUpdate() {


		NeuroEvolution.elitism = elitism;
		NeuroEvolution.mutationRate = mutationRate;
		NeuroEvolution.randomBehaviour = randomBehaviour;

		mutationRateText.text = "Mutation Rate: " + NeuroEvolution.mutationRate;
		randomBehaviourText.text = "Random Behaviour: " + NeuroEvolution.randomBehaviour;

		double[] output = currentGenome.getNeuralNetwork ().compute (carSensors.getSensorOutput ());

		carController.Move ((float)(output[1] * 7 - 3.5), (float)output[0], (float)output[0], 0);
	}

	void GameOver() {
		carRigid.velocity = new Vector3(0,0,0);
		currentGenome.setScore ((int)distanceTraveled);

		if (distanceTraveled > bestFitness) {
			bestFitness = (int)distanceTraveled;
			bestFitnessText.text = "Best Fitness: " + bestFitness;
		}

		distanceTraveled = 0;
		currentGenomeIndex++;

		if (currentGenomeIndex == NeuroEvolution.population) {

			currentGeneration = neuro.nextGeneration ();
			currentGenomes = currentGeneration.getGenomes ();

			currentGenomeIndex = 0;

			generationNumber++;
		}

		currentGenome = currentGenomes[currentGenomeIndex];

		updateCanvasText ();

	}

	public Genome GetCurrentGenome(){
		return currentGenome;
	}

	private void updateCanvasText() {
		genomeText.text = "Genome " + (currentGenomeIndex + 1);
		generationText.text = "Generation " + generationNumber;
		diversityText.text = "Diversity: " + currentGeneration.diversity;
	}
}
