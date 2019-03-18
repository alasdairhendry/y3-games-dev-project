using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent_SoundEffect : MonoBehaviour {

    [SerializeField] private List<SoundEffects.SoundEffectData> effects = new List<SoundEffects.SoundEffectData> ();

    private void Update ()
    {
        if (Input.GetKeyDown ( KeyCode.C ))
        {
            AnimationEvent_PlaySoundEffect ( 0 );
        }
    }

    public void AnimationEvent_PlaySoundEffect (int index)
    {
        if (index < 0 || index >= effects.Count) { Debug.LogError ( "Out of bounds", this.gameObject ); return; }

        SoundEffects.Instance.Create ( effects[index] );
    }
}
