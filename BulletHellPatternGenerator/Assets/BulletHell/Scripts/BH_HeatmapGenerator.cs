using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BulletHellGenerator.Heatmap
{
    public class BH_HeatmapGenerator : MonoBehaviour
    {
        public Heatmap map;

        [HideInInspector]
        public Texture2D tex;

        public RawImage HeatmapImage;

        public Camera MainCam;

        [Range(1,10)]
        [Tooltip("The scale of the heatmap textures, Width and Height divided by downscale.")]
        public int Downscale = 4;

        public Gradient HeatMapGradiant = new Gradient();

        public void Start()
        {
            map = new Heatmap(Mathf.Max(2,Mathf.Abs(Screen.width / Downscale)), Mathf.Max(2, Mathf.Abs(Screen.height / Downscale)));
        }

        public void Update()
        {
            map.UpdateTex(HeatMapGradiant);

            if(HeatmapImage != null)
            {
                HeatmapImage.texture = map.Texture;
            }
        }

        public void AddHeatWorldPos(Vector3 worldPos)
        {
            worldPos = MainCam.WorldToScreenPoint(worldPos);

            // Normalize the position
            worldPos.x /= (float)MainCam.pixelWidth;
            worldPos.y /= (float)MainCam.pixelHeight;

            worldPos.x *= map.Texture.width;
            worldPos.y *= map.Texture.height;

            AddHeat((int)worldPos.x, (int)worldPos.y);
        }

        public void AddHeat(int x, int y)
        {
            map.AddHeat(x,y,1);
            map.AddHeat(x+1,y,3);
            map.AddHeat(x-1,y,3);
            map.AddHeat(x,y+1,3);
            map.AddHeat(x,y-1,3);
        }
    }

    [System.Serializable]
    public class Heatmap
    {
        public Texture2D Texture;
        private byte[] heatMap;

        int mWidth = 0;
        int mHeight = 0;

        public Heatmap(int width, int height)
        {
            mWidth = width;
            mHeight = height;

            heatMap = new byte[Mathf.Abs(width * height)];

            Texture = new Texture2D(width, height);
            //Texture.filterMode = FilterMode.Point;

            for (int i = 0; i < heatMap.Length; i++)
            {
                heatMap[i] = 0;
            }

        }

        private int GetMapPos(int x, int y) { return (y * mWidth) + x; }

        public void AddHeat(int x, int y, byte heat) 
        {
            if (GetMapPos(x, y) > heatMap.Length-1 || GetMapPos(x, y) < 0) return;
            heatMap[GetMapPos(x, y)] = (byte)Mathf.Min(heatMap[GetMapPos(x, y)] + heat,byte.MaxValue);
        }

        public void UpdateTex(Gradient heatmapGradiant)
        {
            float nTime = 0;
            for(int i = 0; i < heatMap.Length; i++)
            {
                nTime = heatMap[i] / (float)byte.MaxValue;
                Texture.SetPixel(i % mWidth, i / mWidth, heatmapGradiant.Evaluate(nTime));
            }

            Texture.Apply();
        }

    }
}