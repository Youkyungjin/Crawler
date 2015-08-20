using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Net.Sockets;
using System.Reflection;

namespace HK.Database
{
    class SshTunnel : IDisposable
    {
        private SshClient client;
        private ForwardedPortLocal port;
        private int localPort;

        public SshTunnel(ConnectionInfo connectionInfo, uint remotePort)
        {
            try
            {
                client = new SshClient(connectionInfo);
                port = new ForwardedPortLocal("127.0.0.1", "leisuredb01", remotePort);
                //port = new ForwardedPortLocal("127.0.0.1", 3306, "leisuredb01", remotePort);
                //port = new ForwardedPortLocal("127.0.0.1", 22, "leisuredb01", remotePort);
                //port = new ForwardedPortLocal("127.0.0.1", "leisuredb01", remotePort);
                //port = new ForwardedPortLocal()

                //client.ErrorOccurred += (s, args) => args.Dump();
                //port.Exception += (s, args) => args.Dump();
                //port.RequestReceived += (s, args) => args.Dump();

                client.Connect();
                client.AddForwardedPort(port);
                port.Start();
                

                // Hack to allow dynamic local ports, ForwardedPortLocal should expose _listener.LocalEndpoint
                var listener = (TcpListener)typeof(ForwardedPortLocal).GetField("_listener", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(port);
                localPort = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public int LocalPort { get { return localPort; } }

        public void Dispose()
        {
            if (port != null)
                port.Dispose();
            if (client != null)
                client.Dispose();

            port = null;            
            client = null;
        }
    }

    public class SqlHelper
    {
        //private string ip_ = "39.115.210.134";
        //private string port_ = "3306";
        //private string database_ = "crawler";
        //private string account_ = "lq";
        //private string pw_ = "1234qwer";
        //private string connection_string_ = "Server={0};Port={1};Database={2};Uid={3};Pwd={4};Connection Timeout=360";
        private string connection_string_ = "Server={0};Port={1};Database={2};Uid={3};Pwd={4};";
        //private string connection_string_ssh_ = "server={0};port={1};database={2};user={3};password={4}";
        private string connection_string_ssh_ = "server={0};user={1};database={2};port={3};password={4};";
        private string real_connection_string_ = "";

        private SshClient SshClient_ = null;
        public MySqlConnection MySqlConnection_ = null;    // DB  연결 객체

        // 소멸자
        ~SqlHelper()
        {
            try
            {
                Close();
            }
            catch (System.Exception ex)
            {
                
            }
            //Console.WriteLine("~SqlHelper");            
        }

        public bool Close()
        {
            bool bResult = true;
            
            if (MySqlConnection_ != null)
            {
                MySqlConnection_.Close();
                MySqlConnection_.Dispose();
                MySqlConnection_ = null;
            }
            else
            {
                bResult = false;
            }

            if (SshClient_ != null)
            {
                foreach (var pData in SshClient_.ForwardedPorts.ToArray())
                {
                    if (pData.IsStarted)
                        pData.Stop();

                    SshClient_.RemoveForwardedPort(pData);
                }

                if (SshClient_.IsConnected == true)
                {
                    SshClient_.Disconnect();
                    SshClient_.Dispose();
                }
                else
                {
                    bResult = false;
                }

                SshClient_ = null;
            }
            else
            {
                bResult = false;
            }


            return bResult;
        }

        public void Connect(string method, string ip, string port, string dbname, string account, string pw, string sshhostname, string sshuser, string sshpw)
        {
            if (method == "ssh")
            {
                ConnectSSH(ip, port, dbname, account, pw, sshhostname, sshuser, sshpw);
            }
            else
            {
                ConnectNormal(ip, port, dbname, account, pw);
            }
        }

        //// 테스트용 실전에서쓰지 말것
        //public void Open()
        //{
        //    MySqlConnection_.Close();
        //    MySqlConnection_.Open();
        //}

        // DB 연결
        private void ConnectNormal(string ip, string port, string dbname, string account, string pw)
        {
            //ip_ = ip;
            //port_ = port;
            //database_ = dbname;
            //account_ = account;
            //pw_ = pw;
            real_connection_string_ = string.Format(connection_string_, ip, port, dbname, account, pw);

            MySqlConnection_ = new MySqlConnection(real_connection_string_);
        }

        private void ConnectSSH(string ip, string port, string dbname, string account, string pw, string sshhostname, string sshuser, string sshpw)
        {
            SshClient_ = new SshClient(ip, sshuser, sshpw);
            SshClient_.Connect();
            var fowardport = new ForwardedPortLocal("127.0.0.1", sshhostname, Convert.ToUInt32(port));
            //var fowardport = new ForwardedPortLocal("127.0.0.1", 25251, sshhostname, Convert.ToUInt32(port));
            SshClient_.AddForwardedPort(fowardport);

            //private string connection_string_ssh_ = "server={0};user={1};database={2};port={3};password={4};";
            fowardport.Start();            
            real_connection_string_ = string.Format(connection_string_ssh_, "127.0.0.1", account, dbname, fowardport.BoundPort, pw);

            MySqlConnection_ = new MySqlConnection(real_connection_string_);
        }

        // 현재 DB 연결 상태 리턴
        public ConnectionState CurState()
        { 
            if (MySqlConnection_ == null)
                return ConnectionState.Broken;

            return MySqlConnection_.State;
        }

        // 프로시저 실행
        public MySqlDataReader call_proc(string proc_name, Dictionary<string, object> argdic = null)
        {
            MySqlCommand cmd = new MySqlCommand(proc_name, MySqlConnection_);
            cmd.CommandType = CommandType.StoredProcedure;

            if (argdic != null)
            {
                foreach (var pData in argdic)
                {
                    cmd.Parameters.AddWithValue(pData.Key, pData.Value);
                    cmd.Parameters[pData.Key].Direction = ParameterDirection.Input;
                }
            }

            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();
            
            MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default);
            cmd.Dispose();
            cmd = null;

            return dr;
        }

        public void call_proc_without_result(string proc_name, Dictionary<string, object> argdic = null)
        {
            MySqlCommand cmd = new MySqlCommand(proc_name, MySqlConnection_);
            cmd.CommandType = CommandType.StoredProcedure;

            if (argdic != null)
            {
                foreach (var pData in argdic)
                {
                    cmd.Parameters.AddWithValue(pData.Key, pData.Value);
                    cmd.Parameters[pData.Key].Direction = ParameterDirection.Input;
                }
            }

            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();

            cmd.ExecuteNonQuery();
         //   cmd.ExecuteReader(CommandBehavior.Default);            
        }

        // 쿼리문 바로 실행
        public MySqlDataReader execute_sql(string querystring)
        {
            MySqlCommand cmd = new MySqlCommand(querystring, MySqlConnection_);
            cmd.CommandType = CommandType.Text;


            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();

            MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default);

            return dr;
        }
    }
}
