using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private int levelID;

    public int LevelID { get => levelID; set => levelID = value; }
}
