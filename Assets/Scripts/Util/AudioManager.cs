using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    [SerializeField] GameObject audioSfxPrefab;
    public void PlaySfx(AudioClip clip, Vector3 pos, float vol) {
        if (clip==null){
            Debug.LogWarning("cant play empty clip!");
            return;
        }
        GameObject sfxgo = Instantiate(audioSfxPrefab, transform);
        sfxgo.transform.position = pos;
        AudioSource audioSource = sfxgo.GetComponent<AudioSource>();
        audioSource.volume = vol;
        audioSource.PlayOneShot(clip);
        Destroy(sfxgo, clip.length);
    }
}