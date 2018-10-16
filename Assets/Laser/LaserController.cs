// Code to control the lasers fired by the Giant Seagull in the game developed 
// by Adam Turner, Amie Xie & Shevon Mendis for Project 02 of COMP30019.
//
// Written by Shevon Mendis, September 2018.

using UnityEngine;

public class LaserController : MonoBehaviour
{

    public GameObject smallExplosionPrefab;
    public GameObject largeExplosionPrefab;
    public Vector3 direction;
    public int explosionType; // 0 for small explosion (default)
                              // 1 for large explosion

    const float velocity = 2.0f;
    private bool isExploded;
    
    void Update()
    {
        // move lasers in direction of target
        transform.Translate(direction * Time.deltaTime * velocity, Space.World);

        // code to combat tunneling
        if (transform.position.y <= -12){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if ((col.gameObject.CompareTag("Terrain") || col.gameObject.CompareTag("Player")) && !isExploded)
        {
            isExploded = true;
            Destroy(gameObject);
            GameObject explosion;
            explosion = explosionType == 0 ? Instantiate(smallExplosionPrefab) : Instantiate(largeExplosionPrefab);
            explosion.transform.position = transform.position;
        }
    }
}
