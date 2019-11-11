using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPosContainer : MonoBehaviour
{
    private Vector3 pos;
    private Quaternion rot;

    public GhostPosContainer(Vector3 pos, Quaternion rot)
    {
        this.pos = pos;
        this.rot = rot;
    }

    public Quaternion Rot { get => rot;}
    public Vector3 Pos { get => pos; }
}
