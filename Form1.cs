namespace AOTW_P2PChat
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Microsoft.Data.Sqlite;

    public partial class Form1 : Form
    {
        IPAddress SendToIP = IPAddress.Parse("1.1.1.1");
        TcpClient client = new();
        TcpListener Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
        bool send = false;
        SqliteConnection DBconnection = new SqliteConnection($"Data Source=MessageHistory.db,Mode=ReadWriteCreate");

        public Form1()
        {
            InitializeComponent();
            ReadBox.ReadOnly = true;
            StartMessageDB();
            Listener.Start();
        }

        public void StartMessageDB()
        {
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
                this.Invoke(() => ReadBox.AppendText(TimeStamp + " : " + dataReceived + '\n'));

                AddMessageToDB(TimeStamp + " : " + dataReceived);
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
            string CollectMessages = "SELECT GROUP_CONCAT(ChatLog, '\n') FROM MessageHistory WHERE IpAddr = @CurrentIP";
            var Command = new SqliteCommand(CollectMessages, DBconnection);
            Command.Parameters.AddWithValue("@CurrentIP", SendToIP.ToString());
            var Result = Command.ExecuteScalar();
            this.Invoke(() => ReadBox.AppendText(Result.ToString()));
            if (Result.ToString().Length > 0)
            {
                this.Invoke(() => ReadBox.AppendText("\n"));
            }
        }

        void AddMessageToDB(string message)
        {
            var MessageID = FindNextMessageId();
            string AppendMessageToDB = "INSERT INTO MessageHistory VALUES (@MessageID, @CurrentIP, @MessageContent)";
            var CommandRunner = new SqliteCommand(AppendMessageToDB, DBconnection);
            CommandRunner.Parameters.AddWithValue("@MessageID", MessageID.ToString());
            CommandRunner.Parameters.AddWithValue("@CurrentIP", SendToIP.ToString());
            CommandRunner.Parameters.AddWithValue("@MessageContent", message);
            CommandRunner.ExecuteNonQuery();
        }

        private void ReadBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
