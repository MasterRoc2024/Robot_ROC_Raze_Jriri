using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JririRazeInterface
{
    public class Robot
    {
        public string receivedText = "";
        public float distanceTelemetreDroit;
        public float distanceTelemetreCentre;
        public float distanceTelemetreGauche;
        public Queue<byte> ByteListReceived;

        //Constructeur
        public Robot()
        {
            ByteListReceived = new Queue<byte>();
        }


    }
}