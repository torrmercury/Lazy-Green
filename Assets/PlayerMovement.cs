using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{

    Vector3 destination = new Vector3 (5,0,5);
    float stoppingDistance = 0f;
    bool hitdest = false;

    // Use this for initialization
    void Start()
    {

    }

    void FixedUpdate()
    {
        float moveSpeed = (transform.position.x * transform.position.x) + (transform.position.z * transform.position.z);
        //int roundedMoveSpeed = (int) moveSpeed;

        if (hitdest)
        {
            // take distance to our destination; if further than __, then keep moving towards it
            if (Vector3.Distance(transform.position, destination) > stoppingDistance)
            {
                transform.position += (Vector3.Normalize(destination - transform.position) * moveSpeed)*Time.deltaTime;
            }
            else
            {
                // if we should stop, then add force in opposite direction of current velocity (brakes)
                //rigidbody.AddForce(-rigidbody.velocity);
            }
            if (transform.position == destination)
            {
                hitdest = false;
            }
        }        
    }

    // Update is called once per frame
    void Update()
    {

        // left-click sets new destination using raycast
        if (Input.GetMouseButtonDown(0))
        {
            // we only have to prep raycast IF the player left-clicked...

            // first, project mouse cursor onto camera matrix
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // initialize Raycast impact info variable
            RaycastHit rayHit = new RaycastHit();

            if (Physics.Raycast(mouseRay, out rayHit, 10000f))
            {
                destination = rayHit.point;
                hitdest = true;
            }
        }

    }

}
