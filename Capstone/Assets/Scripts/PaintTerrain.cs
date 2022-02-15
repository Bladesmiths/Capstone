using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Paints terrain with textures based on height data
//Source: https://www.youtube.com/watch?v=aUcWm1k0xDc

//This script runs once and applies textures in the editor.
//It should not be running every time the game starts.

public class PaintTerrain : MonoBehaviour
{
    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public float startingHeight;
        public float stoppingHeight;
    }

    public SplatHeights[] splatHeights;

    // Paint terrain
    void Start()
    {
        //Get the current terrain's array of height values
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        float[, ,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        //Loop through all points on the terrain
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(y, x);

                float[] splat = new float[terrainData.terrainLayers.Length];

                //Find which specified height matches this point
                for (int i = 0; i < splatHeights.Length; i++)
                {

                    //Set the height-appropriate texture to be visible
                    if(terrainHeight >= splatHeights[i].startingHeight && terrainHeight <= splatHeights[i].stoppingHeight)
                    {
                        splat[splatHeights[i].textureIndex] = 1;
                    }
                }

                for (int j = 0; j < terrainData.terrainLayers.Length; j++)
                {
                    splatmapData[x, y, j] = splat[j];
                }
            }

            //Apply texture changes to terrain data
            terrainData.SetAlphamaps(0, 0, splatmapData);
        }
    }
}
