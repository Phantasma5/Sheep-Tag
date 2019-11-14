using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    #region References

    #endregion
    #region Variables
    [HideInInspector] public float speed = 5;
    /*[HideInInspector]*/ public bool captured = false;
    #endregion
    private void Awake()
    {
        if (GetComponent<NetworkSync>().owned)
        {
            References.localPlayer = this.gameObject;
        }
    }
    
    private void PlayerMovement()
    {
        if(captured)
        {
            return;
        }
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime, 0);
        transform.position += movement;

        //Check if the player is within 10 from origin box
        //if(transform.position.x < 2 &&
        //    transform.position.x > -2 &&
        //    transform.position.y < 2 &&
        //    transform.position.y > -2)
        //{
        //    GetComponent<PlayerRPC>().Rescue();
        //}
    }
    void Update()
    {
        if (GetComponent<NetworkSync>().owned)
        {
            PlayerMovement();
        }
    }
}
