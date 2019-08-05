using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace FireFighter.RemoteEvents
{
   public class RemoteEvents : Script
    {

        // When all fires are out
        [RemoteEvent("FireComplete")]
        public void FiresCompleteEvent(Client player)
        {

            // get all players online
            List<Client> allCurrentPlayers = NAPI.Pools.GetAllPlayers();

            // foreach fireman
            foreach (Client playerOnline in allCurrentPlayers)
            {
                // for each online player remove the fire entity 
                NAPI.ClientEvent.TriggerClientEvent(playerOnline, "StopFireById");

                if (NAPI.Data.HasEntityData(playerOnline, "FireModel"))
                {
                    NAPI.Data.ResetEntityData(playerOnline, "FireModel");
                    NAPI.Notification.SendNotificationToPlayer(playerOnline, "The Fire is out!");


                }
            }

        }

    }
}
