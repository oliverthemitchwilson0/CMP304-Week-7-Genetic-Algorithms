//using System.Numerics;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.HID;



/**
 * GameController will manage the game loop and asteroid waves, as well we call the GA algorithm steps when needed
 * 
 */
public class GameController : MonoBehaviour
{

    // define the game state variables
    public enum ImpactDirectionCategory { N, NE, NW};
    public enum ImpactDistanceCategory {Near, Medium, Far };
    public enum ImpactSpeedCategory { Fast, Medium, Slow};
    public enum ImpactSizeCategory { Small, Medium, Large };

    // create string lookups for output
    private string[] dirCatStr =  { "N", "NE" , "NW" };
    private string[] distCatStr = { "Near", "Medium", "Far" };
    private string[] speedCatStr = { "Fast", "Medium", "Slow"};
    private string[] sizeCatStr = { "Small", "Medium", "Large" };

    // define the bounds for the ships and asteroids
    //  - the asteroid bounds are larger than the ship to esnure they do not spawn on top of ships
    public float xBound = 9;
    public float yBound = 5;

    public float xAsteroidBound = 9;
    public float yAsteroidBound = 8;

    // GA Parameters
    public float mutationRate = 0.01f;
    public int populationSize = 100;
    private int maxGenerations = 30;

    // Simulation Parameters
    public float simTimeScale = 1f; 


    // Wave config
    private int asteroidWaveTotal = 30; // number of asteroids per wave
    private int asteroidPerWave = 1; // not currently implemented

    // Wave tracking
    private int asteroidWaveCounter = 0;
    Asteroid currentAsteroid;
    WaveData waveData = new WaveData();

    // Prefabs to be used when spawning ships and astroids
    public GameObject asteroidPrefab;
    public GameObject shipPrefab;

    public GameObject shipSpawnPoint;

   
    // status tracking
    bool gameRunning = false;


    // UI & Reporting
    float averageCollisionCount;
    float averageDamageCount;
    public TMP_Text impactAngleText;
    public TMP_Text impactSpeedText;
    public TMP_Text impactDistanceText;
    public TMP_Text impactSizeText;

    public TMP_Text impactAngleCatText;
    public TMP_Text impactSpeedCatText;
    public TMP_Text impactDistanceCatText;
    public TMP_Text impactSizeCatText;

    public TMP_Text averageCollisionText;
    public TMP_Text averageDamageText;
    public TMP_Text currentAsteroidText;
    public TMP_Text currentGenerationText;

    private int shipDataView = 0;


    // Game State Tracking


    private float impactAngleData;
    private float impactSpeedData;
    private float impactDistanceData;
    private float impactSizeData;


    private ImpactDirectionCategory impactDirectionCategoryData;
    private ImpactDistanceCategory impactDistanceCategoryData;
    private ImpactSpeedCategory impactSpeedCategoryData;
    private ImpactSizeCategory impactSizeCategoryData;



    // AI & Tracking
    private GeneticAglorithm<int> ga;       // Genetic Aglorithm with each Gene being an int
    private int curGeneration = 0;

    private List<Ship> allShips = new List<Ship>();

    private int numStates;
    private int dnaLength;
    private Boolean simulationInProgress;

