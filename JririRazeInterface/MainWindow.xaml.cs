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

namespace JririRazeInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush initBrush;
        ExtendedSerialPort serialPort1;
        public MainWindow()
        {
            InitializeComponent();
            initBrush = buttonEnvoyer.Background;
            buttonEnvoyer.Click += ButtonEnvoyer_Click;
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1 = new ExtendedSerialPort("COM7", 115200, Parity.None, 8, StopBits.One);
            serialPort1.Open();
        }

        private void SerialPort1_DataReceived(object? sender, DataReceivedArgs e)
        {
            textBoxReception.Text += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
        }

        //Fonction d'Evenements (Interruptions)
        private void ButtonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            /*if (buttonEnvoyer.Background == initBrush)
                buttonEnvoyer.Background = Brushes.RoyalBlue;
            else 
                buttonEnvoyer.Background = initBrush;*/
            SendMessage();
        
        }

        private void TextBoxEmission_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            textBoxReception.Text += textBoxEmission.Text;
            textBoxEmission.Text = "";
        }
    }
}
