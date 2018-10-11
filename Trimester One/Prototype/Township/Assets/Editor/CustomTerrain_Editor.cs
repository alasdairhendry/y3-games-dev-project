using UnityEditor;
using UnityEngine;
using EditorGUITable;

[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]
public class CustomTerrain_Editor : Editor {

    CustomTerrain terrain;

    SerializedProperty randomHeightRange;
    SerializedProperty heightMapScale;
    SerializedProperty heightMapImage;

    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;

    SerializedProperty perlinOffsetX;
    SerializedProperty perlinOffsetY;

    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistence;
    SerializedProperty perlinHeightScale;

    SerializedProperty fallOff;
    SerializedProperty dropOff;

    SerializedProperty MpDHeightMin;
    SerializedProperty MpDHeightMax;
    SerializedProperty MpDHeightDampPower;
    SerializedProperty MpDRoughness;

    GUITableState perlinParameterTable;
    SerializedProperty perlinParameters;

    GUITableState splatMapTable;
    SerializedProperty splatLayers;
    SerializedProperty splatOffset;
    SerializedProperty splatNoiseXScale;
    SerializedProperty splatNoiseYScale;
    SerializedProperty splatNoiseScaler;

    SerializedProperty erosionType;
    SerializedProperty erosionStrength;
    SerializedProperty erosionAmount;
    SerializedProperty springsPerRiver;
    SerializedProperty solubility;
    SerializedProperty droplets;
    SerializedProperty erosionSmoothAmount;

    private bool showRandom = false;
    private bool showLoadHeightMap = false;
    private bool showGeneratePerlinMap = false;
    private bool showGenerateFractalBrowning = false;
    private bool showGenerateLayeredFractalBrowning = true;
    private bool showVoronoi = false;
    private bool showMidpointDisplacement = false;
    private bool showSplatmap = false;
    private bool showErosion = false;

