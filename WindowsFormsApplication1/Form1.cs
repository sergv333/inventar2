using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace WindowsFormsApplication1
{
 
    public partial class Form1 : Form
    {
      
        public Form1()
        {
            InitializeComponent();
        }
        public string seregja = "";
        string connectionString = 
            
                    "Provider=SQLOLEDB;"
                + "Data Source=MBN01_sfkdb01;"
                + "Initial Catalog=TambovSFKPlant;"
                + "User Id=ASUTPsqlUserR;"
                + "Password =Asutp2016ROnly;";
            

        private TcpListener tcpListener;
        private Thread listenThread;
        private void button1_Click(object sender, EventArgs e)
        {
            /*Start listener and kick to new thread to free the UI*/
            tcpListener = new TcpListener(IPAddress.Any, int.Parse(textBox1.Text));
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
            this.Invoke((MethodInvoker)delegate
            {
                //This unnamed delegate is used to access UI elements on the main thread
                txtTalk.AppendText("Server started...Listening for clients...\r\n");
            }); button1.Enabled = false; button2.Enabled = true;

        }

        private void ListenForClients()
        {
            /*Listner thread for connections, runs indefinitely until server stopped
             kicks incomming connection communication to new thread*/
            tcpListener.Start();

            try
            {
                while (true)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();   //blocking call

                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);
                
                    this.Invoke((MethodInvoker)delegate
                    {
                        txtTalk.AppendText("\r\nClient connected: ");
                       
                    });
                }
            }
            catch (SocketException)
            {
                //This exception is thrown when server is terminated
                //caught just so that the program doesn't crash
            }
            finally
            {
                this.Invoke((MethodInvoker)delegate
                {
                    txtTalk.AppendText("Server stopped\r\n");
                });
            }
        }
           int counter = 0;
        string u = "";
        private void HandleClientComm(object client)
        {
            
            /*Handles all communication once server receives client*/
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            Byte[] bytes = new Byte[4096];
            String data = null;
            string message = u;
         
            // int i;
            int bytesRead;
            bool b=false;
            bytesRead = 0;
            //  data = null;
        //     string s = "";
        //    string s1 = "1";
            // Принимаем данные от клиента в цикле пока не дойдём до конца.
            while ((bytesRead = clientStream.Read(bytes, 0, bytes.Length)) != 0)  //blocking calltrue)
            {   UnicodeEncoding encoder = new UnicodeEncoding();
                 
                this.Invoke((MethodInvoker)delegate
                {
                    counter++;
                
                    textBox4.Clear();
                    textBox4.AppendText(" Получено: " + encoder.GetString(bytes, 0, bytesRead) + "\r\n" + dataGridView2.Rows.Count + "\r\n" + counter.ToString() + "\r\n");
                    txtTalk.AppendText(" Получено: " + encoder.GetString(bytes, 0, bytesRead));
                    seregja = textBox2.Text;



                    textBox5.Text = seregja;
                    textBox2.Clear();
                    textBox2.AppendText(encoder.GetString(bytes, 0, bytesRead));
                    if (seregja == textBox2.Text)
                    {
                        txtTalk.AppendText("уже был ");
                    }

                });
                try
                {

                    // Преобразуем данные в ASCII string.
                    data = System.Text.Encoding.Unicode.GetString(bytes, 0, bytesRead);

                    // Преобразуем строку к верхнему регистру.
                    data = data.ToUpper();

                    // Преобразуем полученную строку в массив Байт.
                    byte[] msg = System.Text.Encoding.Unicode.GetBytes(data);
                    // Отправляем данные обратно клиенту (ответ).
                    //   clientStream.Write(msg, 0, msg.Length);

                    UnicodeEncoding encoder1 = new UnicodeEncoding();
                    this.Invoke((MethodInvoker)delegate
                    {
                        //dataGridView2.Rows[counter].Cells[4].Value = textBox2.Text;
                        try
                        {
                            for (int i = 0; i <= dataGridView2.Rows.Count - 1; i++)
                                //  for (int j = 0; j <= dataGridView2.ColumnCount - 1; j++)

                                if (dataGridView2.Rows[i].Cells[2].Value != null && dataGridView2.Rows[i].Cells[2].Value.ToString() == textBox2.Text)
                                {
                                    
                                    b = true;
                                    dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.GreenYellow;
                                    dataGridView2.Rows[i].Selected = true;
                                    // dataGridView2.Rows[i].Cells[4].Value = textBox2.Text;
                                    dataGridView2.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;

                                }
                             
                                   

                                    foreach (DataGridViewRow row in dataGridView2.SelectedRows)
                                    {
                                        object[] items = new object[row.Cells.Count];
                                        for (int i1 = 0; i1 < row.Cells.Count; i1++)
                                        {
                                            items[i1] = row.Cells[i1].Value;


                                        }
                                        dataGridView1.Rows.Add(items);
                                for (int j = 0; j <= dataGridView1.RowCount - 1; j++)
                                    if (dataGridView1.Rows[j].Cells[2].Value != null && dataGridView1.Rows[j].Cells[2].Value.ToString() == textBox2.Text)
                                        dataGridView1.Rows[j].DefaultCellStyle.BackColor = Color.GreenYellow;
                                dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
                               
                            }
                                
                    label4.Text = "Количество паллет прошедшие инвентаризацию\rВсего: " + dataGridView1.RowCount.ToString();

                          
                        }
                        catch
                        {

                        } 
                        if (b == true)
                        {
                            b = true;
                            foreach (DataGridViewRow row in dataGridView2.SelectedRows)
                            {
                                dataGridView2.Rows.Remove(row);
                              
                            }
                            label3.Text = "Количество паллет для инвентаризации\rВсего: " + dataGridView2.RowCount.ToString();
                            u = "ДА "+ textBox2.Text;
                            byte[] data1 = Encoding.Unicode.GetBytes(u);
                            clientStream.Write(data1, 0, data1.Length);
                            txtTalk.AppendText(" Отправлено: Паллет в списке. " );
                          
                        }
                        else
                        {
                             dataGridView1.Rows.Add(null,null,textBox2.Text);
                            for (int j = 0; j <= dataGridView1.RowCount - 1; j++)
                               if (dataGridView1.Rows[j].Cells[2].Value != null && dataGridView1.Rows[j].Cells[2].Value.ToString() == textBox2.Text)
                                    dataGridView1.Rows[j].DefaultCellStyle.BackColor = Color.Red;
                            //  listBox1.Items.Add(textBox2.Text);
                            u = "НЕТ" + textBox2.Text;
                            byte[] data1 = Encoding.Unicode.GetBytes(u);
                            clientStream.Write(data1, 0, data1.Length);
                            txtTalk.AppendText(" Отправлено: Паллет не в списке. " );
                        }
                  


                    //    txtTalk.AppendText("послал клиенту: " + encoder1.GetString(msg, 0, bytesRead) + "\r\n");
                    });

                }
                catch (Exception ex)
                {
                    txtTalk.AppendText("ERROR: " + ex.Message + "\r\n");
                    break;
                }

              
            

                 if (bytesRead == 0)
                {
                    break;
                }

            }
           tcpClient.Close();
            this.Invoke((MethodInvoker)delegate
            {
                txtTalk.AppendText("Client disconnected\r\n");
            });




        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*Stops the server
            Note that because stop() will cause the listener to terminate with an exception (see above)
            any code after this statement in this event handler will be skipped*/
         tcpListener.Stop(); button1.Enabled = true; button2.Enabled = false;
            /*DON'T PUT ANYTHING HERE, NEVER EXECUTES*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UploadProductsListFromDB(); // Выгрузка списка продуктов в таблицу

        }
        DataSet ds = new DataSet();
        public void UploadProductsListFromDB() // Выгрузка списка продуктов в таблицу
        {
           
            string queryString = "SELECT [Synonym].[description],w.[Synonym_ID],w.[id],w.[Position_ID]" +
        "FROM[TambovSFKPlant].[dbo].[Pallet] AS w INNER JOIN Synonym ON w.Synonym_ID=Synonym.[id]" +
       "where w.[Position_ID]='130'" + " and w.[ToDestination_ID] = '130' order by [Synonym].[description] ";

            ds.Clear(); // Очистка данных о продукте и списка продуктов

            using (OleDbConnection con = new OleDbConnection(connectionString)) // Параметры соединения
            {
                OleDbDataAdapter dataadapter = new OleDbDataAdapter(queryString, con); // Запрос

                try
                {
                    con.Open(); // Открытие соединения

                    dataadapter.Fill(ds, "Products_Table"); // Полученные данные кладем в "Products_Table" в 
                }                                           // DataSet

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                finally
                {
                    con.Close(); // Закрываем соединение
                    
                    dataGridView2.DataSource = ds; // Указываем источник данных для таблицы
                    dataGridView2.DataMember = "Products_Table"; // Указываем таблицу в DataSet, откуда брать данные

                    //for (int i = 0; i < dataGridView2.Columns.Count; i++)
                    //{
                    //    dataGridView2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    //}

                    dataGridView2.Columns[0].HeaderText = "Имя синонима"; // Задаем имя первого столбца
                    dataGridView2.Columns[1].HeaderText = "SFK-ID"; // Задаем имя второго столбца
                    dataGridView2.Columns[2].HeaderText = "ID-паллеты"; // Задаем имя второго столбца
                    dataGridView2.Columns[3].HeaderText = "Позиция"; // Задаем имя второго столбца
              
                //    dataGridView2.Columns.Add("tn_ob", "SCAN");
                    dataGridView1.Columns.Add("tn_ob0", "Имя синонима");
                    dataGridView1.Columns.Add("tn_ob1", "SFK-ID");
                    dataGridView1.Columns.Add("tn_ob2", "ID-паллеты");
                    dataGridView1.Columns.Add("tn_ob3", "Позиция");
                   
                    label3.Text = "Количество паллет для инвентаризации\rВсего: " + dataGridView2.RowCount.ToString();
                    label4.Text = "Количество паллет прошедшие инвентаризацию\rВсего: "+ dataGridView1.RowCount.ToString();
                //    dataGridView2.AutoResizeColumns();
                  //  dataGridView1.AutoResizeColumns();
                    //DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
                    //columnHeaderStyle.BackColor = Color.Beige;
                    //columnHeaderStyle.Font = new Font("Verdana", 8, FontStyle.Bold);
                    //dataGridView2.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
                }
            }
        }

        [Serializable]
        public class Person
        {
            public string Name, Surname, Title, FathName;
        }
        //класс и его члены объявлены как public

       //[Serializable]
       // public class Person
       // {
       //     public string Name { get; set; }
       //     public int Age { get; set; }

       //     // стандартный конструктор без параметров
       //     public Person()
       //     { }

       //     public Person(string name, int age)
       //     {
       //         Name = name;
       //         Age = age;
       //     }
            
       // }
       // public class Person1
       // {
       //     public string Name { get; set; }
       //     public int Age { get; set; }

       //     // стандартный конструктор без параметров
       //     public Person1()
       //     { }

       //     public Person1(string name, int age)
       //     {
       //         Name = name;
       //         Age = age;
       //     }

       // }

        private void button4_Click(object sender, EventArgs e)
        {
            // считать с файла
            string fileName = "test.xml";
            if (File.Exists(fileName))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(fileName);
                dataGridView1.DataSource = ds;
             //   dataGridView1.DataMember = "row";
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // // объект для сериализации
            // Person person = new Person();
            // person.Name = textBox3.Text;
            // Person person1 = new Person(textBox3.Text, 298769);
            //// Person[] people = new Person[] { person1, person };

            // // Console.WriteLine("Объект создан");

            // // передаем в конструктор тип класса
            // XmlSerializer formatter = new XmlSerializer(typeof(Person));

            // // получаем поток, куда будем записывать сериализованный объект
            // using (FileStream fs = new FileStream("persons.xml", FileMode.OpenOrCreate))
            // {
            //     formatter.Serialize(fs, person);

            //     // Console.WriteLine("Объект сериализован");
            // }





            button3.Enabled = false;
            Person Bob = new Person();
            //  Bob.Name = textBox6.Text;
            Bob.Surname = textBox7.Text;
            Bob.Title = textBox8.Text;
            Bob.FathName = textBox9.Text;
           XmlSerializer formatter = new XmlSerializer(typeof(Person));
            FileStream filestream = new   FileStream("output11.xml", FileMode.OpenOrCreate);
            //  BinaryFormatter bf = new BinaryFormatter();
            //  bf.Serialize(filestream, Bob);
            formatter.Serialize(filestream, Bob);
            filestream.Close();
            button3.Enabled = true;
        }
    }
}
//[Serializable]
//private class Person
//{
//    public string Name, Surname, Title, FathName;
//}
//private string path = "test.xml";
//public DataSet dset;
//DataTable load_table;
//BindingSource bs;
//private void button4_Click(object sender, EventArgs e)
//{

//    DataSet dataSet = new DataSet();
//    dataSet.ReadXml("test.xml");
//    dataGridView1.DataSource = dataSet.Tables[0];

//}

//private void button3_Click(object sender, EventArgs e)
//{

//    DataSet ds = new DataSet();

//    ds = (DataSet)dataGridView2.DataSource;

//    ds.WriteXml("test.xml");


//}
//    }
//}