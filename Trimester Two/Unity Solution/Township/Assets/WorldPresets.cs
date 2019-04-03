using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldPresets : MonoSingleton<WorldPresets> {

    [SerializeField] private List<Preset> presets = new List<Preset> ();
    public List<Preset> Presets { get { return presets; } }

    public int selectedIndex { get; protected set; } = 0;

    public Preset GetPresetOfName(string name)
    {
        return presets.Find ( x => x.presetName == name );
    }

    public void SetPresets (int index, World world)
    {
        //World world = FindObjectOfType<World> ();
        world.worldData = Presets[index].worldData;
        world.noiseData = Presets[index].noiseData;
        world.textureData = Presets[index].textureData;
        selectedIndex = index;
    }

    [System.Serializable]
    public class Preset
    {
        public string presetName;
        public WorldData worldData;
        public NoiseData noiseData;
        public TextureData textureData;
    }
}
