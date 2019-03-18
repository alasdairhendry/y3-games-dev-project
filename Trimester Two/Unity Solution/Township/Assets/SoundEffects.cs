using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour {

    public static SoundEffects Instance;

    [SerializeField] private GameObject prefab;

    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy ( this.gameObject );
    }

    //public void Create (AudioClip clip, float volume, float spatialBlend, Vector2 pitchRange, Vector2 distanceRolloff, Vector3 position)
    //{
    //    GameObject go = Instantiate ( prefab );

    //    if (pitchRange == Vector2.zero || pitchRange == null) pitchRange = Vector2.one;
    //    if (distanceRolloff == Vector2.zero || distanceRolloff == Vector2.one || distanceRolloff == null) distanceRolloff = new Vector2 ( 3.0f, 25.0f );
    //    if (position == null) position = Vector3.zero;

    //    go.transform.position = position;

    //    AudioSource source = go.GetComponent<AudioSource> ();
    //    SelfDestruct sd = go.GetComponent<SelfDestruct> ();

    //    source.clip = clip;
    //    source.volume = volume;
    //    source.spatialBlend = spatialBlend;
    //    source.pitch = Random.Range ( pitchRange.x, pitchRange.y );
    //    source.minDistance = distanceRolloff.x;
    //    source.maxDistance = distanceRolloff.y;

    //    sd.SetLifetime ( clip.length + 0.5f );
    //    sd.Play ();
    //}

    public void Create (SoundEffectData data)
    {
        GameObject go = Instantiate ( prefab );

        if (data.pitchRange == Vector2.zero || data.pitchRange == null) data.pitchRange = Vector2.one;
        if (data.distanceRolloff == Vector2.zero || data.distanceRolloff == Vector2.one || data.distanceRolloff == null) data.distanceRolloff = new Vector2 ( 3.0f, 25.0f );
        if (data.position == null) data.position = Vector3.zero;

        data.SetLocalisedPosition ();
        go.transform.position = data.position;

        AudioSource source = go.GetComponent<AudioSource> ();
        SelfDestruct sd = go.GetComponent<SelfDestruct> ();

        source.clip = data.clip;
        source.volume = data.volume;
        source.spatialBlend = data.spatialBlend;
        source.pitch = Random.Range ( data.pitchRange.x, data.pitchRange.y );
        source.minDistance = data.distanceRolloff.x;
        source.maxDistance = data.distanceRolloff.y;

        source.Play ();

        sd.SetLifetime ( data.clip.length + 0.5f );
        sd.Play ();
    }

    [System.Serializable]
    public class SoundEffectData
    {
        public AudioClip clip;
        [Range ( 0, 1 )] public float volume = 1;
        [Range ( 0, 1 )] public float spatialBlend = 1;
        public Vector2 pitchRange = Vector2.one;
        public Vector2 distanceRolloff = new Vector2 ( 3.0f, 25.0f );
        public Vector3 position = Vector3.zero;
        public bool positionIsLocal = false;
        public Transform transform;

        private bool positionHasBeenLocalised = false;

        public SoundEffectData (AudioClip clip, float volume, float spatialBlend, Vector2 pitchRange, Vector2 distanceRolloff, Vector3 position, bool positionIsLocal, Transform transform = null)
        {
            this.clip = clip;
            this.volume = volume;
            this.spatialBlend = spatialBlend;
            this.pitchRange = pitchRange;
            this.distanceRolloff = distanceRolloff;
            this.position = position;
            this.positionIsLocal = positionIsLocal;
            this.transform = transform;
        }

        public void SetLocalisedPosition ()
        {
            if (transform == null)
            {
                this.positionIsLocal = false;
            }
            else
            {
                if (this.positionIsLocal)
                {
                    if (!this.positionHasBeenLocalised)
                    {
                        this.position = transform.TransformPoint ( position );
                        this.positionHasBeenLocalised = true;
                    }
                }
            }
        }
    }
}
