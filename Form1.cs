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
        public Form1()
        {
            InitializeComponent();
            ReadBox.ReadOnly = true;
            this.IPBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(ValidateIP);
            StartMessageDB();
            Listener.Start();
        }

        public void StartMessageDB()
        {
            if (!File.Exists("MessageHistory.db"))
            {
                File.Create("MessageHistory.db");
            }

            var DBconnection = new SqliteConnection($"Data Source=MessageHistory.db");
            DBconnection.Open();
            string MakeTable = "CREATE TABLE IF NOT EXISTS MessageHistory (IPAddr STRING PRIMARY KEY, ChatLog NVARCHAR(2048) NULL)";
            var CommandRunner = new SqliteCommand(MakeTable);
            CommandRunner.Connection = DBconnection;
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

        private void ValidateIP(object? sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (!IPAddress.TryParse(IPBox.Text, out SendToIP))
            {
                IPBox.ForeColor = Color.Red;
            }
            else
            {
                IPBox.ForeColor = Color.Black;
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
                await Task.Delay(100);
            }
        }
    }
}
