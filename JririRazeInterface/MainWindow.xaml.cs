using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExtendedSerialPort_NS;
using System.IO.Ports;
using System.Windows.Threading;
using System.Runtime.ConstrainedExecution;
using System.Collections;
using System.Security.Cryptography;
using System.Drawing;
using System.Collections.Specialized;
using System.Security.Policy;
using KeyboardHook_NS;

namespace JririRazeInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public enum StateRobot
    {
        STATE_ATTENTE = 0,
        STATE_ATTENTE_EN_COURS = 1,
        STATE_AVANCE = 2,
        STATE_AVANCE_EN_COURS = 3,
        STATE_TOURNE_GAUCHE = 4,
        STATE_TOURNE_GAUCHE_EN_COURS = 5,
        STATE_TOURNE_DROITE = 6,
        STATE_TOURNE_DROITE_EN_COURS = 7,
        STATE_TOURNE_SUR_PLACE_GAUCHE = 8,
        STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS = 9,
        STATE_TOURNE_SUR_PLACE_DROITE = 10,
        STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS = 11,
        STATE_ARRET = 12,
        STATE_ARRET_EN_COURS = 13,
        STATE_RECULE = 14,
        STATE_RECULE_EN_COURS = 15
    }

    public enum StateReception
    {
        Waiting,
        FunctionMSB,
        FunctionLSB,
        PayloadLengthMSB,
        PayloadLengthLSB,
        Payload,
        CheckSum
    }

    public partial class MainWindow : Window
    {
        int MOTEUR_GAUCHE= 0;
        int MOTEUR_DROIT= 1;
        Brush initBrush;
        ExtendedSerialPort serialPort1;
        DispatcherTimer timerAffichage;
        public Robot robot;
        byte[] byteList;
        GlobalKeyboardHook KeyboardHook= new GlobalKeyboardHook();

        StateReception rcvState = StateReception.Waiting;
        int msgDecodedFunction = 0;
        int msgDecodedPayloadLength = 0;
        byte[] msgDecodedPayload;
        int msgDecodedPayloadIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            initBrush = buttonEnvoyer.Background;
            buttonEnvoyer.Click += ButtonEnvoyer_Click;
            buttonTest.Click += ButtonTest_Click;
            serialPort1 = new ExtendedSerialPort("COM7", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            buttonClear.Click += ButtonClear_Click;
            serialPort1.Open();
            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();
            robot = new Robot();
            byteList = new byte[20];
            foreach(var checkBox in LedPanel.Children)
            {
                ((CheckBox)checkBox).Click += CheckBox_Click;
            }
            ControlRobot.Click += ControlRobot_Click;
            KeyboardHook.KeyPressed += KeyboardHook_KeyPressed;


        }

        private void KeyboardHook_KeyPressed(object? sender, KeyArgs e)
        {
            switch(e.keyCode)
            {
                case KeyCode.LEFT:
                    UartEncodeAndSendMessage(0x0051, 1, 
                                                new byte[] { (byte)StateRobot.STATE_TOURNE_SUR_PLACE_GAUCHE });
                            break;
                case KeyCode.RIGHT:
                                UartEncodeAndSendMessage(0x0051, 1, new byte[] {
                (byte)StateRobot.STATE_TOURNE_SUR_PLACE_DROITE });
                break;
            case KeyCode.UP:
                UartEncodeAndSendMessage(0x0051, 1, new byte[]
                { (byte)StateRobot.STATE_AVANCE });
                break;
            case KeyCode.DOWN:
                UartEncodeAndSendMessage(0x0051, 1, new byte[]
                { (byte)StateRobot.STATE_ARRET });
                break;
            case KeyCode.PAGEDOWN:
                UartEncodeAndSendMessage(0x0051, 1, new byte[]
                { (byte)StateRobot.STATE_RECULE });
                break;

            }
        }

        private void ControlRobot_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)e.Source).IsChecked == true)
            {
                UartEncodeAndSendMessage(0x0052, 1, new byte[]
                { 0 });
            }
            else
            {
                UartEncodeAndSendMessage(0x0052, 1, new byte[]
                { 1 });
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)e.Source;
            var ledID = 0;
            if(checkBox.Content.Equals("Led Blanche"))
                ledID = 0;
            if (checkBox.Content.Equals("Led Bleue"))
                ledID = 1;
            if (checkBox.Content.Equals("Led Orange"))
                ledID = 2;
            byte[] payloadLed = new byte[2] { (byte)ledID, (byte)Convert.ToInt32((bool)checkBox.IsChecked) };
            UartEncodeAndSendMessage(0x0020,
            payloadLed.Length, payloadLed);
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxReception.Text = "";
        }

        private void TimerAffichage_Tick(object? sender, EventArgs e)
        {
            while(robot.ByteListReceived.Count > 0)
            {
                var c = new byte[1] { robot.ByteListReceived.Dequeue() };
                DecodeMessage(c[0]);
                //textBoxReception.Text += "0x" + c.ToString("X2") + " ";
            }

        }

        private void SerialPort1_DataReceived(object? sender, DataReceivedArgs e)
        {
            //robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            foreach (var value in e.Data)
            {
                robot.ByteListReceived.Enqueue(value);
            }
        }

        //Fonction d'Evenements (Interruptions)
        private void ButtonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            /*if (buttonEnvoyer.Background == initBrush)
                buttonEnvoyer.Background = Brushes.RoyalBlue;
            else 
                buttonEnvoyer.Background = initBrush;*/
            //SendMessage();
            //if (textBoxEmission.Text == "")
            //{
            /*for (int i = 0; i < byteList.Length; i++)
            {
                byteList[i] = (byte)(2*i);
            }
            serialPort1.Write(byteList, 0, byteList.Length);*/
            //}
            if (serialPort1.IsOpen == true)
            {
                //serialPort1.WriteLine(textBoxEmission.Text);
                UartEncodeAndSendMessage(128, textBoxEmission.Text.Length, Encoding.ASCII.GetBytes(textBoxEmission.Text));
                textBoxEmission.Text = "";
            }
            else
                throw new Exception("Envoi de data sur un port ferme");
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            //Transmission Texte
            /*var content = Encoding.ASCII.GetBytes(textBoxEmission.Text);
            UartEncodeAndSendMessage(0X0080, content.Length, content);
            //Reglage LED
            var leds = new byte[2] { (byte)7, 0X01 };
            UartEncodeAndSendMessage(0X0020, leds.Length, leds);*/
            //Reglage TELEMETRE

            //UartEncodeAndSendMessage(0X0020, content.Length, content);
            //Reglage MOTEURS
            var moteur = new byte[2] { (byte)30, (byte)30 };
            UartEncodeAndSendMessage(0X0040, moteur.Length, moteur);
        }

        private void TextBoxEmission_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //SendMessage();
            }
        }

        private void SendMessage()
        {
            textBoxReception.Text += textBoxEmission.Text;
            textBoxEmission.Text = "";
        }
        byte CalculateChecksum(int msgFunction,
            int msgPayloadLength, byte[] msgPayload)
        {
            byte checksum = 0;
            checksum ^= 0xFE;
            checksum ^= (byte)(msgFunction >> 8);
            checksum ^= (byte)(msgFunction >> 0);
            checksum ^= (byte)(msgPayloadLength >> 8);
            checksum ^= (byte)(msgPayloadLength >> 0);
            for (int i = 0;i< msgPayloadLength; i++)
            {
                checksum ^= msgPayload[i];
            }

            return checksum;
        }

        private void DecodeMessage(byte c)
        {
            switch (rcvState)
            {
                case StateReception.Waiting:
                    //Etape1
                    if (c == 0xFE)
                    {
                        msgDecodedFunction = 0;
                        msgDecodedPayloadIndex = 0;
                        msgDecodedPayloadLength = 0;
                        rcvState = StateReception.FunctionMSB;
                    }
                    break;
                case StateReception.FunctionMSB:
                    msgDecodedFunction += c * (int)Math.Pow(2, 8);
                    //Etape2
                    rcvState = StateReception.FunctionLSB;
                    break;
                case StateReception.FunctionLSB:
                    //Etape3
                    msgDecodedFunction += c;
                    rcvState = StateReception.PayloadLengthMSB;
                    break;
                case StateReception.PayloadLengthMSB:
                    msgDecodedPayloadLength += c * (int)Math.Pow(2, 8);
                    rcvState = StateReception.PayloadLengthLSB;
                    break;
                case StateReception.PayloadLengthLSB:
                    msgDecodedPayloadLength += c;
                    msgDecodedPayload = new byte[msgDecodedPayloadLength];
                    rcvState = StateReception.Payload;
                    break;
                case StateReception.Payload:
                    msgDecodedPayload[msgDecodedPayloadIndex] = c;
                    msgDecodedPayloadIndex += 1;
                    if (msgDecodedPayloadIndex >= msgDecodedPayloadLength)
                    {
                        rcvState = StateReception.CheckSum;
                    }
                    break;
                case StateReception.CheckSum:
                    var calculatedChecksum = 0;
                    calculatedChecksum ^= 0xFE;
                    calculatedChecksum ^= (byte)(msgDecodedFunction >> 8);
                    calculatedChecksum ^= (byte)(msgDecodedFunction >> 0);
                    calculatedChecksum ^= (byte)(msgDecodedPayloadLength >> 8);
                    calculatedChecksum ^= (byte)(msgDecodedPayloadLength >> 0);
                    //    msgDecodedPayloadLength;
                    foreach (var ci in msgDecodedPayload)
                    {
                        calculatedChecksum ^= ci;
                    }
                    var receivedChecksum = c;
                    if (calculatedChecksum == receivedChecksum)
                    {
                        ProcessDecodedMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                        rcvState = StateReception.Waiting;
                    }
                    else
                    {
                        rcvState = StateReception.Waiting;
                        Console.WriteLine("Il y a eu une perturbation dans la trame");
                    }
                        
                    break;
                default:
                    rcvState = StateReception.Waiting;
                    break;
            }
        }

        void UartEncodeAndSendMessage(int msgFunction,
            int msgPayloadLength, byte[] msgPayload)
        {
            var checksum = CalculateChecksum(msgFunction,
             msgPayloadLength, msgPayload);

            var array = new byte[msgPayloadLength + 6];

            byte startOfFrame = 0xFE;
            array[0] = 0xFE;
            array[1] = (byte)(msgFunction >> 8);
            array[2] = (byte)(msgFunction >> 0);
            array[3] = (byte)(msgPayloadLength >> 8);
            array[4] = (byte)(msgPayloadLength >> 0);
            for (var i = 0; i< msgPayloadLength; i++)
            {
                array[i + 5] = msgPayload[i];
            }
            array[array.Length-1] = checksum;
            //var trame = new byte[];
            //serialPort1.Write(byteList, 0, byteList.Length);
            serialPort1.Write(array.ToArray(), 0, array.Length);
        }

        void ProcessDecodedMessage(int msgFunction,
                                    int msgPayloadLength, byte[] msgPayload)
        {
            if (msgFunction == 0x0080)
            {
                var cleanPrintPayload = new byte[msgPayloadLength + 2];
                for (var i = 0; i < cleanPrintPayload.Length; i++)
                {
                    if (i == cleanPrintPayload.Length - 2)
                        //Ajout de CR = déplace le curseur au début de la ligne sans avancer à la ligne suivante
                        cleanPrintPayload[i] = 0X0D;
                    else if (i == cleanPrintPayload.Length - 1)
                        //Ajout de LF
                        cleanPrintPayload[i] = 0X0A;
                    else
                        cleanPrintPayload[i] = msgPayload[i];
                }
                textBoxReception.Text += Encoding.UTF8.GetString(cleanPrintPayload, 0, cleanPrintPayload.Length);
            }
                
            //Fonction de Pilotage de Led
            else if (msgFunction == 0x0020)
            {
                var numLed = msgPayload[0];
                var stateLed= msgPayload[1];
                if (msgPayload[1] == 0x0001)
                    ((CheckBox)LedPanel.Children[numLed]).IsChecked = true;
                if (msgPayload[1] == 0x0000)
                    ((CheckBox)LedPanel.Children[numLed]).IsChecked = false;
            }
            //Fonction de réception de position IR
            else if (msgFunction == 0x0030)
            {
                var irL = msgPayload[0];
                var irC= msgPayload[1];
                var irR= msgPayload[2];
                UpdateIRDetection(irL, irC, irR);
            }
            else if(msgFunction == 0x0040)
            {
                var consigneL= msgPayload[0];
                var consigneR= msgPayload[1];
                UpdateMotors(consigneL, consigneR);
            }
        }

        void UpdateIRDetection(double irL, double irC, double irR)
        {
            Dispatcher.BeginInvoke(new Action(delegate () {
                ((Label)IRPanel.Children[0]).Content = "IR Gauche: " + irL;
                ((Label)IRPanel.Children[1]).Content = "IR Centre: " + irC;
                ((Label)IRPanel.Children[2]).Content = "IR Droit: " + irR;
            }));

            
        }

        void UpdateMotors(double mL, double mR)
        {
            Dispatcher.BeginInvoke(new Action(delegate () {
                ((Label)MoteurPanel.Children[0]).Content = "Moteur Gauche: " + mL;
                ((Label)MoteurPanel.Children[1]).Content = "Moteur Droit: " + mR;
            }));
            
        }

    }

}
