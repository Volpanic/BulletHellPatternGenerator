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

        [HideInInspector]
        public Camera mainCam;

        [Range(1,10)]
        [Tooltip("The scale of the heatmap textures, Width and Height divided by downscale.")]
        public int Downscale = 4;

        public void Start()
        {
            map = new Heatmap(Mathf.Max(2,Mathf.Abs(Screen.width / Downscale)), Mathf.Max(2, Mathf.Abs(Screen.height / Downscale)));
            mainCam = Camera.main;
        }

        public void Update()
        {
            tex = map.GetTexture2D();

            if(HeatmapImage != null)
            {
                HeatmapImage.texture = tex;
            }
        }

        public void AddHeatWorldPos(Vector3 worldPos)
        {
            worldPos = mainCam.WorldToScreenPoint(worldPos);
            worldPos /= Downscale;

            AddHeat((int)worldPos.x, (int)worldPos.y);
        }

        public void AddHeat(int x, int y)
        {
            map.AddHeat(x,y,  2);
            map.AddHeat(x+1,y,1);
            map.AddHeat(x-1,y,1);
            map.AddHeat(x,y+1,1);
            map.AddHeat(x,y-1,1);
        }
    }

    public class Heatmap
    {
        public Texture2D tax;

        private byte[] heatMap;

        int mWidth = 0;
        int mHeight = 0;

        public Heatmap(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            heatMap = new byte[Mathf.Abs(width * height)];

            for (int i = 0; i < heatMap.Length; i++)
            {
                heatMap[i] = 255;
            }

        }

        private int GetMapPos(int x, int y) { return (y * mWidth) + x; }

        public void SetHeat(int x, int y, byte heat) { heatMap[GetMapPos(x, y)] = heat; }
        public void AddHeat(int x, int y, byte heat) 
        {
            if (GetMapPos(x, y) > heatMap.Length-1 || GetMapPos(x, y) < 0) return;
            heatMap[GetMapPos(x, y)] = (byte)(heatMap[GetMapPos(x, y)] - heat);
        }

        public Texture2D GetTexture2D()
        {
            Texture2D tex = new Texture2D(mWidth, mHeight);
            tex.filterMode = FilterMode.Point;

            for (int i = 0; i < heatMap.Length; i++)
            {
                float g = heatMap[i] / byte.MaxValue;
                tex.SetPixel(i % mWidth, i / mWidth, new Color(g, g, g, 1));
            }

            tex.Apply();
            return tex;
        }

    }
}