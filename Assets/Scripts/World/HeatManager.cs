using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DefaultExecutionOrder(5)]// after buildings
public class HeatManager : Singleton<HeatManager> {

    [Header("Heat")]

    /*
    Keep heat above a threshold
    using kelivn scale
    
    */

    public float currentHeatLevel = 1;

    public float worldHeatLevel = 1;
    public float currentHeatLossRate = 1;

    // public float currentHeatProduction = 1;

    [Space]
    [SerializeField, ReadOnly] float heatPercentage;
    [SerializeField, ReadOnly] float heatPercentageAbs;
    [SerializeField, ReadOnly] float heatProductionAvg = 0f;
    [Space]
    [SerializeField, ReadOnly] float heatThisFrame = 0f;
    [SerializeField, ReadOnly] float heatLastFrame = 0f;
    [SerializeField, ReadOnly] float heatProductionThisFrameNorm = 0;

    [Space]
#if UNITY_EDITOR
    [SerializeField][Range(0f, 20f)] float timescaler = 1f;
#endif
    [SerializeField, ReadOnly] float time = 0;

    [Header("Rates")]
    // per minute
    [SerializeField] float heatLossStartRate = 5;
    [SerializeField] float heatLossRateRate = 1;
    [SerializeField] float heatProductionScale = 1;
    // [SerializeField] float heatLossRateExp = 1;

    [Header("Levels")]
    [SerializeField] float startingHeatLevel = 288;
    [SerializeField] float absZeroHeatLevel = 0;
    [SerializeField] float freezeThreshold = 273;
    [SerializeField] float maxHeatLevel = 330;



    [Header("History")]
    [SerializeField] int recordHistoryLength = 50;
    [SerializeField] float recordInterval = 0.2f;
    float recordTimer = 0f;
    LinkedList<float> heatLevelOverTime = new LinkedList<float>();
    LinkedList<float> heatProductionOverTime = new LinkedList<float>();
    [SerializeField] int heatProductionAvgSampleSize = 30;


    [Header("Effects")]

    [SerializeField] Health playerHealth;
    public float heatDamageRate = 1;

    [Space]
    [SerializeField] ParticleSystem snowParticles;
    [SerializeField] float minSnowSpawnRate = 10;
    [SerializeField] float maxSnowSpawnRate = 80;

    [Space]
    [SerializeField] Slider temperatureSlider;
    [SerializeField] bool keepInfoShown = false;
    [SerializeField] TMPro.TMP_Text tempInfoText;
    bool temperatureHovered = false;

    [Space]
    [SerializeField] float changeTileRate = 0.1f;
    [SerializeField] TileType grassType;
    [SerializeField] TileType snowType;
    [SerializeField] TileType waterType;
    [SerializeField] TileType iceType;
    [SerializeField, ReadOnly] float changeTileTimer = 0f;

    public UnityEvent<float> onHeatUpdate;

    protected override void Awake() {
        ResetHeat();
        HideTemperInfo();
    }
#if UNITY_EDITOR
    private void OnValidate() {
        if (Application.isPlaying) {
            Time.timeScale = timescaler;
        }
    }
#endif

    private void Update() {
        UpdateHeatLevels();
        time = Time.time / 60f;
    }

    [ContextMenu("Reset heat")]
    void ResetHeat() {
        currentHeatLevel = startingHeatLevel;
        currentHeatLossRate = heatLossStartRate;
        worldHeatLevel = startingHeatLevel;
    }

