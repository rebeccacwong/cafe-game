using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Interface
/*
 * When you call Level::InitializeLevel, it should set up
 * all the level specific parameters to make the level
 * harder or easier. 
 */
public interface Level
{
    public void InitializeLevel();
}
#endregion

#region Levels
public class LevelOne : Level
{
    public void InitializeLevel()
    {
        Customer.setCustomerWaitTime(2.5f * 60f);
        SpawnController.minNumCustomers = 5;
        SpawnController.maxNumCustomers = 8;
        SpawnController.minSpawnInterval = 12f;
        SpawnController.maxSpawnInterval = 50f;
    }
}

public class LevelTwo : Level
{
    public void InitializeLevel()
    {
        Customer.setCustomerWaitTime(2f * 60f);
        SpawnController.minNumCustomers = 8;
        SpawnController.maxNumCustomers = 10;
        SpawnController.minSpawnInterval = 12f;
        SpawnController.maxSpawnInterval = 45f;
    }
}

public class LevelThree : Level
{
    public void InitializeLevel()
    {
        Customer.setCustomerWaitTime(1.5f * 60f);
        SpawnController.minNumCustomers = 12;
        SpawnController.maxNumCustomers = 15;
        SpawnController.minSpawnInterval = 10f;
        SpawnController.maxSpawnInterval = 40f;
    }
}
#endregion