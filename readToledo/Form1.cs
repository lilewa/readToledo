using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace readToledo
{
    public partial class Form1 : Form
    {
        SerialPort mySerialPort = new SerialPort("COM1");
        StringBuilder sb = new StringBuilder();
        Dictionary<string, string> dictASCII2Num;
        public Form1()
        {
            InitializeComponent();

            mySerialPort.BaudRate = 9600;
            mySerialPort.Parity = Parity.None;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None; 
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            mySerialPort.ReceivedBytesThreshold = 128;

            dictASCII2Num = new Dictionary<string, string>();
            dictASCII2Num["48"] = "0";
            dictASCII2Num["49"] = "1";
            dictASCII2Num["50"] = "2";
            dictASCII2Num["51"] = "3";
            dictASCII2Num["52"] = "4";
            dictASCII2Num["53"] = "5";
            dictASCII2Num["54"] = "6";
            dictASCII2Num["55"] = "7";
            dictASCII2Num["56"] = "8";
            dictASCII2Num["57"] = "9";
     

        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            //48,48,48,48,13,2,53,48,32,32,32
            byte[] byt = new byte[128];
            mySerialPort.Read(byt, 0, 128);
            string input= string.Join("", byt);
            string[] result= input.Split(new string[] {"484848481325348" }, 2, StringSplitOptions.RemoveEmptyEntries);
            string num = string.Empty;
            if (result.Length == 2)
            {
                int k = 0;
                for (k=0; k <= 8; k+=2)
                {
                    if (result[1].Substring(k, 2) != "32")
                        break;
                }
                for (int i = 0; i <= 10; i++)
                {
                    string s = result[1].Substring((i * 2)+k, 2);
                    if (!dictASCII2Num.ContainsKey(s))
                        break;

                    num += dictASCII2Num[s];
                }
                textBox1.Text = textBox1.Text+" " + num; 
            }
            mySerialPort.DiscardInBuffer();
            mySerialPort.Close();



        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mySerialPort.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mySerialPort.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                mySerialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
    }
}
