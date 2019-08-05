using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using FireFighter.DataModels;

namespace FireFighter.Commands
{
    public class Commands : Script
    {

        // Get your current coordinates and rotation
        [Command("loc")]
        public void Loc(Client sender)
        {
            NAPI.Util.ConsoleOutput(sender.Position.ToString());
            sender.SendChatMessage(sender.Position.ToString());
            sender.SendChatMessage(sender.Rotation.ToString());
        }

        // To stop the fire
        [Command("stopfireid")]
        public void StopFireById(Client sender)
        {

            if (NAPI.Data.HasEntityData(sender, "FireModel"))
            {
                NAPI.ClientEvent.TriggerClientEvent(sender, "StopFireById");
                NAPI.Data.ResetEntityData(sender, "FireModel");
            }

        }

        // To stop the fire
        [Command("obj")]
        public void getobj(Client sender)
        {

            NAPI.ClientEvent.TriggerClientEvent(sender, "GetObj",sender);

        }



    }
}