    // Random number
    System.Random random;



 
    void Start()
    {
        // cacluate the nubmer of states needed based on enums
        int v1 = Enum.GetNames(typeof(ImpactDirectionCategory)).Length;
        int v2 = Enum.GetNames(typeof(ImpactDistanceCategory)).Length;
        int v3 = Enum.GetNames(typeof(ImpactSpeedCategory)).Length;
        int v4 = Enum.GetNames(typeof(ImpactSizeCategory)).Length;

        // calculate the number of states to ensure the DNA covers each
        numStates = v1 * v2 * v3 * v4;
        dnaLength = numStates;

        // Init the Random class
        random = new System.Random();

        // Create genetic algorithm class
        ga = new GeneticAglorithm<int>(populationSize, dnaLength, random, GetRandomGene, FitnessFunction, mutationRate: mutationRate);

        // set the time scale
        Time.timeScale = simTimeScale;



    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            DisplayUI();

            simulationInProgress = !IsLevelComplete();

            if (simulationInProgress)
            {
                HandleInput();

                UpdateShips();         
                MoveShips();

                ProcessAsteroids();


            }
            else
            {
                // simualtion is over, proceeed to the next geneation


                float colTotal = 0;
                float damageTotal = 0;
                for (int i = 0; i < allShips.Count; i++)
                {
                    colTotal += allShips[i].GetCollisions();
                    damageTotal += allShips[i].GetDamage();
                }
             
                averageCollisionCount = colTotal / allShips.Count;
                Debug.Log(" Generation"+ curGeneration +" Av Collison is " + averageCollisionCount);
                averageDamageCount = damageTotal / allShips.Count;
               // Debug.Log(" Generation" + curGeneration + " Av Damage is " + averageDamageCount);


                // trigger the next generation
                ga.NewGeneration();

                curGeneration++;

                // if reached the generation limit, end GA process
                if (curGeneration >= maxGenerations)
                {
                    StopGame();
                    ReportEndResults();
            
                }
                else
                {

                    // create a new batch of ships using the new DNA
                    CreateShipGeneration();
                    asteroidWaveCounter = 0;
                    simulationInProgress = true;
                }
            }

        }

    
    }


    /**
     * StartGame is called when the user preses the 'Start' button, and will begin the GA process
     */
    public void StartGame()
    {
        // crete a new wave data
        waveData.CreateWave(asteroidWaveTotal, xAsteroidBound, yAsteroidBound);
        CreateShips();
        asteroidWaveCounter = 0;

        //start the level
        NewAsteroidWave();   
        gameRunning = true;
        simulationInProgress = true;



    }



    void StopGame()
    {

        gameRunning = false;
        simulationInProgress = false;


    }




    /*
     * ProcessAsteroids will check the Asteroid status each frame to see if a new one needs to be created
     */
    void ProcessAsteroids()
    {

        if(currentAsteroid.IsDesinationReached())
        {
            GameObject.Destroy(currentAsteroid.gameObject);
            NewAsteroidWave();
        }
    }


    /**
     * NewAsteroidWave will create a new asteroids wave
     * - - currently only 1 asteroid per wave
     */
    void NewAsteroidWave()
    {
        for(int i=0;i<asteroidPerWave;i++) 
            SpawnAsteroid();

        asteroidWaveCounter++;
    }

    void ReportEndResults()
    {
        Debug.Log(" --- END RESULTS --- ");

        Debug.Log("Best solution has fitness " + ga.BestFitness);
    }





    /**
     * IsLevelComplete will return true if al the asteroids for the level have been sent, otherise false
     */
    bool IsLevelComplete()
    {
        if (asteroidWaveCounter >= asteroidWaveTotal)
            return true;

        return false;
    }


    void CreateShipGeneration()
    {
        RemoveOldShips();
        CreateShips();
    }

    void RemoveOldShips()
    {
        for (int i = allShips.Count-1; i >=0; i--)
        {
            GameObject.Destroy(allShips[i].gameObject);
        }

        allShips.Clear();

    }

    /**
     * CreateShips will spawn the ships within the Scene 
     */
    void CreateShips()
    {

        for(int i=0;i< populationSize; i++) 
        {
            // create the ship based on the prefab  and set the DNA based on the ga data
            GameObject go = GameObject.Instantiate(shipPrefab, shipSpawnPoint.transform.position, Quaternion.identity);
            Ship newShip = go.GetComponent<Ship>();
            newShip.Init(numStates, xBound, yBound);
            newShip.SetGene(ga.Population[i]);
  
            allShips.Add(newShip);
        }

    }

  
    /**
     * MoveShips should be called once per frame to update the ship's position in the game world
     */
    void MoveShips()
    {

        foreach(Ship s in allShips)
        {
            s.ApplyMovement();
        }
   
    }



    /**
     * SpawnAsteroid will create a new astroid based on the WaveData
     */
    void SpawnAsteroid()
    {

        if(asteroidWaveCounter >= waveData.spawnPoint.Count)
        {
            Debug.Log("WARNING: No spawn data for asteroid " + asteroidWaveCounter);
            return;
        }

        // use the counter to retrieve the Asteroid initialisation data
        Vector2 _spawn = waveData.spawnPoint[asteroidWaveCounter];
        Vector2 _dest = waveData.destPoint[asteroidWaveCounter];
        float _speed = waveData.speed[asteroidWaveCounter];
        int _size = waveData.size[asteroidWaveCounter];
        GameObject go = GameObject.Instantiate(asteroidPrefab, new Vector3(_spawn.x, _spawn.y, 0), Quaternion.identity);
        Asteroid a = go.GetComponent<Asteroid>();
        a.Init(new Vector2(_dest.x, _dest.y), _speed, _size);
        a.StartMovement();


        currentAsteroid = a;



    }

    /**
     * UpdateShips will update the game stata for each ship. Should be called once per frame. 
     * 
     */
    void UpdateShips()
    {


        if(currentAsteroid==null)
            return;

        int _shipTracker = 0;

        foreach(Ship ship in allShips)
        {

            // get closest asteroid (as only one per wave will always be current, but could be expanded for multiple per wave)
            Asteroid _a = GetClosestAsteroidToPoint(new Vector2(ship.transform.position.x, ship.transform.position.y));


            // calc angle from ship to asteroid

            Vector2 _dir = new Vector2(ship.transform.position.x, ship.transform.position.y) - new Vector2(_a.transform.position.x, _a.transform.position.y);
            float _impactAngle =   Mathf.Atan2(_dir.x, _dir.y) * Mathf.Rad2Deg;

            // get speed of asteroid
            float _impactSpeed = _a.GetSpeed();

            // get distance of asteroid
            float _impactDistance = Mathf.Sqrt(((_a.transform.position.x - ship.transform.position.x) * (_a.transform.position.x - ship.transform.position.x)) + ((_a.transform.position.y - ship.transform.position.y) * (_a.transform.position.y - ship.transform.position.y)));

            // convert the real numbers to discrete IDs
            ImpactDistanceCategory _impactDistanceCategory = ConvertDistanceToCategory(_impactDistance);
            ImpactDirectionCategory _impactDirectionCategory = ConvertDirectionToCategory(_impactAngle);
            ImpactSpeedCategory _impactSpeedCategory = ConvertSpeedToCategory(_impactSpeed);
            ImpactSizeCategory _impactSizeCategory = ConvertSizeToCategory(_a.GetAsteroidSize());

            // set current game state for the ship
            ship.SetGameState(_impactDirectionCategory, _impactDistanceCategory, _impactSpeedCategory, _impactSizeCategory);

            // apply the Aciton for the Game State
            ship.SelectActionForGameState();

            // if this is the ship we are viewing data for, update the trackers for the UI
            if(_shipTracker==shipDataView)
            {
                impactAngleData = _impactAngle;
                impactSpeedData = _impactSpeed;
                impactDistanceData = _impactDistance;
                impactSizeData = _a.GetAsteroidSize();

                impactDistanceCategoryData = _impactDistanceCategory;
                impactDirectionCategoryData = _impactDirectionCategory;
                impactSpeedCategoryData = _impactSpeedCategory;
                impactSizeCategoryData = _impactSizeCategory;
            }

            _shipTracker++;

            
        }



    }


    Asteroid GetClosestAsteroidToPoint(Vector2 point)
    {
        
        return currentAsteroid;
    }

    private int GetRandomGene()
    {
        // return a random mumber - it will be applied to the gene range later
        return UnityEngine.Random.Range(0, 1000);
    }



    /**
     * FitnessFunction should return the fitness value for the member of the population at the provided index
     * e.g. should calcuate the fitness of ONE ship, not all ships
     */
    private float FitnessFunction(int _index)
    {

        //TASK TODO - replace below temporary code so that the function will return the number of collisions the ship given by _index had made

        // each ship is a member of the population, represented by the Ship class
        // the class variable 'allShips' is a List<Ship> , which is the equivelent of a C++ std::vector
        // you can acess a member of the list using [ ] 

        // using the provided _index, you should return the nubmer of collision that ship made in the Simulation

        // you just need to return the fitness of ONE ship (given by _index) not all ships

        float score = 1;

        return score;
    }







    void HandleInput()
    {


    }


    void DisplayUI()
    {
        impactAngleText.text = "Impact Angle: " + impactAngleData;
        impactSpeedText.text = "Impact Speed: " + impactSpeedData;
        impactDistanceText.text = "Impact Distance: " + impactDistanceData;
        impactSizeText.text = "Impact Size: " + impactSizeData;

        impactAngleCatText.text = "Angle Category: " + GetDirectionCategoryString();
        impactSpeedCatText.text = "Speed Category: " + GetSpeedCategoryString();
        impactDistanceCatText.text = "Distance Category: " + GetDistanceCategoryString();
        impactSizeCatText.text = "Size Category: " + GetSizeCategoryString();

        averageCollisionText.text = "Average Collision: " + averageCollisionCount;
        averageDamageText.text = "Average Damage: " + averageDamageCount;
        currentAsteroidText.text = "Current Asteroid: " + asteroidWaveCounter + "/" + asteroidWaveTotal;
        currentGenerationText.text = "Current Generation: " + curGeneration;

    }







    /**
     * Conversion funcitons - will map real numbers to discrete IDs
     */

    ImpactSpeedCategory ConvertSpeedToCategory(float speed)
    {
        if (speed ==4)
            return ImpactSpeedCategory.Fast;
        else if (speed ==3)
            return ImpactSpeedCategory.Medium;
        else 
            return ImpactSpeedCategory.Slow;

    }


    ImpactSizeCategory ConvertSizeToCategory(int size)
    {
        if (size == 2)
            return ImpactSizeCategory.Large;
        else if (size ==1)
            return ImpactSizeCategory.Medium;
        else
            return ImpactSizeCategory.Small;

    }


    ImpactDistanceCategory ConvertDistanceToCategory(float distance)
    {
        if (distance > 9)
            return ImpactDistanceCategory.Far;
        else if (distance > 6)
            return ImpactDistanceCategory.Medium;
        else
            return ImpactDistanceCategory.Near;

    }

    ImpactDirectionCategory ConvertDirectionToCategory(float angle)
    {


     
        if (angle <135 && angle > 0)
            return ImpactDirectionCategory.NW;
        else if (angle >= 135 || angle >= -135)
            return ImpactDirectionCategory.N;
        else if (angle <-135 && angle < 0)
            return ImpactDirectionCategory.NE;

        return ImpactDirectionCategory.N;
    }


    /**
     * ShipDataChange will be called when the HUD data input value for the ship data view changes
     */

    public void ShipDataChange(string value)
    {
       

        int _shipData;


        int.TryParse(value, out _shipData);

        if( _shipData <0 || _shipData>=allShips.Count )
        {
            Debug.Log("Warning: Cannot view data for ship " + value);
            
        }else
        {
            shipDataView = _shipData;
        }
    }


    /**
     *  UI Text functions
     */ 

    string GetDirectionCategoryString()
    {


        int index = (int)impactDirectionCategoryData;

        if (index >= 0 && index < dirCatStr.Length)
            return dirCatStr[index];
        else
            return "Invalid Direction Category " + index;

    }


    string GetDistanceCategoryString()
    {


        int index = (int)impactDistanceCategoryData;

        if (index >= 0 && index < distCatStr.Length)
            return distCatStr[index];
        else
            return "Invalid Distance Category " + index;

    }


    string GetSpeedCategoryString()
    {


        int index = (int)impactSpeedCategoryData;

        if (index >= 0 && index < speedCatStr.Length)
            return speedCatStr[index];
        else
            return "Invalid Speed Category " + index;

    }

    string GetSizeCategoryString()
    {


        int index = (int)impactSizeCategoryData;

        if (index >= 0 && index < sizeCatStr.Length)
            return sizeCatStr[index];
        else
            return "Invalid Size Category " + index;

    }

}

