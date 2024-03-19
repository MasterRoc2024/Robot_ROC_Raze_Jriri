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
        Robot robot;
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
            if (robot.receivedText != "")
            {
                textBoxReception.Text += robot.receivedText;
                textBoxEmission.Text = "";
                robot.receivedText = "";
            }

        }

        private void SerialPort1_DataReceived(object? sender, DataReceivedArgs e)
        {
            robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            
        }

        //Fonction d'Evenements (Interruptions)
        private void ButtonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            /*if (buttonEnvoyer.Background == initBrush)
                buttonEnvoyer.Background = Brushes.RoyalBlue;
            else 
                buttonEnvoyer.Background = initBrush;*/
            //SendMessage();
            if (textBoxEmission.Text == "")
            {
                for (int i = 0; i < byteList.Length; i++)
                {
                    byteList[i] = (byte)(2*i);
                }
                serialPort1.Write(byteList, 0, byteList.Length);
            }
            else
            {
                if (serialPort1.IsOpen == true)
                    serialPort1.WriteLine(textBoxEmission.Text);
                else
                    throw new Exception("Envoi de data sur un port ferme");
            }
                
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
    }
}
