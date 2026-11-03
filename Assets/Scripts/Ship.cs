using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using UnityEditor;
using UnityEngine;
using static GameController;



/**
 * 
 * Ship represnets the AI's ship in the level
 */
public class Ship : MonoBehaviour
{

    // define the possible actions
  
    public enum Action { None=0, Left, Right};


    // current speed multiplier
    private float speed = 1;
    
    // tracking data
    private int collisions;
    private float damage;

    // visual options
    public Sprite stopSprite;
    public Sprite slowSprite;
    public Sprite fastSprite;


    // organise the DNA data into lists
    private List<int> shipAction = new List<int>();

    private int actionSize;
    private int stateSize;

    Action updateAction;


    // the map boundries the ship cannot move beyond - symetrical, so valid range is from -xBount to +xBound etc
    private float xBound;
    private float yBound;

    // store the current data for this ship - each ship will be different as data releative to closest Asteroid
    private GameState currentState = new GameState();


    private void Awake()
    {

        actionSize = Enum.GetNames(typeof(Action)).Length;
    }

    void Start()
    {

        updateAction = Action.None;


    }

    // Update is called once per frame
    void Update()
    {


    }

    /**
     * Init should be called to initliase a ship at the start of any level / simulation phase
     */
    public void Init(int _stateSize, float _xBound, float _yBound)
    {
        collisions = 0;
        damage = 0;
        //this.transform.position = Vector3.zero;

        stateSize = _stateSize;
        xBound = _xBound;
        yBound = _yBound;
    }


    /**
     * SetGene will take the GA's DNA structure and store it in the Ships data structures, ready for use
     */
    public void SetGene(DNA<int> _geneData)
    {
        if(_geneData.Genes.Length != stateSize)
        {
            Debug.Log("WARNING: _geneData is not sufficent length for state size " + stateSize);
            return;
        }


        shipAction.Clear();

        for (int i = 0; i < stateSize; i++)
        {
            shipAction.Add(_geneData.Genes[i]);
        }

        // Map the integers to the enum range
        ApplyDNARange();
    }



    /**
     * 
     * ApplyDNARange will ensure the gene data is mapped to an appropriate enum
     */
    void ApplyDNARange()
    {

        for (int i = 0; i < shipAction.Count; i++)
        {
            shipAction[i] = shipAction[i] % actionSize;
        }
    }

    /**
     * SetGameState will store the game state based on the provided values
     */
    public void SetGameState(ImpactDirectionCategory _direction, ImpactDistanceCategory _distance, ImpactSpeedCategory _speed , ImpactSizeCategory _size)
    {
        currentState.UpdateState(_direction, _distance, _speed, _size);
    }

    /**
     * SelectActionForGameState will set the Thrust and Steer based on the DNA data for the currently stored Game State
     */
    public void SelectActionForGameState()
    {

        int _gameStateID = currentState.gameStateID;
        if(_gameStateID >= shipAction.Count)
        {
            Debug.LogWarning("WARNING: Cannot select action for game state id " + _gameStateID);
        }


        // use the _gameStateID index to pick the Thrust and Steer values from the list
        //_gameStateID is auto calculated when the game state is set in the GameState class function UpdateState()
        SetAction((Action)shipAction[_gameStateID]);


    }
    public int GetCollisions()
    {
        return collisions;
    }

    public float GetDamage()
    {
        return damage;
    }


    public void SetAction(Action _action)
    {


        updateAction = _action;



    }



    /**
     * ApplyMovement will move the ship accoring to the steer direction
     */
    public void ApplyMovement()
    {

        if (updateAction == Action.Left)
        {
            this.transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
        }
        else if (updateAction == Action.Right)
        {
            this.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }

    
        Vector3 _newPos = this.transform.position;      

        // enforce the boundry
        if(this.transform.position.x >= xBound )
            _newPos.x = xBound;
        if(this.transform.position.x <= -xBound)
            _newPos.x = -xBound;
        if (this.transform.position.y >= yBound)
            _newPos.y = yBound;
        if (this.transform.position.y <= -yBound)
            _newPos.y = -yBound;

        this.transform.position = _newPos;

    }



    /**
     * 
     * OnTriggerEnter2D will be called when the Ship collides with another object
     */
    void OnTriggerEnter2D(Collider2D col)
    {
        // if asteroid, update counter
        if (col.gameObject.CompareTag("Asteroid"))
        {
            collisions++;

            // set the damage to be the size value of the astroid: large = 2, small = 0
            damage += col.GetComponent<Asteroid>().GetAsteroidSize() + 1; // +1 to ensure all asteroids damage
        }
    }


}
