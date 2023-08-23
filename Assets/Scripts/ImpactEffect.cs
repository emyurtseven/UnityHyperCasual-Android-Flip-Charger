using UnityEngine;
using System.Collections;

public class ImpactEffect : MonoBehaviour
{
    private void Start() 
    {
        float moveSpeed = GameManager.Instance.GameSpeed;
        GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, 0);
    }
}
