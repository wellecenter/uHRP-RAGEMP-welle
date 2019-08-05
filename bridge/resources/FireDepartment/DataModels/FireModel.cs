using System;

namespace FireFighter.DataModels
{
    public class FireModel
    {
        public float xPos { get; set; }
        public float yPos { get; set; }
        public float zPos { get; set; }
        public int maxChilderen { get; set; }
        public int AmountFires { get; set; }
        public bool gasPowerd { get; set; }


        public FireModel() { }

        public FireModel(float xPos, float yPos, float zPos, int maxChilderen,int amountFires, bool gasPowerd)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.zPos = zPos;
            this.AmountFires = amountFires;
            this.maxChilderen = maxChilderen;
            this.gasPowerd = gasPowerd;

        }

    }
}
