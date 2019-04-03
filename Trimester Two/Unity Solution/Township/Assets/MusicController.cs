using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    [SerializeField] private List<AudioClip> musicTracks = new List<AudioClip> ();

    private AudioSource source;
    private Queue<AudioClip> queue = new Queue<AudioClip> ();

    private float counter = 3.0f;

    private void Awake ()
    {
        source = GetComponent<AudioSource> ();
        InitializeQueue ();
        PlayNext ();
    }

    private void InitializeQueue ()
    {
        queue.Clear ();

        List<AudioClip> clips = new List<AudioClip> ();

        for (int i = 1; i < musicTracks.Count; i++)
        {
            clips.Add ( musicTracks[i] );
        }

        StaticExtensions.ShuffleList ( clips );

        queue.Enqueue ( musicTracks[0] );

        for (int i = 0; i < clips.Count; i++)
        {
            queue.Enqueue ( clips[i] );
        }
    }

    private void RefreshQueue ()
    {
        List<AudioClip> clips = new List<AudioClip> ();

        for (int i = 0; i < musicTracks.Count; i++)
        {
            clips.Add ( musicTracks[i] );
        }

        StaticExtensions.ShuffleList ( clips );

        for (int i = 0; i < clips.Count; i++)
        {
            queue.Enqueue ( clips[i] );
        }
    }

    //[SimpleButtonNew("Play", typeof(MusicController))]
    public void PlayNext ()
    {
        if (queue.Count <= 0) RefreshQueue ();
        source.clip = queue.Dequeue ();
        source.Play ();
    }

    private void Update ()
    {
        if (queue.Count <= 0) return;
        if (source.isPlaying == false) { counter -= Time.deltaTime; }
        else { counter = 3.0f; return; }

        if(counter <= 0.0f)
        {
            counter = 3.0f;
            PlayNext ();
        }
    }
}
