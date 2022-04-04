using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    public Player player;
    public Inventory playerInventory;

    [SerializeField][Range(-80, 20)] float maxVol = 10;
    float minVol = -80;
    [SerializeField] AudioMixer mixer;
    [SerializeField] string volParam = "Volume";

    [SerializeField] bool showMainMenu;
    [SerializeField] MenuScreen mainMenu;
    [SerializeField] MenuScreen gameOverScreen;
    [SerializeField] MenuScreen hudScreen;
    [SerializeField] GameObject mainmenuCam;
    [SerializeField] MenuScreen tempScreen;
    [SerializeField] float tempScreenShowDur;


    private void Start() {
        PauseManager.Instance.enablePauseButton = false;
        // PauseManager.Instance.SetPaused(true, false);
        Time.timeScale = 0;
        mainmenuCam.SetActive(true);
        hudScreen.Hide();
        Time.timeScale = 0f;
        if (!showMainMenu) {
            StartGame();
        }
    }


    public void SetAudioVolume(float val) {
        SetVolume(volParam, val);
    }

    public void OnPlayerDie(){
        GameOver();
    }


    public void StartGame() {
        Debug.Log("Game start");
        // todo anim
        PauseManager.Instance.enablePauseButton = true;
        mainmenuCam.SetActive(false);
        mainMenu.Hide();
        hudScreen.Show();
        Time.timeScale = 1f;
        tempScreen.Show();
        Invoke(nameof(HideTScreen), tempScreenShowDur);
    }
    void HideTScreen(){
        tempScreen.Hide();
    }
    [ContextMenu("game Ove")]
    public void GameOver() {
        Debug.Log("Game over");
        PauseManager.Instance.enablePauseButton = false;
        PauseManager.Instance.SetPaused(true, false);
        gameOverScreen.Show();
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene(0);
    }

    public void Exit() {
        Application.Quit();
    }







    float NormalizeVolume(float value) {
        // from -80 20 to 0 1
        // return (value - minVol) / (maxVol - minVol);
        value = Remap(value, minVol, maxVol, -4, 0f);
        value = Mathf.Pow(10, value);
        return value;
    }
    float DenormalizeVolume(float value) {
        // from 0 1 to -80 20
        // return value * (maxVol - minVol) + minVol;
        value = Mathf.Max(value, 0.0001f);
        value = Mathf.Log10(value);
        // -4 to 0 is the range of log10
        value = Remap(value, -4, 0f, minVol, maxVol);
        return value;
    }
    float Remap(float value, float oldMin, float oldMax, float newMin, float newMax) {
        float oldRange = oldMax - oldMin;
        float newRange = newMax - newMin;
        return ((value - oldMin) / oldRange) * newRange + newMin;
    }
    void SetVolume(string paramName, float volumeNorm, bool save = true) {
        volumeNorm = DenormalizeVolume(volumeNorm);
        // note: this will not work in Awake or OnEnable, Unity bug
        mixer.SetFloat(paramName, volumeNorm);
    }
    float GetVolume(string paramName) {
        mixer.GetFloat(paramName, out float val);
        return NormalizeVolume(val);
    }
}