using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCoillder : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

        // create new Mesh Filter &amp; Mesh objects
        var meshFilter = GetComponent<MeshFilter>();
 
        // add collider! 
        gameObject.AddComponent<MeshCollider>();

        GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
    }
}
