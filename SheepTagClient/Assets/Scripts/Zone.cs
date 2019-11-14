using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    private enum PlayerCondition
    {
        NONE,
        SAFE,
        CAPTURED
    }
    [SerializeField] private PlayerCondition Condition = PlayerCondition.NONE;
    [SerializeField] private List<PlayerRPC> previousFoundRPCs = new List<PlayerRPC>();

    private void FixedUpdate()
    {
        float radius = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        Collider2D[] overlappedCols = Physics2D.OverlapCircleAll(transform.position, radius);
        List<PlayerRPC> rpcsNotFound = new List<PlayerRPC>(previousFoundRPCs);
        foreach (Collider2D col in overlappedCols)
        {
            PlayerRPC prpc = col.GetComponent<PlayerRPC>();
            if (prpc)
            {
                if (!rpcsNotFound.Contains(prpc))
                {
                    prpc.SetCondition(ConditionToString(Condition));
                    previousFoundRPCs.Add(prpc);
                }
                else
                {
                    rpcsNotFound.Remove(prpc);
                }
            }
        }
        while(rpcsNotFound.Count > 0)
        {
            rpcsNotFound[0].SetCondition(ConditionToString(PlayerCondition.NONE));
            rpcsNotFound.RemoveAt(0);
        }
    }

    private string ConditionToString(PlayerCondition cond)
    {
        switch(cond)
        {
            case PlayerCondition.NONE:
                return "NONE";
            case PlayerCondition.SAFE:
                return "SAFE";
            case PlayerCondition.CAPTURED:
                return "CAPTURED";
            default:
                return "";

        }
    }

    private void OnDrawGizmos()
    {
        if(Condition == PlayerCondition.CAPTURED)
        {
            Gizmos.color = Color.yellow;
        }
        else if(Condition == PlayerCondition.SAFE)
        {
            Gizmos.color = Color.blue;
        }

        float radius = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
