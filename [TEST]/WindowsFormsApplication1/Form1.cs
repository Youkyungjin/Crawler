using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections;
using HKLibrary.WEB;
using Tamir.SharpSsh;
using HKLibrary.comwls;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string makefile = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
            string sDirPath;
            sDirPath = makefile + "\\data";
            DirectoryInfo di = new DirectoryInfo(sDirPath);
            if (di.Exists == false)
            {
                di.Create();
            }
            
            string PinCode = "41302903000002";
            makefile = makefile + "\\data\\"+PinCode+".png";
            Int32 OrderSeq = 406359;

            string folderName = Convert.ToString(OrderSeq);
            string ftpfileName = folderName + "/" + PinCode + ".jpg";

            string ftpBasicPath = "/var/www/IMAGE/Web/upload/order/barcode";
            //string ftpBasicPath = "/var/www/IMAGE/Web/upload/order/qrcode/";
            
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode = qrEncoder.Encode(PinCode);
            var renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero), Brushes.Black, Brushes.White);
            MemoryStream ms = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
            var image = new Bitmap(Image.FromStream(ms), new Size(new Point(200, 200)));
            image.Save(makefile, ImageFormat.Png);
            Tamir.SharpSsh.Sftp sftp;

            sftp = new Tamir.SharpSsh.Sftp("ledev.leisureq.co.kr", "lion", "gkffl1!");
            sftp.Connect(10004);
            ArrayList res =  sftp.GetFileList("/");
            sftp.Mkdir(ftpBasicPath + "/" + folderName+ "/");
            sftp.Put(makefile, ftpBasicPath + "/" + ftpfileName);
            sftp.Close();


            //BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            //b.Encode(BarcodeLib.TYPE.CODE128, PinCode);
            //b.SaveImage(makefile, BarcodeLib.SaveTypes.JPG);
            /*
            string date = "2015-05-20 17:16:44";
            string[] datePartPath = new string[4];

            DateTime dt = Convert.ToDateTime(date);
            datePartPath[0] = dt.ToString("yyyy");
            datePartPath[1] = dt.ToString("MM");
            datePartPath[2] = dt.ToString("dd");
            datePartPath[3] = Convert.ToString(OrderSeq);


            for(Int32 i =0; i<datePartPath.Length; i++){
                ftpBasicPath = ftpBasicPath + "/" + datePartPath[i];
            }
            comwls.image_01_FileMake(ftpBasicPath);
           
            */
            string ftpUri = "sftp://ledev.leisureq.co.kr:10004/"+ ftpBasicPath + folderName;

            HKLibrary.comwls.comwls.image_01_FileMake(ftpBasicPath + folderName);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            //request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("lion", "gkffl1!");
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream ftpStream = response.GetResponseStream();

            ftpStream.Close();
            response.Close();
            

           /*
            string ftpUri = "ftp://121.78.127.40:21/" + ftpBasicPath + folderName;
                
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
                
            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("infobay", "info9887");
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                ftpUri = "ftp://121.78.127.40:21/" + ftpBasicPath + ftpfileName;
                request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("infobay", "info9887");

                byte[] fileContents = File.ReadAllBytes(makefile);
                request.ContentLength = fileContents.Length;

                request.UsePassive = true;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
                response = (FtpWebResponse)request.GetResponse();
                response.Close();
           


            //sftp.ConnectTimeoutMs = 15000;
            //sftp.IdleTimeoutMs = 15000;

            //string hostname = "ftp://ledev.leisureq.co.kr:10004/";
            
            //bool success = sftp.Connect(hostname, 10004);
            */
        }

       
    }
}
