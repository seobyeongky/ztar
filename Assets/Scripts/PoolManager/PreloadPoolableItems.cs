using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreloadPoolableItems : MonoBehaviour
{
    public List<Item> items = new List<Item>();

	void Start ()
    {
		foreach (var item in items)
        {
            Global.poolManager.Reserve(item.name, item.quantity);
        }
	}

    [System.Serializable]
    public struct Item
    {
        [PoolableItem(typeof(Transform))]
        public string name;
        public int quantity;
    }


}
