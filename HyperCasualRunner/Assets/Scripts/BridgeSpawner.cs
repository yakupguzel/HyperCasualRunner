using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject startReference, endReference;
    public BoxCollider hiddenPlatform;

    private void Start()
    {
        Vector3 direction = endReference.transform.position - startReference.transform.position;
        float distance = direction.magnitude; // vector aðýrlýðý yani bileþkesi
        direction = direction.normalized; // normalize birim vectore dönüþtürüyoruz. yani yönü buluyoruz
        hiddenPlatform.transform.forward = direction; // gizli platforma yönünü söylüyoruz

        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance); // colliderin z size ini ayarlýyoruz.

        hiddenPlatform.transform.position = startReference.transform.position + (direction * distance / 2) +
            (new Vector3(0, -direction.z, direction.y) * hiddenPlatform.size.y / 2); //collider in tam yerini ayarlýyoruz. start ve end in tam ortasýna gelmeli
    }
}
