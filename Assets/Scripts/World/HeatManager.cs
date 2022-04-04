using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(5)]// after buildings
public class HeatManager : Singleton<HeatManager> {

    [Header("Heat")]

    /*
    Keep heat above a threshold
    
    */

    public float currentHeatLevel = 1;

    public float worldHeatLevel = 1;
    public float currentHeatLossRate = 1;

    // public float currentHeatProduction = 1;

    [Space]
    [SerializeField] float minHeatLevel = 0;
    [SerializeField] float maxHeatLevel = 100;

    [SerializeField] float heatLossRateRate = 1;
    [SerializeField] float heatLossRateExp = 1;


    [Header("History")]
    [SerializeField] int recordHistoryLength = 50;
    [SerializeField] float recordInterval = 0.2f;
    float recordTimer = 0f;
    LinkedList<float> heatLevelOverTime = new LinkedList<float>();
    LinkedList<float> heatProductionOverTime = new LinkedList<float>();
    [SerializeField, ReadOnly] float heatThisFrame = 0f;



    [Header("Effects")]

    [SerializeField] Health playerHealth;
    public float heatDamageRate = 1;
    public float heatDamageThreshhold = 1;

    [Space]
    [SerializeField] ParticleSystem snowParticles;
    [SerializeField] float minSnowSpawnRate = 10;
    [SerializeField] float maxSnowSpawnRate = 80;

    [Space]
    [SerializeField] Slider temperatureSlider;
    [SerializeField] TMPro.TMP_Text tempInfoText;

    [Space]
    [SerializeField] TileType grassType;
    [SerializeField] TileType snowType;
    [SerializeField] TileType waterType;
    [SerializeField] TileType iceType;


    private void Update() {
        UpdateHeatLevels();
    }

    void UpdateHeatLevels() {


        currentHeatLossRate += (heatLossRateRate * Time.deltaTime);
        //  * Mathf.Exp(currentHeatLossRate) * heatLossRateExp;
        float heatLossFrame = currentHeatLossRate * Time.deltaTime;
        worldHeatLevel -= heatLossFrame;
        currentHeatLevel += heatThisFrame - heatLossFrame;


        if (Time.time > recordTimer + recordInterval) {
            // fixed sample rate
            recordTimer = Time.time;
            // record values
            if (heatLevelOverTime.Count >= recordHistoryLength) {
                heatLevelOverTime.RemoveFirst();
                heatProductionOverTime.RemoveFirst();
            }
            heatLevelOverTime.AddLast(currentHeatLevel);
            heatProductionOverTime.AddLast(heatThisFrame);// smooth it out?
        }

        // reset
        heatThisFrame = 0f;

        // UpdateHeatEffects();
    }

    public void AddHeat(float amount) {
        heatThisFrame += amount;
    }


    protected override void Awake() {
        HideTemperInfo();
    }


    void UpdateHeatEffects() {
        float heatPercentage = Mathf.InverseLerp(minHeatLevel, maxHeatLevel, currentHeatLevel);
        // slider
        temperatureSlider.value = heatPercentage;
        // hurt player 
        // if ( < heatDamageThreshhold) {
        //     playerHealth.TakeDamage(heatDamageRate * Time.deltaTime);
        // }

        ParticleSystem.EmissionModule emission = snowParticles.emission;
        emission.rateOverTime = Mathf.Lerp(minSnowSpawnRate, maxSnowSpawnRate, heatPercentage);

        // make more snow tiles
        RectInt bounds = WorldManager.Instance.bounds;
        Vector2Int randomPos = new Vector2Int(
            Random.Range(bounds.xMin, bounds.xMax),
            Random.Range(bounds.yMin, bounds.yMax)
        );
        Tile rtile = WorldManager.Instance.GetTileAt(randomPos);
        if (rtile != null) {
            if (rtile.groundTileType == grassType) {
                rtile.ChangeGroundTile(snowType);
                // } else if (rtile.groundTileType == waterType) {
                //     rtile.ChangeGroundTile(iceType);
            }

        }



        // wind sfx louder?

    }

    public void ShowTemperInfo() {
        tempInfoText.text = "?";
    }
    public void HideTemperInfo() {
        tempInfoText.text = "";
    }
}