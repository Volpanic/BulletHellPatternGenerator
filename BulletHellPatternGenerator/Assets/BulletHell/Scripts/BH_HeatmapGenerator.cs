using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHellGenerator.Heatmap
{
    public class BH_HeatmapGenerator : MonoBehaviour
    {
        public Heatmap map = new Heatmap(256, 256);
        public Texture2D tex;

        public void Awake()
        {
            tex = map.GetTexture2D();
        }

        public void SetHeat(int x, int y)
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

        }

        private int GetMapPos(int x, int y) { return (y * mWidth) + x; }

        public void SetHeat(int x, int y, byte heat) { heatMap[GetMapPos(x, y)] = heat; }
        public void AddHeat(int x, int y, byte heat) 
        {
            if (GetMapPos(x, y) >= heatMap.Length) return;
            heatMap[GetMapPos(x, y)] += heat;
        }

        public Texture2D GetTexture2D()
        {
            Texture2D tex = new Texture2D(mWidth, mHeight);

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