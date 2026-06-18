using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public static GeneticAlgorithm Instance;

    [Header("AG Config")]
    public int populationSize = 50;
    public float mutationRate = 0.05f;
    public float mutationStrength = 0.5f;

    [HideInInspector] public int generation = 0;
    [HideInInspector] public float bestFitness = 0f;
    [HideInInspector] public int alive = 0;

    List<NeuralNetwork> population = new List<NeuralNetwork>();
    int currentIndex = 0;

    void Awake()
    {
        Instance = this;
        InitPopulation();
    }

    void InitPopulation()
    {
        population.Clear();
        for (int i = 0; i < populationSize; i++)
            population.Add(new NeuralNetwork(new int[] { 3, 6, 1 }));
        currentIndex = 0;
        generation++;
    }

    public NeuralNetwork GetNext()
    {
        if (currentIndex < population.Count)
            return population[currentIndex++];
        return null;
    }

    public void BirdDied(NeuralNetwork nn, float fitness)
    {
        nn.fitness = fitness;
        alive--;
        if (alive <= 0)
            Evolve();
    }

    void Evolve()
    {
        var sorted = population.OrderByDescending(n => n.fitness).ToList();
        bestFitness = sorted[0].fitness;

        List<NeuralNetwork> newPop = new List<NeuralNetwork>();

        int eliteCount = Mathf.Max(1, populationSize / 10);
        for (int i = 0; i < eliteCount; i++)
            newPop.Add(new NeuralNetwork(sorted[i]));

        while (newPop.Count < populationSize)
        {
            NeuralNetwork parent1 = sorted[Random.Range(0, eliteCount)];
            NeuralNetwork parent2 = sorted[Random.Range(0, eliteCount)];
            NeuralNetwork child = Crossover(parent1, parent2);
            child.Mutate(mutationRate, mutationStrength);
            newPop.Add(child);
        }

        population = newPop;
        currentIndex = 0;
        generation++;

        GameManager.Instance.StartNewGeneration();
    }

    NeuralNetwork Crossover(NeuralNetwork a, NeuralNetwork b)
    {
        NeuralNetwork child = new NeuralNetwork(a);
        for (int i = 0; i < child.weights.Length; i++)
            for (int j = 0; j < child.weights[i].Length; j++)
                for (int k = 0; k < child.weights[i][j].Length; k++)
                    if (Random.value < 0.5f)
                        child.weights[i][j][k] = b.weights[i][j][k];
        return child;
    }
}
