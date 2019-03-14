using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TextureGenerator {

    static bool done = false;

    // Returns a height map in the form of a texture.
    public static Texture2D TextureFromHeightMap(float[,] heightMap, FilterMode filterMode)
    {
        // Initialise the width and height of the new texture
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        
        // Initialise the colour spectrum for the new texture
        Color[] colourMap = new Color[width * height];

        // Loop the texture dimensions
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Assign a normalised colour, from black to white, depending on the value of  height map at this texture co-ord
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        // Return the new texture, utilising the TextureFromColourMap() method.
        return TextureFromColourMap(colourMap, width, height, filterMode);
    }

    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height, FilterMode filterMode)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = filterMode;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        
        return texture;
    }
}
