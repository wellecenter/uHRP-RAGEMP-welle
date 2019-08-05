using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace FireFighter.VehicleHandler
{
   public class VehicleHandler : Script
    {

        // create the job vehicle
        public static Vehicle CreateVehicle(Client player, VehicleHash hash,Vector3 spawnPos, float rot,int color1, int color2,string plate)
        {
            Vehicle jobVeh = NAPI.Vehicle.CreateVehicle(hash, spawnPos, rot, color1, color2, plate);

            return jobVeh;
        }

    }
}
