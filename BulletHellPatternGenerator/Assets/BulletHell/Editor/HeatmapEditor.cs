using BulletHellGenerator.Heatmap;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BH_HeatmapGenerator))]
public class HeatmapEditor : Editor
{
    BH_HeatmapGenerator heatmap;

    private void OnEnable()
    {
        heatmap = (BH_HeatmapGenerator)target;

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Save Bullet Heatmap"))
            {
                //Add save buttons
                if (heatmap.BulletMap.Texture != null)
                {
                    heatmap.BulletMap.Texture.Apply();
                    byte[] data = heatmap.BulletMap.Texture.EncodeToPNG();

                    string path = EditorUtility.SaveFilePanel("Save Heatmap texture as", "", "bulletmap.png", ".png");

                    if (path.Length != 0)
                    {
                        File.WriteAllBytes(path, data);
                    }
                }
            }

            if (GUILayout.Button("Save Player Heatmap"))
            {
                //Add save buttons
                if (heatmap.PlayerMap.Texture != null)
                {
                    heatmap.PlayerMap.Texture.Apply();
                    byte[] data = heatmap.PlayerMap.Texture.EncodeToPNG();

                    string path = EditorUtility.SaveFilePanel("Save Heatmap texture as", "", "playermap.png", ".png");

                    if (path.Length != 0)
                    {
                        File.WriteAllBytes(path, data);
                    }
                }
            }

            if (GUILayout.Button("Save Both Heatmaps"))
            {
                //Add save buttons
                if (heatmap.BulletMap.Texture != null && heatmap.PlayerMap.Texture != null)
                {
                    heatmap.BulletMap.Texture.Apply();
                    heatmap.PlayerMap.Texture.Apply();

                    Texture2D newTex = new Texture2D(heatmap.BulletMap.Texture.width, heatmap.BulletMap.Texture.height);

                    for(int x = 0; x < newTex.width; x++)
                    {
                        for (int y = 0; y < newTex.height; y++)
                        {
                            newTex.SetPixel(x,y,heatmap.BulletMap.Texture.GetPixel(x,y) + (heatmap.PlayerMap.Texture.GetPixel(x, y) * heatmap.PlayerMap.Texture.GetPixel(x, y).a));
                        }
                    }

                    newTex.Apply();

                    byte[] data = newTex.EncodeToPNG();

                    string path = EditorUtility.SaveFilePanel("Save Heatmap texture as", "", "playerandbulletmap.png", ".png");

                    if (path.Length != 0)
                    {
                        File.WriteAllBytes(path, data);
                    }
                }
            }
        }
    }
}
