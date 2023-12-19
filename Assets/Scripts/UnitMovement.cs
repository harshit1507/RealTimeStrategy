using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter;
    [SerializeField] private float chaseRange = 10f;
    
    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > (chaseRange * chaseRange))  //Distance between self and target > chaseRange | It is faster that way becasue calculating square root is slower
            {
                //chase
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                //stop chase
                agent.ResetPath();
            }
            
            return;
        }
        
        if (!agent.hasPath)
            return;
        
        if (agent.remainingDistance > agent.stoppingDistance)
            return;
        
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    [Server]
    public void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }
        agent.SetDestination(hit.position);
    }
    
    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }
    #endregion
    
    
}
