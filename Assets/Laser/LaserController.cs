// Code to control the lasers fired by the Giant Seagull in the game developed 
// by Adam Turner, Amie Xie & Shevon Mendis for Project 02 of COMP30019.
//
// Written by Shevon Mendis, September 2018.

using UnityEngine;

public class LaserController : MonoBehaviour
{

    public GameObject explosionPrefab;
    public Vector3 direction;

    const float velocity = 2.0f;
    bool exploded;

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

        if ((col.gameObject.tag == "Terrain" || col.gameObject.tag == "Player" || col.gameObject.tag == "ArenaWall") && !exploded)
        {
            exploded = true;

            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;

            Destroy(gameObject);
        }
    }
}
