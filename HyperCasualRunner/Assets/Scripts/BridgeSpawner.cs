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
        float distance = direction.magnitude; // vector a��rl��� yani bile�kesi
        direction = direction.normalized; // normalize birim vectore d�n��t�r�yoruz. yani y�n� buluyoruz
        hiddenPlatform.transform.forward = direction; // gizli platforma y�n�n� s�yl�yoruz

        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance); // colliderin z size ini ayarl�yoruz.

        hiddenPlatform.transform.position = startReference.transform.position + (direction * distance / 2) +
            (new Vector3(0, -direction.z, direction.y) * hiddenPlatform.size.y / 2); //collider in tam yerini ayarl�yoruz. start ve end in tam ortas�na gelmeli
    }
}
