using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
   [SerializeField] private Health health;
   [SerializeField] private GameObject unitPrefab;
   [SerializeField] private Transform unitSpawnPoint;

   #region  Server

   public override void OnStartServer()
   {
      health.ServerOnDie += ServerHandleDie;
   }

   public override void OnStopServer()
   {
      health.ServerOnDie -= ServerHandleDie;
   }

   [Server]
   private void ServerHandleDie()
   {
      NetworkServer.Destroy(gameObject);
   }
   
   [Command]
   private void CmdSpawnUnit()
   {
      GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
      
      NetworkServer.Spawn(unitInstance, connectionToClient);
   }

   #endregion

   #region Client

   public void OnPointerClick(PointerEventData eventData) //Call this funciton if clicked on the object on which this script is attatched
   {
      if (eventData.button != PointerEventData.InputButton.Left)
      {
         return;
      }

      if (!isOwned)
      {
         return;
      }
      
      CmdSpawnUnit();
   }

   #endregion

   
}
