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

        //FE
        [Header("Detection Settings")]
        public BoxCollider SampleSpace;
        public LayerMask BulletLayer;
        public LayerMask PlayerLayer;
        public bool use3DColliders = false;

        [Range(0.1f,1f)]
        public float SampleEveryXSeconds = 0.25f;

        private float sampleTimer = 0;

        [Header("Heatmap Settings")]
        [Range(1,100)]
        [Tooltip("How many pixel width and height one unit is.")]
        public int UnitPixelScale = 16;

        public Gradient HeatMapGradiant = new Gradient();

        public void Start()
        {
            if (SampleSpace != null)
            {
                map = new Heatmap((int)SampleSpace.bounds.size.x * UnitPixelScale, (int)SampleSpace.bounds.size.x * UnitPixelScale);
            }
            else
            {
                Debug.LogWarning("A Sample Space has not been assigned, no heatmap will generate!");
            }
        }

        public void Update()
        {
            //map.UpdateTex(HeatMapGradiant);

            //if(HeatmapImage != null)
            //{
            //    HeatmapImage.texture = map.Texture;
            //}
            sampleTimer += Time.deltaTime;

            if(SampleSpace != null && sampleTimer >= SampleEveryXSeconds)
            {
                sampleTimer -= SampleEveryXSeconds;
                if(use3DColliders) // Detect 3D Bullets
                {
                    Collider[] DetectedColliders;
                }
                else // Detect 2D bullets
                {
                    Collider2D[] DetectedColliders;

                    DetectedColliders = Physics2D.OverlapAreaAll(SampleSpace.bounds.min, SampleSpace.bounds.max,BulletLayer);

                    if(DetectedColliders != null)
                    {
                        //Loop through all colliders
                        for(int i = 0; i < DetectedColliders.Length; i++)
                        {
                            //Convert Bullet pos to collider pos
                            Vector3 pos = SampleSpace.ClosestPoint(DetectedColliders[i].transform.position) - SampleSpace.bounds.min;
                            pos *= UnitPixelScale;
                            AddHeat((int)pos.x,(int)pos.y);
                        }
                    }
                }

                map.UpdateTex(HeatMapGradiant);
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