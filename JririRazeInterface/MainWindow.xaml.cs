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

namespace JririRazeInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
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
            var content = Encoding.ASCII.GetBytes(textBoxEmission.Text);
            UartEncodeAndSendMessage(0X0080, content.Length, content);
            //Reglage LED
            var leds = new byte[2] { (byte)7, 0X01 };
            UartEncodeAndSendMessage(0X0020, leds.Length, leds);
            //Reglage TELEMETRE

            //UartEncodeAndSendMessage(0X0020, content.Length, content);
            //Reglage MOTEURS
            var moteur = new byte[2] { (byte)MOTEUR_DROIT, (byte)35 };
            UartEncodeAndSendMessage(0X0020, moteur.Length, moteur);
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
                        var cleanPrintPayload= new byte[msgDecodedPayload.Length + 2];
                        for (var i= 0; i < cleanPrintPayload.Length; i++)
                        {
                            if (i == cleanPrintPayload.Length - 2)
                                //Ajout de CR = déplace le curseur au début de la ligne sans avancer à la ligne suivante
                                cleanPrintPayload[i] = 0X0D;
                            else if (i == cleanPrintPayload.Length -1)
                                //Ajout de LF
                                cleanPrintPayload[i] = 0X0A;
                            else
                                cleanPrintPayload[i] = msgDecodedPayload[i];
                        }
                        textBoxReception.Text += Encoding.UTF8.GetString(cleanPrintPayload, 0, cleanPrintPayload.Length);
                        rcvState = StateReception.Waiting;
                    }
                    else
                    {
                        rcvState = StateReception.Waiting;
                        throw new Exception("Il y a eu une perturbation dans la trame");
                    }
                        
                    break;
                default:
                    rcvState = StateReception.Waiting;
                    break;
            }
        }

        void ProcessDecodedMessage(int msgFunction,
                                    int msgPayloadLength, byte[] msgPayload)
        {

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

        
    }

}
