using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace FireFighter.Colshape
{
    public class ColshapeHandler : Script
    {

        // When a player enter a colshape Trigger this
        [ServerEvent(Event.PlayerEnterColshape)]
        public void OnPlayerEnterColshape(ColShape shape, Client player)
        {
            // if the colshape is a heal point
            if (NAPI.Data.HasEntityData(shape, "HealPoint"))
            {
                // if the player is a fireman 
                if (NAPI.Data.HasEntityData(player,"FireModel"))
                {
                    player.SendChatMessage("Healed");
                    player.Health = 100;                   
                }
                else
                {
                    player.SendChatMessage("Your not a fireman");
                }
                
            }

            // Check if the current colshape has Job data
            if (NAPI.Data.HasEntityData(shape,"Job"))
            {
                // Get the colshape Job data
                string jobName = NAPI.Data.GetEntityData(shape, "Job");

                switch(jobName)
                {                   
                    case "FireDepartment":

                        player.SendChatMessage("Entered Fire Department");

                        // Set the current colshape data to the player
                        // So we can later use it when we leave the colshape
                        NAPI.Data.SetEntityData(player, "atJob", jobName);
                        break;    
                        
                        // if you have more jobs
                    case "PoliceDepartment":
                        break;
                }

            }
        }

        // When a player leave a colshape Trigger this
        [ServerEvent(Event.PlayerExitColshape)]
        public void OnPlayerExitColshape(ColShape shape, Client player)
        {
            // Check if the current colshape has Job data
            if (NAPI.Data.HasEntityData(player, "atJob"))
            {
                // Get the colshape Job data
                string jobName = NAPI.Data.GetEntityData(player, "atJob");

                player.SendChatMessage("You left: " + jobName);

                // remove the data on the player so we know you no longer stand on the colshape
                NAPI.Data.ResetEntityData(player, "atJob");

            }
        }



    }
}
