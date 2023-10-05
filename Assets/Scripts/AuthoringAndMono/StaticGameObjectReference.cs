using AxieRescuer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticGameObjectReference : MonoBehaviour
{
    public static GameObject Player;
    public static List<GameObject> ZombiePrefabs;
    public List<GameObject> ZombiePrefabList = new List<GameObject>();
    public static List<GameObject> AxiePrefabs;
    public List<GameObject> AxiePrefabList = new List<GameObject>();

    public void Awake()
    {
        ZombiePrefabs = ZombiePrefabList;
        AxiePrefabs = AxiePrefabList;
    }
}
