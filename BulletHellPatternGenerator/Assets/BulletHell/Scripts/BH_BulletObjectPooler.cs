using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHellGenerator
{
    public class BH_BulletObjectPooler : MonoBehaviour
    {
        public struct Pool
        {
            List<BH_Bullet> Objects;
            int pickerTicker;
            Transform create;

            private GameObject originalPrefab;

            public Pool(GameObject prefab, int count, Transform creator)
            {
                create = creator;
                originalPrefab = prefab;
                Objects = new List<BH_Bullet>();
                pickerTicker = 0;

                for (int i = 0; i < count; i++)
                {
                    GameObject P = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    P.transform.parent = creator;
                    P.SetActive(false);
                    Objects.Add(P.GetComponent<BH_Bullet>());

                }
            }

            public BH_Bullet GetPoolObject()
            {
                int count = 0;
                while (count < Objects.Count)
                {
                    if (!Objects[pickerTicker].gameObject.activeInHierarchy)
                    {
                        Objects[pickerTicker].gameObject.SetActive(true);
                        int oldTick = pickerTicker;
                        pickerTicker++;
                        if (pickerTicker >= Objects.Count) pickerTicker = 0;
                        return Objects[oldTick];
                    }

                    pickerTicker++;
                    if (pickerTicker >= Objects.Count) pickerTicker = 0;
                    count++;
                }

                //Create another one
                BH_Bullet P = Instantiate(originalPrefab, Vector3.zero, Quaternion.identity).GetComponent<BH_Bullet>();
                Objects.Add(P);
                P.transform.parent = create;
                pickerTicker = 0;
                return P;
            }
        }

        private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

        public void MakePool(GameObject prefab, int prePool)
        {
            if (prefab != null && !pools.ContainsKey(prefab.name))
            {
                pools[prefab.name] = new Pool(prefab, prePool, transform);
            }
        }

        public BH_Bullet PoolInstansiate(GameObject Prefab, Vector3 position, Quaternion rotation)
        {
            if (pools.ContainsKey(Prefab.name))
            {
                BH_Bullet obj = pools[Prefab.name].GetPoolObject();
                obj.transform.position = position;
                obj.transform.rotation = rotation;

                return obj;
            }
            else
            {
                MakePool(Prefab, 100);

                BH_Bullet obj = pools[Prefab.name].GetPoolObject();
                obj.transform.position = position;
                obj.transform.rotation = rotation;

                return obj;
            }
        }
    }

}