    void UpdateHeatLevels() {
        currentHeatLossRate += ((heatLossRateRate / 60f) * Time.deltaTime);

        float heatLossFrame = (currentHeatLossRate / 60f) * Time.deltaTime;
        worldHeatLevel -= heatLossFrame;
        if (worldHeatLevel < absZeroHeatLevel) {
            worldHeatLevel = absZeroHeatLevel;
        }
        currentHeatLevel -= heatLossFrame;
        if (currentHeatLevel < absZeroHeatLevel) {
            currentHeatLevel = absZeroHeatLevel;
        }
        // stop at freeze level, to allow for possible recovery?
        currentHeatLevel = Mathf.Max(currentHeatLevel, freezeThreshold - 10);

        // should stop cur heat from reaching max
        // https://www.desmos.com/calculator/8elrztdnhd
        float heatMaxLimit = -Mathf.Pow(2 * (maxHeatLevel - currentHeatLevel) + 1, -1f) + 1f;
        heatProductionThisFrameNorm = heatThisFrame / 60f * heatMaxLimit;
        currentHeatLevel += heatProductionThisFrameNorm;
        if (currentHeatLevel > maxHeatLevel) {
            currentHeatLevel = maxHeatLevel;
        }

        if (Time.time > recordTimer + recordInterval) {
            // fixed sample rate
            recordTimer = Time.time;
            // record values
            if (heatLevelOverTime.Count >= recordHistoryLength) {
                heatLevelOverTime.RemoveFirst();
                heatProductionOverTime.RemoveFirst();
            }
            heatLevelOverTime.AddLast(currentHeatLevel);
            // frame min
            heatProductionOverTime.AddLast(heatThisFrame * 1f / Time.deltaTime);

            heatProductionAvg = heatProductionOverTime.Skip(heatProductionOverTime.Count - heatProductionAvgSampleSize)
                                .Average();
        }

        heatPercentage = Mathf.InverseLerp(freezeThreshold, maxHeatLevel, currentHeatLevel);
        heatPercentageAbs = Mathf.InverseLerp(absZeroHeatLevel, maxHeatLevel, currentHeatLevel);
        // reset
        heatLastFrame = heatThisFrame;
        heatThisFrame = 0f;

        UpdateHeatEffects();
    }

    public void AddHeat(float amount) {
        // Debug.Log("adding " + amount);
        heatThisFrame += amount * heatProductionScale;
    }



    void UpdateHeatEffects() {
        onHeatUpdate?.Invoke(heatPercentage);
        // info
        if (temperatureHovered) {
            UpdateTemperatureInfo();
        }
        // slider
        temperatureSlider.value = heatPercentage;

        // hurt player 
        if (currentHeatLevel < freezeThreshold) {
            playerHealth.TakeDamage(heatDamageRate * Time.deltaTime);
        }

        ParticleSystem.EmissionModule emission = snowParticles.emission;
        emission.rateOverTime = Mathf.Lerp(minSnowSpawnRate, maxSnowSpawnRate, 1f - heatPercentage);

        // make more snow tiles
        if (currentHeatLevel < startingHeatLevel && Time.time >= changeTileTimer) {
            // try only once in rate
            changeTileTimer = Time.time + changeTileRate * Random.Range(0.6f, 1.4f);
            RectInt bounds = WorldManager.Instance.bounds;
            Vector2Int randomPos = new Vector2Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax)
            );
            Tile rtile = WorldManager.Instance.GetTileAt(randomPos);
            if (rtile != null) {
                // Debug.Log("freezing tile " + rtile.groundTileType + " " + rtile.name);
                if (rtile.groundTileType == grassType) {
                    rtile.ChangeGroundTile(snowType);
                    // Debug.Log("freezing tile to snow");
                    // } else if (rtile.groundTileType == waterType) {
                    //     rtile.ChangeGroundTile(iceType);
                }
            }
        }

        // wind sfx louder?

    }
    public void ShowTemperInfo() {
        temperatureHovered = true;
        UpdateTemperatureInfo();
    }

    private void UpdateTemperatureInfo() {
        tempInfoText.text =
        $@"Current Heat: 
{currentHeatLevel:F2}K
Loss Rate: 
{currentHeatLossRate:F2}K/min
Production Rate: 
{heatProductionAvg:F2}K/min"
                    ;
    }

    public void HideTemperInfo() {
        if (!keepInfoShown) {
            temperatureHovered = false;
            tempInfoText.text = "";
        }
    }
}