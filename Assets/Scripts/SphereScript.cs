using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SphereScript : MonoBehaviour
{
    private GameObject ogPlayer;

    public GameObject OgPlayer
    {
        set { ogPlayer = value; }
        get { return ogPlayer; }
    }
}
