using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sounds;

    
    private void Awake()
    {
        if (AudioManager.instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.group;
        }
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            StopAllandPlay("BGM_01");
        }
        //if (GameManager.instance.buttonNavigation.checkIfSceneActive(ButtonNav.SceneList.MainMenu))
        //{
        //    Debug.Log("Playing Music");
        //    Play("lagu3");
        //}

    }

    private Sound SearchSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound " + s + " not found");
            return null;
        }
        else
        {
            return s;
        }
    }

    public void Play(string name)
    {
        Sound s = SearchSound(name);
        if (s != null)
        {
            s.source.Play();
        }

    }

    public void Pause(string name)
    {
        Sound s = SearchSound(name);
        if (s != null)
        {
            if (isSoundPlaying(name))
            {
                s.source.Pause();
            }
        }
    }

    public void Stop(string name)
    {
        Sound s = SearchSound(name);
        if (s != null)
        {
            s.source.Stop();
        }
    }

    public string GetCurrentlyPlayingAudio()
    {
        string name = "";
        foreach(Sound a in sounds)
        {
            if (isSoundPlaying(a.name))
            {
                name = a.name;
            }
        }

        return name;
    }

    public void StopAllandPlay(string name)
    {
        StopAll();
        Play(name);
    }

    public bool isSoundPlaying(string name)
    {
        Sound s = SearchSound(name);
        return s.source.isPlaying;
    }

    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            Stop(s.name);
        }
    }

}
