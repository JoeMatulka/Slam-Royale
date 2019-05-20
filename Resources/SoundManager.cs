using System.Collections;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    private AudioClip footstep_1;
    private AudioClip footstep_2;

    private AudioClip smallThud;
    private AudioClip bigThud;

    private AudioClip grunt_1;
    private AudioClip grunt_2;

    private AudioClip slap_1;
    private AudioClip slap_2;

    private AudioClip cheer_1;
    private AudioClip cheer_2;

    private AudioClip bellRing;
    private AudioClip singleBellRing;

    private AudioClip music;

    protected SoundManager() { }

    public void LoadSounds()
    {
        footstep_1 = (AudioClip)Resources.Load("Sounds/footstep_1", typeof(AudioClip));
        footstep_2 = (AudioClip)Resources.Load("Sounds/footstep_2", typeof(AudioClip));

        smallThud = (AudioClip)Resources.Load("Sounds/small_thud", typeof(AudioClip));
        bigThud = (AudioClip)Resources.Load("Sounds/big_thud", typeof(AudioClip));

        grunt_1 = (AudioClip)Resources.Load("Sounds/grunt_1", typeof(AudioClip));
        grunt_2 = (AudioClip)Resources.Load("Sounds/grunt_2", typeof(AudioClip));

        slap_1 = (AudioClip)Resources.Load("Sounds/slap_1", typeof(AudioClip));
        slap_2 = (AudioClip)Resources.Load("Sounds/slap_2", typeof(AudioClip));

        cheer_1 = (AudioClip)Resources.Load("Sounds/crowd_1", typeof(AudioClip));
        cheer_2 = (AudioClip)Resources.Load("Sounds/crowd_2", typeof(AudioClip));

        bellRing = (AudioClip)Resources.Load("Sounds/bell_ring", typeof(AudioClip));
        singleBellRing = (AudioClip)Resources.Load("Sounds/single_bell_ring", typeof(AudioClip));

        music = (AudioClip)Resources.Load("Music/Street - Mayhem_Looping", typeof(AudioClip));
    }

    public void Step(AudioSource source)
    {
        AudioClip step = Random.value > .5f ? footstep_1 : footstep_2;
        PlayAudioClip(step, source, 1f);
    }

    public void Slap(AudioSource source)
    {
        AudioClip slap = Random.value > .5f ? slap_1 : slap_2;
        PlayAudioClip(slap, source, .6f);
    }

    public void Grunt(AudioSource source)
    {
        AudioClip grunt = Random.value > .5f ? grunt_1 : grunt_2;
        PlayAudioClip(grunt, source, .65f);
    }

    public void SmallThud(AudioSource source)
    {
        PlayAudioClip(smallThud, source, .5f);
    }

    public void BigThud(AudioSource source)
    {
        PlayAudioClip(bigThud, source, .5f);
    }

    public void CrowdCheer(AudioSource source)
    {
        AudioClip cheer = Random.value > .5f ? cheer_1 : cheer_2;
        PlayAudioClip(cheer, source, .75f);
    }

    public void BellRing(AudioSource source) {
        PlayAudioClip(bellRing, source, .75f);
    }

    public void SingleBellRing(AudioSource source)
    {
        PlayAudioClip(singleBellRing, source, 1f);
    }

    public void FadeOutAllSounds(float transitionTime) {
        AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource source in allAudioSources)
        {
            if (!source.name.Equals("Game Manager")) {
                StartCoroutine(FadeOutAudioSource(source, transitionTime));
            }
        }
    }

    public IEnumerator FadeOutAudioSource(AudioSource source, float transitionTime) {
        float startVolume = source.volume;

        while (source.volume > 0) {
            source.volume -= startVolume * Time.deltaTime / transitionTime;

            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    public void StopAllSounds() {
        AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource source in allAudioSources)
        {
            source.Stop();
        }
    }

    public void PlayGameMusic(bool toggle)
    {
        AudioSource musicPlayer = Camera.main.GetComponent<AudioSource>();
        if (musicPlayer)
        {
            if (toggle)
            {
                musicPlayer.Play();
            }
            else {
                musicPlayer.Stop();
            }
        }
    }

    private void PlayAudioClip(AudioClip clip, AudioSource source, float volume)
    {
        if (GameDataManager.Instance.IsSound)
        {
            source.PlayOneShot(clip, volume);
        }
    }
}
