namespace AOTW_P2PChat
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Microsoft.Data.Sqlite;
    using System.Collections.Generic;

    public partial class Form1 : Form
    {
        IPAddress? SendToIP = IPAddress.Parse("127.0.0.1");
        TcpClient client = new();
        TcpListener Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
        bool send = false;
        SqliteConnection DBconnection = new SqliteConnection($"Data Source=MessageHistory.db");

        public Form1()
        {
            InitializeComponent();
            ReadBox.ReadOnly = true;
            this.IPBox.TextChanged += ValidateIP;
            StartMessageDB();
            Listener.Start();
        }

        public void StartMessageDB()
        {
            if (!File.Exists("MessageHistory.db"))
            {
                File.Create("MessageHistory.db");
            }
            DBconnection.Open();
            string MakeTable = "CREATE TABLE IF NOT EXISTS MessageHistory (MessageID INT PRIMARY KEY, IPAddr STRING, ChatLog NVARCHAR(2048) NULL)";
            var CommandRunner = new SqliteCommand(MakeTable, DBconnection);
            CommandRunner.ExecuteReader();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _ = Task.Run(SendTextMessage);
            _ = Task.Run(ReceiveTextMessage);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            send = true;
        }

        private void ValidateIP(object? sender, EventArgs e)
        {
            if (!IPAddress.TryParse(IPBox.Text, out SendToIP))
            {
                IPBox.ForeColor = Color.Red;
            }
            else
            {
                IPBox.ForeColor = Color.Black;
                ReplayMessages();
            }
        }

        private async Task SendTextMessage()
        {
            while (true)
            {
                if (SendToIP is not null && send)
                {
                    if (!client.Connected)
                    {
                        await client.ConnectAsync(SendToIP, 8080);
                    }
                    NetworkStream Datastream = client.GetStream();
                    byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(MessageBox.Text);
                    Datastream.Write(Buffer, 0, Buffer.Length);
                    send = false;
                }
                await Task.Delay(100);
            }
        }

        private async Task ReceiveTextMessage()
        {
            TcpClient clientReceive = await Listener.AcceptTcpClientAsync();
            while (true)
            {
                NetworkStream nwStream = clientReceive.GetStream();
                byte[] buffer = new byte[clientReceive.ReceiveBufferSize];
                int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                string TimeStamp = DateTime.Now.ToShortTimeString();
                this.Invoke(() => ReadBox.AppendText(TimeStamp + ": " + dataReceived + '\n'));
                var MessageID = FindNextMessageId();
                string AppendMessageToDB = "INSERT INTO MessageHistory VALUES (" + MessageID.ToString() + " , '" + SendToIP + "','" + dataReceived + "')";
                var CommandRunner = new SqliteCommand(AppendMessageToDB, DBconnection);
                CommandRunner.ExecuteNonQuery();
                await Task.Delay(100);
            }
        }

        int FindNextMessageId()
        {
            string FindNextId = "SELECT IFNULL(MAX(MessageID),0) FROM MessageHistory";
            var Command = new SqliteCommand(FindNextId, DBconnection);
            var Result = Command.ExecuteScalar();
            int MessageId = Convert.ToInt32(Result);
            return MessageId + 1;
        }

        void ReplayMessages()
        {
            string CollectMessages = "SELECT GROUP_CONCAT(ChatLog, '\n') FROM MessageHistory WHERE IpAddr = " + "'" + SendToIP.ToString() + "'";
            var Command = new SqliteCommand(CollectMessages, DBconnection);
            var Result = Command.ExecuteScalar();
            this.Invoke(() => ReadBox.AppendText(Result.ToString()));
        }
    }
}
