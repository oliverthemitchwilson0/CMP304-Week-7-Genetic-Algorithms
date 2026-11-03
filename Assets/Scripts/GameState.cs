using System;
using UnityEngine;
using static GameController;


/**
 * GameState stores data for a GameState for any ship
 */
public class GameState
{
    // the four components of the game state
    private ImpactDirectionCategory direction;
    private ImpactDistanceCategory distance; 
    private ImpactSpeedCategory speed;
    private ImpactSizeCategory size;

    // the unique ID based on the combination of the 3 status variables
    public int gameStateID;


    /**
     * UpdateState will set the game state based on the provided values, and update the gameStateID value automatically
     */
    public void UpdateState(ImpactDirectionCategory _direction, ImpactDistanceCategory _distance, ImpactSpeedCategory _speed, ImpactSizeCategory _size)
    {
        distance = _distance;
        direction = _direction;
        speed = _speed;
        size = _size;

        SetGameStateID();
    }


    /**
     * SetGameStateID will calculate the unique ID for the current game state based on the 4 status dimensions
     * This can then be used in lookups to a flat array/list/vector
     */
    private void SetGameStateID()
    {
        int v1 = Enum.GetNames(typeof(ImpactDirectionCategory)).Length;
        int v2 = Enum.GetNames(typeof(ImpactDistanceCategory)).Length;
        int v3 = Enum.GetNames(typeof(ImpactSpeedCategory)).Length;
        int v4 = Enum.GetNames(typeof(ImpactSizeCategory)).Length;


        int s1 = (int)direction;
        int s2 = (int)distance;
        int s3 = (int)speed;
        int s4 = (int)size;

        gameStateID = (  v2* v3 * v4 * s1) +   (v3 * v4 * s2) + (v4 * s3) + s4;



    }


}
