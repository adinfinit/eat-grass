using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{


    public enum SoundEffect
    {
        SheepBaa, SheepCharging, SheepAttack,
        PlayerAttack, PlayerDodge,
        Wind, Thunder, Birds,
    };

    public AudioClip[] SheepBaa;
    public AudioClip[] SheepCharging;
    public AudioClip[] SheepAttack;
    public AudioClip[] PlayerAttack;
    public AudioClip[] PlayerDodge;
    public AudioClip[] Wind;
    public AudioClip[] Thunder;
    public AudioClip[] Birds;
    
    public void PlaySound(SoundEffect sfx)
    {
        // Enum to array mapping
        switch (sfx)
        {
            case SoundEffect.PlayerAttack:
                _PlaySoundEffectSafe(ChooseRandom( PlayerAttack ));
                return;
            case SoundEffect.PlayerDodge:
                _PlaySoundEffectSafe(ChooseRandom(PlayerDodge));
                return;
            case SoundEffect.SheepBaa:
                _PlaySoundEffectSafe(ChooseRandom(SheepBaa));
                return;
            case SoundEffect.SheepCharging:
                _PlaySoundEffectSafe(ChooseRandom(SheepCharging));
                return;
            case SoundEffect.SheepAttack:
                _PlaySoundEffectSafe(ChooseRandom(SheepAttack));
                return;
            case SoundEffect.Wind:
                _PlaySoundEffectSafe(ChooseRandom(Wind));
                return;
            case SoundEffect.Thunder:
                _PlaySoundEffectSafe(ChooseRandom(Thunder));
                return;
            case SoundEffect.Birds:
                _PlaySoundEffectSafe(ChooseRandom(Birds));
                return;
        }

    }

    private static AudioClip ChooseRandom(AudioClip[] files)
    {
        if (files.Length > 0)
        {
            int index = Random.Range(0, files.Length - 1);
            return files[index];
        }

        return null;
    }

    private void _PlaySoundEffectSafe( AudioClip sfx )
    {
        if (sfx)
        {
            GameObject soundGameObject = new GameObject("Sound Effect");
            soundGameObject.transform.parent = transform;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            soundGameObject.AddComponent<AudioSourceAutoDestroy>();
            audioSource.PlayOneShot(sfx);
        }
    }


    void Awake()
    {
        // Sheep events
        EventManager.StartListening("SheepAttack", () => PlaySound(SoundEffect.SheepAttack));
        EventManager.StartListening("SheepCharging", () => PlaySound(SoundEffect.SheepCharging));
        EventManager.StartListening("SheepBaa", () => PlaySound(SoundEffect.SheepBaa));

        // Player events
        EventManager.StartListening("PlayerAttack", () => PlaySound(SoundEffect.PlayerAttack));
        EventManager.StartListening("PlayerDodge", () => PlaySound(SoundEffect.PlayerDodge));

        // Environment events 
        EventManager.StartListening("Wind", () => PlaySound(SoundEffect.Wind));
        EventManager.StartListening("Thunder", () => PlaySound(SoundEffect.Thunder));
        EventManager.StartListening("Birds", () => PlaySound(SoundEffect.Birds));
    }
}