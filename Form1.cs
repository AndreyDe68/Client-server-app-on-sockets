using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    public partial class Form1 : Form
    {

        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
        private Server server;
        private string message;
        //private string text;
        SqlConnection connection = new SqlConnection("Data Source=WIN-JGFMOHKVAA9\\SQLEXPRESS; Initial Catalog = Cinema1; User ID = sa; Password = 123456");
        SqlDataAdapter adapter;
        SqlCommand command;
        DataTable table;
        //string p;

        public Form1()
        {
            InitializeComponent();
        }
        public void Connect()

        {

        socket.Bind(new IPEndPoint(IPAddress.Any, 904));

        socket.Listen(1);

        Socket client = socket.Accept();

        server = new Server(client);

        Receive();

        }

    public async void Receive()

    {
        message = await server.Receive();
        searchData(message);
            using (var ms = new MemoryStream()) 
            {
            new BinaryFormatter().Serialize(ms, table); byte[] array = ms.ToArray();
            server.Send(array);
            }
            Receive();
    }

        public void searchData(string message)
        {
            string query =$"SELECT [Название фильма], Жанр, Время, Дата FROM [Cinema1].[dbo].[Table] WHERE [Название кинотеатра] ='{message}'";
            command = new SqlCommand(query, connection);
            adapter = new SqlDataAdapter(command);
            table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            
        }
    private void button1_Click(object sender, EventArgs e)
        {
            Connect();
        }
        public class Server

        {

            private Socket _client;

            private string message;
            public Server(Socket client)

            {
                _client = client;
            }

            public Task<string> Receive()
            {
                return Task.Run(() =>

                {
                    byte[] buffer1 = new byte[128];

                    _client.Receive(buffer1);

                    message = Encoding.UTF8.GetString(buffer1);

                    return message;
                });
            }
            public void Send(byte[] text)
            {
                _client.Send(text);
                
            }
        }
    }
}
