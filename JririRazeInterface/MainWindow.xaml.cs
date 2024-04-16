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
    public partial class MainWindow : Window
    {
        Brush initBrush;
        ExtendedSerialPort serialPort1;
        DispatcherTimer timerAffichage;
        public Robot robot;
        byte[] byteList; 

        public MainWindow()
        {
            InitializeComponent();
            initBrush = buttonEnvoyer.Background;
            buttonEnvoyer.Click += ButtonEnvoyer_Click;
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
                textBoxReception.Text += Encoding.UTF8.GetString(c, 0, 1);
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
            var t = 1;
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
                //serialPort1.WriteLine(textBoxEmission.Text);

                UartEncodeAndSendMessage(128, textBoxEmission.Text.Length, Encoding.ASCII.GetBytes(textBoxEmission.Text));
            else
                throw new Exception("Envoi de data sur un port ferme");

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

        void UartEncodeAndSendMessage(int msgFunction,
            int msgPayloadLength, byte[] msgPayload)
        {
            var checksum = CalculateChecksum(msgFunction,
             msgPayloadLength, msgPayload);
            int startOfFrame = 0xFE;
            var debutTrame = new byte[5];
            //var trame = new byte[];
            //serialPort1.Write(byteList, 0, byteList.Length);
            //serialPort1.Write()
        }
    }

}
