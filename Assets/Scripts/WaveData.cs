
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/**
 * WaveData stores the Asteroid spawn data for a Level
 * 
 */
public class WaveData
{
    // List of spawn points,  destination points and speeds to define each Asteroid
    public List<Vector2> spawnPoint = new List<Vector2>();
    public List<Vector2> destPoint = new List<Vector2>();
    public List<float> speed = new List<float>();
    public List<int> size = new List<int>();

    // define the range of speeds the asroids will appear with
    private const float minMoveSpeed = 2; // inclusive
    private const float maxMoveSpeed = 5; // exclusive

    // define the range of size the asroids will appear with
    private const float minSize = 0;// inclusive
    private const float maxSize = 3; // exclusive

    /**
     * CreateWave will generate the data points for the number of specified asteroids. They will appear from along the boundry points provided. 
     */
    public void CreateWave(int asteroidCount, float xBound, float yBound)
    {
        spawnPoint.Clear();
        destPoint.Clear();
        speed.Clear();


        for (int i = 0; i < asteroidCount; i++)
        {
            // select which side the asteroid shoudl appear from
            int spawnDir = 0; // for now, force to top to bottom, but could move to other or many directions of approach

            Vector2 _spawn = new Vector2();
            Vector2 _dest = new Vector2();




            if (spawnDir == 0) // Top
            {
                _spawn.x = UnityEngine.Random.Range(-xBound, xBound);
                _spawn.y = yBound;

                _dest.x = UnityEngine.Random.Range(-xBound, yBound);
                _dest.y = -yBound;


            }
            else if (spawnDir == 1) // Right
            {
                _spawn.x = xBound;
                _spawn.y = UnityEngine.Random.Range(-yBound, yBound);

                _dest.x = -xBound;
                _dest.y = UnityEngine.Random.Range(-yBound, yBound);

            }
            else if (spawnDir == 2) // Bottom
            {
                _spawn.x = UnityEngine.Random.Range(-xBound, xBound);
                _spawn.y = -yBound;

                _dest.x = UnityEngine.Random.Range(-xBound, xBound);
                _dest.y = yBound;

            }
            else if (spawnDir == 3) // Left
            {
                _spawn.x = -xBound;
                _spawn.y = UnityEngine.Random.Range(-yBound, yBound);

                _dest.x = xBound;
                _dest.y = UnityEngine.Random.Range(-yBound, yBound);

            }

            // pick a speed from the provided range
            int _speed = (int)UnityEngine.Random.Range(minMoveSpeed, maxMoveSpeed);
            int _size = (int)UnityEngine.Random.Range(minSize, maxSize);
            spawnPoint.Add(_spawn);
            destPoint.Add(_dest);
            speed.Add(_speed);
            size.Add(_size);
        }

    }
}
