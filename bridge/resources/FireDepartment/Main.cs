using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using FireFighter.DataModels;
using System.Threading.Tasks;
using System.Threading;
using FireFighter.VehicleHandler;


namespace FireFighter
{
   public class Main : Script
    {
        public const string BlipName = "Fire Department";
        public const uint BlipId = 436; // Flame blip icon
        public const uint BlipColorId = 1; // Red
        public Vector3 BlipPos = new Vector3(206, -1652, 30); // Fire department in gta v 
        public static Vector3 MissionStartColshapePos = new Vector3(197.6139, -1651.797, 28.9); // The colshape position to start the job
        public Vector3 HealColshapePos = new Vector3(209.3157, -1657.908, 28.9); // the colshape where you can heal
        public static Vector3 VehSpawnPos = new Vector3(208.6827, -1648.25, 29.80321); // the vehicle spawn position
        public static float VehSpawnRotation = 315; // vehicle spawn rotation
        

     [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            // create world blip
            NAPI.Blip.CreateBlip(BlipId, BlipPos, 1,Convert.ToByte(BlipColorId), BlipName);
            // Create Job clopshape
            ColShape missionColshape = NAPI.ColShape.Create2DColShape(MissionStartColshapePos.X, MissionStartColshapePos.Y, 3f, 1);
            ColShape healColshape = NAPI.ColShape.Create2DColShape(HealColshapePos.X, HealColshapePos.Y, 3f, 1);

            Marker missionMarker = NAPI.Marker.CreateMarker(1, MissionStartColshapePos, new Vector3(), new Vector3(), 1.5f, new Color(204, 229, 255, 155));
            Marker healMarker = NAPI.Marker.CreateMarker(1, HealColshapePos, new Vector3(), new Vector3(), 1.5f, new Color(0, 204, 0, 155));

            TextLabel MissionLabel = NAPI.TextLabel.CreateTextLabel("start1", new Vector3(MissionStartColshapePos.X, MissionStartColshapePos.Y, MissionStartColshapePos.Z + 1), 3, 3, 2, new Color(0, 204, 0, 155));
            TextLabel HealLabel = NAPI.TextLabel.CreateTextLabel("Fireman can heal here", new Vector3(HealColshapePos.X, HealColshapePos.Y, HealColshapePos.Z + 1), 3, 3, 2, new Color(204, 229, 255, 155));

            // Set The current colshape data to FireDepartment, so if you create more jobs you know witch one you standing on
            NAPI.Data.SetEntityData(missionColshape, "Job", "FireDepartment");
            // Set the colshape as heal point
            NAPI.Data.SetEntityData(healColshape, "HealPoint", true);

            NAPI.Util.ConsoleOutput("Create Mission Colshape at position:" + MissionStartColshapePos);
            NAPI.Util.ConsoleOutput("Create Heal Colshape at position:" + HealColshapePos);

        }

        // List where fire can start
        public static List<Vector3> HouseList = new List<Vector3>()
        {
            new Vector3(122.3308,-1643.58,29.35815),
            new Vector3(14.54701,-1668.064,29.1899),
            new Vector3(41.39061,-1605.78,29.48604),
            new Vector3(-28.32198,-1527.433,30.47462),
            new Vector3(222.8359,-1702.369,29.69692),
            new Vector3(268.318, -1712.322, 29.66747)
        };

        // create the fire properties
        public FireModel GenerateFireModel(Client player)
        {
            Random randomNum = new Random();
            bool gasPowerd = true;

            int maxChildren = randomNum.Next(10, 25);
            int gas = randomNum.Next(1, 3);
            int fireAmount = randomNum.Next(20, 40);

            player.SendChatMessage("gas random:" + gas);

            if (gas > 1) gasPowerd = true;
            else
            {
                gasPowerd = false;
            }

            // select random house from list
            Vector3 HousePos = HouseList[randomNum.Next(0, HouseList.Count)];
            
            FireModel model = new FireModel(HousePos.X, HousePos.Y, HousePos.Z, maxChildren, fireAmount, gasPowerd);

            return model;
        }
        
        // if the player enter /start in the chat 
        [Command("start1")]
        public void StartMission(Client sender)
        {
            // if the player stand on colshape and has atJob data
            if (NAPI.Data.HasEntityData(sender,"atJob"))
            {

                // get the job data from the player
                string jobName = NAPI.Data.GetEntityData(sender, "atJob");

                switch (jobName)
                {
                    case "FireDepartment":

                        // set the player skin to fireman
                        sender.SetSkin(PedHash.Fireman01SMY);
                        
                        // give fire extinguiser
                        sender.GiveWeapon(WeaponHash.FireExtinguisher, 9999);

                        // if the player dont have JobVehicle data then create the vehicle
                        if (!NAPI.Data.HasEntityData(sender, "JobVehicle"))
                        {
                            // create the firetruck
                            Vehicle jobVeh = VehicleHandler.VehicleHandler.CreateVehicle(sender, VehicleHash.FireTruck, VehSpawnPos, VehSpawnRotation, 123, 123, "Fire_Deprt");
                            // set the vehicle data on the player
                            NAPI.Data.SetEntityData(sender, "JobVehicle", jobVeh);
                        }
                        // create the fire for all online players
                        FireModel fireModel = BuildFire(sender);
                        // Start the fires alive update timer (javascript)
                        NAPI.ClientEvent.TriggerClientEvent(sender, "FiresAliveTimer", fireModel.xPos, fireModel.yPos, fireModel.zPos);
                        // Set the fire model data to the player (fireman)
                        NAPI.Data.SetEntityData(sender, "FireModel", fireModel);
                        break;
                    // if job is not FireDepartment then do noting
                    default:
                        break;
                }
            }
        }

        // Create the fire 
        public FireModel BuildFire(Client player)
        {
            // create a firemodel (setting fire properties)
            FireModel fireModel = GenerateFireModel(player);

            // Create a blip where the fire starts
            NAPI.ClientEvent.TriggerClientEvent(player, "BlipFireLocation", fireModel.xPos, fireModel.yPos, fireModel.zPos);

            // get all players online
            List<Client> allCurrentPlayers = NAPI.Pools.GetAllPlayers();

            int count = 0;
            
            Random amount = new Random();

            // start the fire foreach online player
            foreach (Client playerOnline in allCurrentPlayers)
            {
                while (count < fireModel.AmountFires)
                {
                    // check the position if its reachable for the player 
                    // We also add a random number to the position so we can spread the fire
                    NAPI.ClientEvent.TriggerClientEvent(player, "CheckIfReachable", new Vector3(fireModel.xPos, fireModel.yPos, fireModel.zPos), fireModel.xPos + amount.Next(5, 20), fireModel.yPos + amount.Next(5, 20), fireModel.zPos + amount.Next(0, 1), fireModel.maxChilderen, fireModel.gasPowerd);
                    count++;
                }
            }
            // Send a notification to the player with the fire properies
            NAPI.Notification.SendNotificationToPlayer(player, "Fire Strenght:" + fireModel.maxChilderen + Environment.NewLine + "Gas powerd:" + fireModel.gasPowerd, false);

            return fireModel;
        }


        // if the player enter /teleport in the chat 
        [Command("teleport")]
        public void Teleport(Client sender)
        {
            // set the player to the fire department start location
            NAPI.Entity.SetEntityPosition(sender, MissionStartColshapePos);           
        }

        
    }
}