    private void OnEnable()
    {
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
        heightMapScale = serializedObject.FindProperty("heightMapScale");
        heightMapImage = serializedObject.FindProperty("heightMapImage");

        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");

        perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
        perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");

        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistence = serializedObject.FindProperty("perlinPersistence");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");

        fallOff = serializedObject.FindProperty("fallOff");
        dropOff = serializedObject.FindProperty("dropOff");

        MpDHeightMin = serializedObject.FindProperty("MpDHeightMin");
        MpDHeightMax = serializedObject.FindProperty("MpDHeightMax");
        MpDHeightDampPower = serializedObject.FindProperty("MpDHeightDampPower");
        MpDRoughness = serializedObject.FindProperty("MpDRoughness");

        splatOffset = serializedObject.FindProperty("splatOffset");
        splatNoiseXScale = serializedObject.FindProperty("splatNoiseXScale");
        splatNoiseYScale = serializedObject.FindProperty("splatNoiseYScale");
        splatNoiseScaler = serializedObject.FindProperty("splatNoiseScaler");

        perlinParameterTable = new GUITableState("perlinParameterTable");
        perlinParameters = serializedObject.FindProperty("perlinParameters");

        splatMapTable = new GUITableState("splatMapTable");
        splatLayers = serializedObject.FindProperty("splatLayers");

        erosionType = serializedObject.FindProperty("erosionType");
        erosionStrength = serializedObject.FindProperty("erosionStrength");
        erosionAmount = serializedObject.FindProperty("erosionAmount");
        springsPerRiver = serializedObject.FindProperty("springsPerRiver");
        solubility = serializedObject.FindProperty("solubility");
        droplets = serializedObject.FindProperty("droplets");
        erosionSmoothAmount = serializedObject.FindProperty("erosionSmoothAmount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        terrain = (CustomTerrain)target;

        Draw_ShowRandom();
        Draw_ShowLoadHeightMap();
        Draw_GeneratePerlinMap();
        Draw_GenerateFractalBrowning();
        Draw_GenerateLayeredFractalBrowning();
        Draw_Voronoi();
        Draw_MidpointDisplacement();
        Draw_Splatmap();
        Draw_Erosion();

        if (GUILayout.Button("Smooth Terrain"))
        {
            terrain.Smooth();
        }
        if (GUILayout.Button("Reset Terrain"))
        {
            terrain.ResetTerrain();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void Draw_ShowRandom()
    {
        showRandom = EditorGUILayout.Foldout(showRandom, "Randomise (Lecture 10)");
        if (showRandom)
        {
            BeginVertical();
            GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomHeightRange);
            if (GUILayout.Button("Randomise Terrain"))
            {
                terrain.RandomiseTerrain(false);
            }
            EndVertical();
        }
    }

    private void Draw_ShowLoadHeightMap()
    {
        showLoadHeightMap = EditorGUILayout.Foldout(showLoadHeightMap, "Load Height Map (Lecture 12)");
        if (showLoadHeightMap)
        {
            BeginVertical();
            GUILayout.Label("Set Heights From A Texture", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(heightMapImage);
            EditorGUILayout.PropertyField(heightMapScale);
            if (GUILayout.Button("Load Texture"))
            {
                terrain.LoadTexture(false);
            }
            EndVertical();
        }
    }

    private void Draw_GeneratePerlinMap()
    {
        showGeneratePerlinMap = EditorGUILayout.Foldout(showGeneratePerlinMap, "Perlin Noise (Lecture 15)");
        if (showGeneratePerlinMap)
        {
            BeginVertical();
            GUILayout.Label("Generate Height From Perlin Noise", EditorStyles.boldLabel);
            EditorGUILayout.Slider(perlinXScale, 0, 1, new GUIContent("X Scale"));
            EditorGUILayout.Slider(perlinYScale, 0, 1, new GUIContent("Y Scale"));
            EditorGUILayout.Space();
            EditorGUILayout.IntSlider(perlinOffsetX, 0, 10000, new GUIContent("X Offset"));
            EditorGUILayout.IntSlider(perlinOffsetY, 0, 10000, new GUIContent("Y Offset"));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Additive Map"))
            {
                terrain.GeneratePerlinTerrain(true);
            }
            if (GUILayout.Button("Generate Map"))
            {
                terrain.GeneratePerlinTerrain(false);
            }
            EditorGUILayout.EndHorizontal();
            EndVertical();
        }
    }

    private void Draw_GenerateFractalBrowning()
    {
        showGenerateFractalBrowning = EditorGUILayout.Foldout(showGenerateFractalBrowning, "Fractal Browning Motion (Lecture 16)");
        if (showGenerateFractalBrowning)
        {
            BeginVertical();
            GUILayout.Label("Generate Height From Fractal Browning Motion", EditorStyles.boldLabel);

            EditorGUILayout.Slider(perlinXScale, 0, 1, new GUIContent("X Scale"));
            EditorGUILayout.Slider(perlinYScale, 0, 1, new GUIContent("Y Scale"));

            EditorGUILayout.Space();

            EditorGUILayout.IntSlider(perlinOffsetX, 0, 10000, new GUIContent("X Offset"));
            EditorGUILayout.IntSlider(perlinOffsetY, 0, 10000, new GUIContent("Y Offset"));

            EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));

            EditorGUILayout.Slider(perlinPersistence, 1, 10, new GUIContent("Persistence"));
            EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Additive Map"))
            {
                terrain.GenerateFractalBrownian(true);
            }
            if (GUILayout.Button("Generate Map"))
            {
                terrain.GenerateFractalBrownian(false);
            }
            EditorGUILayout.EndHorizontal();
            EndVertical();
        }
    }

    private void Draw_GenerateLayeredFractalBrowning()
    {
        showGenerateLayeredFractalBrowning = EditorGUILayout.Foldout(showGenerateLayeredFractalBrowning, "Layered Fractal Browning Motion (Lecture 18)");
        if (showGenerateLayeredFractalBrowning)
        {
            BeginVertical();
            GUILayout.Label("Generate Height From Layered Fractal Browning Motion", EditorStyles.boldLabel);

            perlinParameterTable = GUITableLayout.DrawTable(perlinParameterTable, perlinParameters);            

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Layer")) terrain.AddFractalBrownianLayer();            
            if (GUILayout.Button("Remove Layer")) terrain.RemoveFractalBrownianLayer();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Additive Map"))
            {
                terrain.GenerateLayeredFractalBrownian(true);
            }
            if (GUILayout.Button("Generate Map"))
            {
                terrain.GenerateLayeredFractalBrownian(false);
            }
            EditorGUILayout.EndHorizontal();
            EndVertical();
        }
    }

    private void Draw_Voronoi()
    {
        showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Veronoi (Lecture 21)");
        if (showVoronoi)
        {
            BeginVertical();
            GUILayout.Label("Generate Peak Using Veronoi", EditorStyles.boldLabel);

            EditorGUILayout.Slider(fallOff, 0.001f, 10.0f, new GUIContent("Falloff"));
            EditorGUILayout.Slider(dropOff, 0.1f, 10.5f, new GUIContent("Dropoff"));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Additive Peak"))
            {
                terrain.Voronoi(true);
            }
            if (GUILayout.Button("Generate Peak"))
            {
                terrain.Voronoi(false);
            }
            EditorGUILayout.EndHorizontal();
            EndVertical();
        }
    }

    private void Draw_MidpointDisplacement()
    {
        showMidpointDisplacement = EditorGUILayout.Foldout(showMidpointDisplacement, "Midpoint Displacement (Lecture 27)");
        if (showMidpointDisplacement)
        {
            BeginVertical();
            GUILayout.Label("Generate Midpoint Displacement", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(MpDHeightMin);
            EditorGUILayout.PropertyField(MpDHeightMax);
            EditorGUILayout.PropertyField(MpDHeightDampPower);
            EditorGUILayout.PropertyField(MpDRoughness);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Additive Map"))
            {
                terrain.MidpointDisplacement(true);
            }
            if (GUILayout.Button("Generate Map"))
            {
                terrain.MidpointDisplacement(false);
            }
            EditorGUILayout.EndHorizontal();
            EndVertical();
        }
    }

    private void Draw_Splatmap()
    {
        showSplatmap = EditorGUILayout.Foldout(showSplatmap, "Splatmap (Lecture 32)");
        if (showSplatmap)
        {
            BeginVertical();
            GUILayout.Label("Splatmap", EditorStyles.boldLabel);

            splatMapTable = GUITableLayout.DrawTable(splatMapTable, splatLayers);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.Slider(splatOffset, 0.0f, 0.25f, new GUIContent("Offset"));
            EditorGUILayout.Slider(splatNoiseXScale, 0.0001f, 0.50f, new GUIContent("X Scale"));
            EditorGUILayout.Slider(splatNoiseYScale, 0.0001f, 0.50f, new GUIContent("Y Scale"));
            EditorGUILayout.Slider(splatNoiseScaler, 0.0001f, 0.50f, new GUIContent("Noise Scale"));

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Layer")) terrain.AddSplatLayer();
            if (GUILayout.Button("Remove Layer")) terrain.RemoveSplatLayer();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply Splatmap"))
            {
                terrain.ApplySplatmap();
            }
            EditorGUILayout.EndHorizontal();
            EndVertical();
        }
    }

    private void Draw_Erosion()
    {
        showErosion = EditorGUILayout.Foldout(showErosion, "Erosion");
        if (showErosion)
        {
            EditorGUILayout.PropertyField(erosionType);
            EditorGUILayout.Slider(erosionStrength, 0, 1, new GUIContent("Erosion Strength"));
            EditorGUILayout.Slider(erosionAmount, 0, 1, new GUIContent("Erosion Amount"));
            EditorGUILayout.IntSlider(droplets, 0, 500, new GUIContent("Droplets"));
            EditorGUILayout.Slider(solubility, 0.001f, 1, new GUIContent("Solubility"));
            EditorGUILayout.IntSlider(springsPerRiver, 0, 20, new GUIContent("Springs Per River"));
            EditorGUILayout.IntSlider(erosionSmoothAmount, 0, 10, new GUIContent("Smooth Amount"));

            if (GUILayout.Button("Erode"))
            {
                terrain.Erode();
            }
        }
    }

    private void BeginVertical()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
    }

    private void EndVertical()
    {
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    private void BeginHorizontal(bool box)
    {
        if (box) EditorGUILayout.BeginHorizontal("Box");
        else EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
    }

    private void EndHorizontal()
    {
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }
}
