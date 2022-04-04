using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatManager : Singleton<HeatManager> {

    public float currentHeatLevel = 1;

    public float worldHeatLevel = 1;
    public float currentHeatLossRate = 1;

    public float currentHeatProduction = 1;


    [SerializeField] float heatLossRateRate = 1;
    [SerializeField] float heatLossRateExp = 1;
    [SerializeField] Health playerHealth;
    public float heatDamageThreshhold = 1;

    [SerializeField] ParticleSystem snowParticles;
    [SerializeField] float minSnowSpawnRate = 10;
    [SerializeField] float maxSnowSpawnRate = 80;




    LinkedList<float> heatProductionOverTime= new LinkedList<float>();


    void UpdateHeatLevels(){
        

        // fixed sample rate?
        // heatProductionOverTime.RemoveFirst();
        // heatProductionOverTime.AddLast();
    }

    void UpdateHeatEffects(){
        // float heatPercentage
        // snowParticles.emission.rateOverTime
        // make more snow tiles
    }
}