using System;
using UnityEngine;



/**
 * 
 * DNA stores the Gene and other data for any GA Solution
 * Note: the <T> notation is for Generic methods - see https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/generic-methods 
 * It is similar to C++ Templates
 * In this Project, the data type for the DNA is <int> , as that is the data used within the Genes
 */
public class DNA<T>
{
    // the Gene data - for this project, will be an array of integers
    public T[] Genes { get; private set; }

    // the Fitness for this Solution
    public float Fitness { get; private set; }

    // Functions for the various methods
    private System.Random random;
    private Func<T> getRandomGene;
    private Func<int, float> fitnessFunction;


    public DNA(int size, System.Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction, bool shouldInitGenes = true)
    {
        Genes = new T[size];
        this.random = random;
        this.getRandomGene = getRandomGene;
        this.fitnessFunction = fitnessFunction;

        // if we need to create new Gene data, use the random funciton to pick random values
        if (shouldInitGenes)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = getRandomGene();
            }
        }
    }

    /**
     * CalculateFitness will call the stored Fitness Function and set the Fitness value with the value returned
     */
    public float CalculateFitness(int index)
    {

        Fitness = fitnessFunction(index);

        return Fitness;
    }



    /**
	 * Crossover function for the Genetic Algorihm
	 * Currently, this will select random data from each parent
	 */
    public DNA<T> Crossover(DNA<T> otherParent)
    {
        // create a new instance of DNA that will be the child's DNA
        DNA<T> child = new DNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, shouldInitGenes: false);



        // TASK TODO
        // Add code to perform a crossover between this parent's Genes and the second parents Genes
        // the results of the crossover should be stored in "DNA<T> child", which is created above and returned by the function


        // - the first parents genes array can be accessed via 'Genes' 
        // - the second parents genes array can be accessed via  otherParent.Genes
        // - access the child genes array by child.Genes
        
        // use 'random.Next()' or 'random.NextDouble()' for any random numbers used in the process
        // e.g. 'if(random.NextDouble() < 0.5 )'

        // there are different Crossover methods that can be used (e.g. Random, Single Point, Multi Point)
        //  see the Week 7 lecture for details
        


        // TEMP CODE - will simply make a child an exact match of one parent
        // Modify this code following instructions above
        for (int i = 0; i < Genes.Length; i++)
        {
            child.Genes[i] = Genes[i];
        }


        // return the child DNA
        return child;
    }


    /**
	 * Mutate will apply mutation at the rate given by mutationRate
	 * 
	 */
    public void Mutate(float mutationRate)
    {

        // TASK TODO
        // Add code below to perform mutation on this DNA's Genes
        //    stored in the 'Genes' class member
        // made random changes to the DNA to represent mutation
        // use the provided mutationRate to determine if this DNA should be mutated
        // use 'random.Next()' or 'random.NextDouble()' for any random numbers used in the process
        // getRandomGene() will return a random Gene value






    

    }
}
