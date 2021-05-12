using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BulletHellGenerator.Heatmap
{
    public class BH_HeatmapGenerator : MonoBehaviour
    {
        public Heatmap BulletMap;
        public Heatmap PlayerMap;

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
        public Transform PlayerTransform;

        [Range(0.1f,1f)]
        public float SampleEveryXSeconds = 0.25f;

        private float sampleTimer = 0;

        [Header("Heatmap Settings")]
        [Range(1,100)]
        [Tooltip("How many pixel width and height one unit is.")]
        public int UnitPixelScale = 8;

        public Gradient BulletMapGradiant = new Gradient();
        public Gradient PlayerMapGradiant = new Gradient();

        public void Start()
        {
            if (SampleSpace != null)
            {
                BulletMap = new Heatmap((int)SampleSpace.bounds.size.x * UnitPixelScale, (int)SampleSpace.bounds.size.x * UnitPixelScale);
                PlayerMap = new Heatmap((int)SampleSpace.bounds.size.x * UnitPixelScale, (int)SampleSpace.bounds.size.x * UnitPixelScale);
            }
            else
            {
                Debug.LogWarning("A Sample Space has not been assigned, no heatmap will generate!");
            }

            if(use3DColliders)
            {

            }
        }
        
        public void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(SampleSpace.bounds.center, SampleSpace.bounds.size);
        }

        public void Update()
        {
            sampleTimer += Time.deltaTime;

            if(SampleSpace != null && sampleTimer >= SampleEveryXSeconds)
            {
                //SamplePlayer
                if(PlayerTransform != null)
                {
                    Vector3 pos = SampleSpace.ClosestPoint(PlayerTransform.position) - SampleSpace.bounds.min;
                    pos *= UnitPixelScale;
                    AddHeat(ref PlayerMap, (int)pos.x, (int)pos.y);

                    PlayerMap.UpdateTex(PlayerMapGradiant);
                }

                sampleTimer -= SampleEveryXSeconds;
                if(use3DColliders) // Detect 3D Bullets
                {
                    Collider[] DetectedColliders;

                    DetectedColliders = Physics.OverlapBox(SampleSpace.bounds.center, SampleSpace.bounds.extents,Quaternion.identity, BulletLayer);

                    if (DetectedColliders != null)
                    {
                        //Loop through all colliders
                        for (int i = 0; i < DetectedColliders.Length; i++)
                        {
                            //Convert Bullet pos to collider pos
                            Vector3 pos = transform.InverseTransformPoint(SampleSpace.ClosestPoint(DetectedColliders[i].transform.position) - SampleSpace.bounds.min);
                            pos *= UnitPixelScale;
                            AddHeat(ref BulletMap, (int)pos.x, (int)pos.y);
                        }
                    }
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
                            AddHeat(ref BulletMap,(int)pos.x,(int)pos.y);
                        }
                    }
                }

                BulletMap.UpdateTex(BulletMapGradiant);
                if (HeatmapImage != null)
                {
                    BulletMap.Texture.Apply();
                    HeatmapImage.texture = BulletMap.Texture;
                }
            }
        }

        public void AddHeatWorldPos(ref Heatmap map,Vector3 worldPos)
        {
            worldPos = MainCam.WorldToScreenPoint(worldPos);

            // Normalize the position
            worldPos.x /= (float)MainCam.pixelWidth;
            worldPos.y /= (float)MainCam.pixelHeight;

            worldPos.x *= BulletMap.Texture.width;
            worldPos.y *= BulletMap.Texture.height;

            AddHeat(ref map, (int)worldPos.x, (int)worldPos.y);
        }

        public void AddHeat(ref Heatmap map,int x, int y)
        {
            map.AddHeat(x,y,3);
            map.AddHeat(x+1,y,1);
            map.AddHeat(x-1,y,1);
            map.AddHeat(x,y+1,1);
            map.AddHeat(x,y-1,1);
        }
    }

    [System.Serializable]
    public class Heatmap
    {
        [HideInInspector]
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
        }

    }
}