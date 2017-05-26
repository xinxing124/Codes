using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EvalGuy;

namespace 测试1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

    //        Private Function Get_CRC16(ByVal Crc16_num As Byte(), ByVal nLength As Byte) As UShort
    //    Dim i, j As UShort
    //    Dim crc As UShort
    //    crc = &HFFFF
    //    For i = 0 To nLength - 1 Step 1
    //        crc = crc Xor Crc16_num(i)
    //        For j = 0 To 7 Step 1
    //            If (crc And &H1) > 0 Then
    //                crc = crc >> 1
    //                crc = crc Xor &HA001
    //            Else
    //                crc = crc >> 1
    //            End If
    //        Next j
    //    Next i
    //    Return crc
    //End Function

        private uint crc16_modebus(byte[] modbusdata, uint modbusdatalength,byte[] crcval)
        {
            uint i, j;
            uint crc16 = 0xFFFF;

            for (i = 0; i < modbusdatalength; i++)
            {
                crc16 ^= modbusdata[i];
                for (j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x01) == 1)
                        crc16 = (crc16 >> 1) ^ 0xA001;
                    else
                        crc16 = crc16 >> 1;
                }
            }
            crcval[0] =(byte) ((crc16 & 0xff00) >> 8);//高位置
            crcval[1] = (byte)(crc16 & 0x00ff);  //低位置
            return crc16;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] sendbytes = { 0x01, 0x03, 0x02, 0x58, 0x00, 0x0C };
            byte[] revbytes = new byte[2];
            //, 0xC4, 0x62
            uint s = crc16_modebus(sendbytes, 6, revbytes);

            byte[] sendbyte = { 0x01, 0x03, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] revbyte = new byte[2];
            //, 0xC4, 0x62
            uint ss = crc16_modebus(sendbyte, 27, revbyte);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Test0: {0}", Evaluator.EvaluateToInteger("(30 + 4) * 2"));
            Console.WriteLine("Test1: {0}", Evaluator.EvaluateToString("\"Hello \" + \"There\""));
            Console.WriteLine("Test2: {0}", Evaluator.EvaluateToBool("30 == 40"));
            Console.WriteLine("Test3: {0}", Evaluator.EvaluateToObject("new DataSet()"));
            Console.WriteLine("Test3: {0}", Evaluator.EvaluateToDouble("(1.0/3.0)*2.0"));
            
            EvaluatorItem[] items = {   
                          new EvaluatorItem(typeof(int), "(30 + 4) * 2", "GetNumber"),   
                          new EvaluatorItem(typeof(string), "\"Hello \" + \"There\"",    
                                                            "GetString"),   
                          new EvaluatorItem(typeof(bool), "30 == 40", "GetBool"),   
                          new EvaluatorItem(typeof(object), "new DataSet()", "GetDataSet"),
                          new EvaluatorItem(typeof(double), "12.5", "GetDouble")   
                        };

            Evaluator eval = new Evaluator(items);
            Console.WriteLine("TestStatic0: {0}", eval.EvaluateInt("GetNumber"));
            Console.WriteLine("TestStatic1: {0}", eval.EvaluateString("GetString"));
            Console.WriteLine("TestStatic2: {0}", eval.EvaluateBool("GetBool"));
            Console.WriteLine("TestStatic3: {0}", eval.Evaluate("GetDataSet"));
            Console.WriteLine("TestStatic4: {0}", eval.Evaluate("GetDouble"));
        }

        private void button3_Click(object sender, EventArgs e)
        {
             Console.WriteLine(GetOID());
        }
        private string GetOID()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}
