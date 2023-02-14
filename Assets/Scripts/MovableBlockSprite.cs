using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBlockSprite : MonoBehaviour
{
    private GameObject blockAttachedTo;

    public GameObject BlockAttachedTo
    {
        set => blockAttachedTo = value;
        get => blockAttachedTo;
    }
}
