using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;
using System.Windows;
using System.Timers;
using System.Runtime.Serialization;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        //ADD Button
        private void button1_Click(object sender, EventArgs e)
        {
            String sender_phone = textBox1.Text;
            listBox1.Items.Add(sender_phone);
        }

        //START Button
        private void button2_Click(object sender, EventArgs e)
        {
            System.Threading.Timer timer = new System.Threading.Timer(urlSend);
            timer.Change(0, 600000);

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

         void urlSend(object sender)
        {
            //Array.Copy(listBox1.Items, sendList, listBox1.Items.Count);
            for (int i = 0; i < listBox1.Items.Count; i++)
            {

                //String UrlParam = "{'id': '{id}', 'x':'{x}', 'y': {y} }";
                String UrlParam = "{\"company_id\": \"{id}\", \"order_seq\":\"{x}\", \"order_name\": \"{y}\", \"order_phone\": \"{y}\" ,\"deal\" : \"{y}\" }";
                Random r = new Random();
                UrlParam = UrlParam.Replace("{id}", listBox1.Items[i].ToString());
                UrlParam = UrlParam.Replace("{x}", Convert.ToString(r.Next(50, 150)));
                UrlParam = UrlParam.Replace("{y}", Convert.ToString(r.Next(50, 150)));
                // Encoding
                Encoding encoding = Encoding.UTF8;
                byte[] result = encoding.GetBytes(UrlParam.ToString());

                // 타겟이 되는 웹페이지 URL
                string Url = "http://devpart.hallifactory.com/center/API/hallifactory_API/order";
                // HttpWebRequest 오브젝트 생성
                HttpWebRequest wReqFirst = (HttpWebRequest)WebRequest.Create(Url);

                // HttpWebRequest 오브젝트 설정
                wReqFirst.Method = "POST";
                wReqFirst.ContentType = "application/json; charset=utf-8";
                wReqFirst.Accept = "application/json";
                wReqFirst.ContentLength = result.Length;

                // POST할 데이터를 입력합니다.
                Stream postDataStream = wReqFirst.GetRequestStream();
                postDataStream.Write(result, 0, result.Length);
                postDataStream.Close();

                // HttpWebRequest오브젝트로 부터 HttpWebResponse오브젝트를 생성합니다.
                // HttpWebRequest오브젝트에 문제가 있을 경우 이부분에서 Exception이 발생합니다.
                // 확실히 해두기 위해서는 try-catch로 핸들링 해줄 필요가 있습니다.
                HttpWebResponse wRespFirst = (HttpWebResponse)wReqFirst.GetResponse();

                // Response의 결과를 스트림을 생성합니다.
                Stream respPostStream = wRespFirst.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

                // 생성한 스트림으로부터 string으로 변환합니다.
                string resultPost = readerPost.ReadToEnd();

            }

        }


        //STOP Button
        private void button3_Click(object sender, EventArgs e)
        {

            timer1.Stop();
            timer1.Dispose();

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
