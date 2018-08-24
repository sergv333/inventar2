using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XmlConfiguration;

namespace test
{
    public partial class Form1 : Form
    {
        XmlElement xRoot;
        XmlDocument xDoc = new XmlDocument();
        string id;
        int count = 0;//счётчик номера инвентаризации
        int count1 = 0;//счётчик номера закрытия инвентаризации
        private TcpListener tcpListener;
        private Thread listenThread;
        public string seregja = "";
        string connectionString =

                    "Provider=SQLOLEDB;"
                + "Data Source=MBN01_sfkdb01;"
                + "Initial Catalog=TambovSFKPlant;"
                + "User Id=ASUTPsqlUserR;"
                + "Password =Asutp2016ROnly;";
        DataSet ds = new DataSet(); XmlDocument doc = new XmlDocument();
        public Form1()
        {
            InitializeComponent();
            //     label7.Text ="Пользователь: " + WindowsIdentity.GetCurrent().Name;// + " \r " + DateTime.Now.ToString(); // Получение имени учетной записи и ее вывод

            #region Settings
            // Читаем настройки в XML-файле.
            ReadSettings();
           // настройка ip
            label11.Text = GetSetting(xRoot, "ip");
            // настройка port
            label10.Text = GetSetting(xRoot, "port");
           // Подсчет количества запросов инвентарихзаций
            count = Convert.ToInt32(GetSetting(xRoot, "N"));
            count1 = Convert.ToInt32(GetSetting(xRoot, "c"));
            //  label13.Text = "№ Ивентаризации:" + count.ToString() + "    Пользователь: " + WindowsIdentity.GetCurrent().Name; ;
            if (count != count1)
            {
                read();
                MessageBox.Show("Незаконченная инвентаризация: "+count);
                label1.Text = "Количество паллет инвентаризации: " + dataGridView1.RowCount.ToString();
                label7.Text = "Количество паллет не прошедшие инвентаризацию: " + count_red;
                label2.Text = "Количество паллет прошедшие инвентаризацию: " + count_green;
                zapros.Enabled = false;
                label13.Text = "№ Ивентаризации:" + count.ToString() + "    Пользователь: " + WindowsIdentity.GetCurrent().Name;
            }
           
        }
        // Читаем xml-ку.
        private void ReadSettings()
        {
            // Указываю активную директорию exe-шника.
            string directory = Directory.GetCurrentDirectory();
            // Читаем документ.
            xDoc.Load(directory + "/link/settings.xml");
            // Читаем содержимое.
            xRoot = xDoc.DocumentElement;
        }

        // Берем нужную настройку.
        private string GetSetting(XmlElement _xRoot, string _setting)
        {
            string result = ""; // Результат поиска.
            // Перебираем все элементы.
            foreach (XmlNode xSetting in _xRoot)
            {
                // Ищем нужную нам настройку.
                if (xSetting.Name == _setting)
                {
                    // Берем ее.
                    result = xSetting.InnerText;
                }
            }
            // И выводим.
            return result;
        }

        // Изменение параметра в настройках.
        private void ChangeSetting(XmlElement _xRoot, string _setting, string _value)
        {
            foreach (XmlNode xSetting in _xRoot)
            {
                // Ищем нужную нам настройку.
                if (xSetting.Name == _setting)
                {
                    // Берем ее.
                    xSetting.InnerText = _value;
                }
            }
        }

        // Сохранение настроек в XML.
        private void SaveSettings()
        {
            // Берем все активные значения.

            // Настройка отчета по простоям.
            // Список получаетелей.
            ChangeSetting(xRoot, "ip", label11.Text);
            // Время начала считывания данных и отправки письма по умолчанию.
            ChangeSetting(xRoot, "port", label10.Text);
            ChangeSetting(xRoot, "N", count.ToString());
            ChangeSetting(xRoot, "c", count1.ToString());
            // Указываю активную директорию exe-шника.
            string directory = Directory.GetCurrentDirectory();

            xDoc.Save(directory + "/link/settings.xml");
        }

        public List<string[]> data1;
      
        public void UploadProductsListFromDB() // Выгрузка списка продуктов в таблицу
        {

            //     string queryString = "SELECT TOP 15 [Synonym].[description],w.[Synonym_ID],w.[id],w.[Position_ID],w.[netWeight],w.[itemCount],w.[palletizingTime]" +
            // "FROM[TambovSFKPlant].[dbo].[Pallet] AS w INNER JOIN Synonym ON w.Synonym_ID=Synonym.[id]" +
            //"where w.[Position_ID] in(130,131)" + " and w.[ToDestination_ID] in(130,131) order by [Synonym].[description] ";

            //     ds.Clear(); // Очистка данных о продукте и списка продуктов

            //     using (OleDbConnection con = new OleDbConnection(connectionString)) // Параметры соединения
            //     {
            //         OleDbDataAdapter dataadapter = new OleDbDataAdapter(queryString, con); // Запрос

            //         try
            //         {
            //             con.Open(); // Открытие соединения

            //             dataadapter.Fill(ds, "Products_Table"); // Полученные данные кладем в "Products_Table" в 
            //         }                                           // DataSet

            //         catch (Exception ex)
            //         {
            //             MessageBox.Show(ex.ToString());
            //         }

            //         finally
            //         {
            //             con.Close(); // Закрываем соединение

            //             dataGridView1.DataSource = ds; // Указываем источник данных для таблицы
            //             dataGridView1.DataMember = "Products_Table"; // Указываем таблицу в DataSet, откуда брать данные

            //             //for (int i = 0; i < dataGridView2.Columns.Count; i++)
            //             //{
            //             //    dataGridView2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //             //}




            //             dataGridView1.Columns[0].HeaderText = "Имя синонима"; // Задаем имя первого столбца
            //             dataGridView1.Columns[1].HeaderText = "SFK-ID"; // Задаем имя второго столбца
            //             dataGridView1.Columns[2].HeaderText = "ID-паллеты"; // Задаем имя второго столбца
            //             dataGridView1.Columns[3].HeaderText = "Позиция"; // Задаем имя второго столбца
            //             dataGridView1.Columns[4].HeaderText = "Вес нетто"; // Задаем имя второго столбца
            //             dataGridView1.Columns[5].HeaderText = "Количество"; // Задаем имя второго столбца
            //             dataGridView1.Columns[6].HeaderText = "Дата паллетирования"; // Задаем имя второго столбца
            //             dataGridView2.Columns.Add("description", "Имя синонима");
            //             dataGridView2.Columns.Add("Synonym_ID", "SFK-ID");
            //             dataGridView2.Columns.Add("id_Pallet", "ID-паллеты");
            //             dataGridView2.Columns.Add("Position_ID", "Позиция");
            //             dataGridView2.Columns.Add("netWeight", "Вес нетто");
            //             dataGridView2.Columns.Add("itemCount", "Количество");
            //             dataGridView2.Columns.Add("palletizingTime", "Дата паллетирования");
            string connectionString =
       "Data Source=MBN01_sfkdb01;"
      + "Initial Catalog=TambovSFKPlant;"
      + "User Id=ASUTPsqlUserR;"
      + "Password =Asutp2016ROnly;";

            string queryString = "SELECT  [Synonym].[description],w.[Synonym_ID],w.[id],w.[Position_ID],w.[netWeight],w.[itemCount],w.[palletizingTime]" +
        "FROM[TambovSFKPlant].[dbo].[Pallet] AS w INNER JOIN Synonym ON w.Synonym_ID=Synonym.[id]" +
       "where w.[Position_ID] in(130,131)" + " and w.[ToDestination_ID] in(130,131) order by [Synonym].[description] ";
            using (SqlConnection myConnection = new SqlConnection(connectionString)) // Параметры соединения
            {

                SqlCommand command = new SqlCommand(queryString, myConnection);
                try
                {


                    myConnection.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                finally
                {

                    SqlDataReader reader = command.ExecuteReader();
                    //  List<string[]> data1 = new List<string[]>();
                    //List<string[]> data = new List<string[]>();
                    data1 = new List<string[]>();
                    //data1.Clear();
                    while (reader.Read())
                    {
                        data1.Add(new string[7]);

                        data1[data1.Count - 1][0] = reader[0].ToString();
                        data1[data1.Count - 1][1] = reader[1].ToString();
                        data1[data1.Count - 1][2] = reader[2].ToString();
                        data1[data1.Count - 1][3] = reader[3].ToString();
                        data1[data1.Count - 1][4] = reader[4].ToString();
                        data1[data1.Count - 1][5] = reader[5].ToString();
                        data1[data1.Count - 1][6] = reader[6].ToString();
                    }

                    reader.Close();

                    myConnection.Close();

                    foreach (string[] s in data1)
                        dataGridView1.Rows.Add(s);
                }
                //dataGridView2.Columns.Add("description", "Имя синонима");
                //dataGridView2.Columns.Add("Synonym_ID", "SFK-ID");
                //dataGridView2.Columns.Add("id_Pallet", "ID-паллеты");
                //dataGridView2.Columns.Add("Position_ID", "Позиция");
                //dataGridView2.Columns.Add("netWeight", "Вес нетто");
                //dataGridView2.Columns.Add("itemCount", "Количество");
                //dataGridView2.Columns.Add("palletizingTime", "Дата паллетирования");
               
                label1.Text = "Количество паллет инвентаризации: " + dataGridView1.RowCount.ToString();
                label2.Text = "Количество паллет прошедшие инвентаризацию: " + dataGridView2.RowCount.ToString();
                 
            }
        }


        public void cleralist()
        {
            if (data1 == null)
            {
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
            }
            else { data1.Clear(); dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
            }
           
           
        }



        private void zapros_Click(object sender, EventArgs e)
        {
            
            count++;//при созданиинового запроса на инвентаризацию +1

            // doc.Save("link/Setting_Server.xml");
            SaveSettings();
            // numcount.Text = counter.ToString();//вывод на форму номер запроса инвентаризации
            label13.Text = "№ Ивентаризации:" + count.ToString() + "    Пользователь: " + WindowsIdentity.GetCurrent().Name; ;
           UploadProductsListFromDB(); // Выгрузка списка продуктов в таблицу
          // dataGridView1.Rows[0].DefaultCellStyle.BackColor = Color.GreenYellow;
            zapros.Enabled = false;

        }
        public string w ="";
        public string _s1 = "link/Profiles.xml";
        public string _s2 = "link/Profiles2.xml";

        public void Read_Click(DataGridView _dgv, string _ss )
        {
           // int rowsCount = _dgv.Rows.Count;

            List<Profile> ProfileList = new List<Profile>();
            ProfileList = Serializer.Load<List<Profile>>(_ss);

           // dataGridView2.Rows.Clear();

            foreach (Profile prof in ProfileList)
            {
                _dgv.Rows.Add(new object[] { prof.description, prof.Synonym_ID, prof.id_Pallet, prof.Position_ID, prof.netWeight, prof.itemCount, prof.palletizingTime });
    //             dataGridView2.Rows[0].DefaultCellStyle.BackColor = Color.GreenYellow;
            }
            for (int i = 0; i <= _dgv.Rows.Count - 1; i++)
            { //  for (int j = 0; j <= dataGridView2.ColumnCount - 1; j++)

                if (_dgv.Rows[i].Cells[1].Value.ToString() ==w )
                {
                    _dgv.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    count_red++;
                    label7.Text = "Количество паллет не прошедшие инвентаризацию: " + count_red;
                }
             else if (_dgv==dataGridView2)
                {
                    _dgv.Rows[i].DefaultCellStyle.BackColor = Color.GreenYellow;
                    count_green++;
                 //  label2.Text = "Количество паллет прошедшие инвентаризацию: " + count_green;
                }
               
               

            }
        }
        public void save_Click(DataGridView _dgv,string _ss)
        {
                List<Profile> ProfileList = new List<Profile>();

            for(int i = 0; i < _dgv.Rows.Count;i++)
            {
                DataGridViewRow _row = _dgv.Rows[i];

                Profile p = new Profile();
              //  if(_row.Cells[0].Value != null)
                    p.description = Convert.ToString(_row.Cells[0].Value);

                //if (_row.Cells[1].Value != null)
                p.Synonym_ID = Convert.ToString(_row.Cells[1].Value);


                p.id_Pallet = Convert.ToString(_row.Cells[2].Value);
                




             //   if (_row.Cells[3].Value != null)
                    p.Position_ID = Convert.ToString(_row.Cells[3].Value);
                //   if (_row.Cells[4].Value != null)
                p.netWeight = Convert.ToString(_row.Cells[4].Value);
                //   if (_row.Cells[5].Value != null)
                p.itemCount = Convert.ToString(_row.Cells[5].Value);
                //   if (_row.Cells[6].Value != null)
                p.palletizingTime = Convert.ToString(_row.Cells[6].Value);

                ProfileList.Add(p);
            }

            Serializer.Save<List<Profile>>(ProfileList, _ss);
        }
        //писька петра
        [Serializable]
        public class Profile
        {
            public string description;
            public string Synonym_ID;
            public string id_Pallet;
          
            public string Position_ID;
            public string netWeight;
            public string itemCount;
            public string palletizingTime;
        }

        class Serializer
        {

            public static T Load<T>(string _fileName)
            {
                /* Восстанавливаем из файла в файл. */

                TextReader _writer = null;

                try
                {
                    XmlSerializer _Serializer = new XmlSerializer(typeof(T));
                    _writer = new StreamReader(_fileName);

                    return (T)_Serializer.Deserialize(_writer);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return default(T);
                }
                finally
                {
                    if (_writer != null)
                        _writer.Close();
                }
            }

            public static bool Save<T>(T obj, string _file)
            {
                TextWriter _writer = null;
                try
                {
                    XmlSerializer _Serializer = new XmlSerializer(typeof(T));
                    _writer = new StreamWriter(_file);
                    _Serializer.Serialize(_writer, obj);
                    return true;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return false;
                }
                finally
                {
                    if (_writer != null)
                        _writer.Close();
                }

            }
        }


        private void btStartServer_Click(object sender, EventArgs e)
        {
            /*Start listener and kick to new thread to free the UI*/
            tcpListener = new TcpListener(IPAddress.Any, int.Parse(label10.Text));
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
            this.Invoke((MethodInvoker)delegate
            {
                //This unnamed delegate is used to access UI elements on the main thread
                txtTalk.AppendText("Server started...Listening for clients...\r\n");
            });
            btStartServer.Enabled = false; btStopServer.Enabled = true;
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

        int count_green = 0;
        int count_red = 0;
        int counter = 0;
        string u = "";
        string u1 = "";

        private void HandleClientComm(object client)
        {
          

            /*Handles all communication once server receives client*/
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            Byte[] bytes = new Byte[256];
            String data = null;
            string message = u;
            
            // int i;
            int bytesRead;
            bool b = false;
            bytesRead = 0;
            //  data = null;
            //     string s = "";
            string s1 = "№:";
            string s2 = "close";
            // Принимаем данные от клиента в цикле пока не дойдём до конца.
            while ((bytesRead = clientStream.Read(bytes, 0, bytes.Length)) != 0)  //blocking calltrue)
            {
                UnicodeEncoding encoder = new UnicodeEncoding();

                this.Invoke((MethodInvoker)delegate
                {
                    counter++;

                    //textBox4.Clear();
                    //textBox4.AppendText(" Получено: " + encoder.GetString(bytes, 0, bytesRead) + "\r\n" + dataGridView2.Rows.Count + "\r\n" + counter.ToString() + "\r\n");
                    txtTalk.AppendText("Получено:" + encoder.GetString(bytes, 0, bytesRead));
                    if (encoder.GetString(bytes, 0, bytesRead) == s1)
                    {
                        //MessageBox.Show("№");
                        u = s1+ "," + count.ToString() + "," + label1.Text;
                        byte[] data1 = Encoding.Unicode.GetBytes(u);
                        clientStream.Write(data1, 0, data1.Length);
                        txtTalk.AppendText(" номер инвентаризации ");
                    }
                    else if (encoder.GetString(bytes, 0, bytesRead) == s2)
                    {
                        count1 = count; 
                        u = s2+"," + count1.ToString() ;
                        byte[] data1 = Encoding.Unicode.GetBytes(u);
                        clientStream.Write(data1, 0, data1.Length);
                        save();
                        cleralist();
                        
                        
                        SaveSettings();
                        zapros.Enabled = true;
                        count_green = 0;
                        count_red = 0;

                    }
                    else {
                        seregja = label6.Text;
                        label12.Text = seregja;
                        label6.ResetText();//.Clear();
                        label6.Text = (encoder.GetString(bytes, 0, bytesRead));
                        if (seregja == label6.Text)
                        {
                            txtTalk.AppendText("уже был ");
                        }

                        //});
                        try
                        {

                            // Преобразуем данные в ASCII string.
                            data = System.Text.Encoding.Unicode.GetString(bytes, 0, bytesRead);

                            // Преобразуем строку к верхнему регистру.
                            // data = data.ToUpper();

                            // Преобразуем полученную строку в массив Байт.
                            byte[] msg = System.Text.Encoding.Unicode.GetBytes(data);
                            // Отправляем данные обратно клиенту (ответ).
                            //   clientStream.Write(msg, 0, msg.Length);

                            //    UnicodeEncoding encoder1 = new UnicodeEncoding();
                            //this.Invoke((MethodInvoker)delegate
                            //{
                            //dataGridView2.Rows[counter].Cells[4].Value = textBox2.Text;
                            try
                            {
                                for (int i = 0; i <= dataGridView1.Rows.Count - 1; i++)
                                    //  for (int j = 0; j <= dataGridView2.ColumnCount - 1; j++)

                                    if (dataGridView1.Rows[i].Cells[2].Value != null && dataGridView1.Rows[i].Cells[2].Value.ToString() == label6.Text)
                                    {

                                        b = true;
                                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.GreenYellow;
                                        dataGridView1.Rows[i].Selected = true;
                                        // dataGridView2.Rows[i].Cells[4].Value = textBox2.Text;
                                        dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;

                                    }



                                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                                {
                                    object[] items = new object[row.Cells.Count];
                                    for (int i1 = 0; i1 < row.Cells.Count; i1++)
                                    {
                                        items[i1] = row.Cells[i1].Value;


                                    }
                                    dataGridView2.Rows.Add(items);

                                    for (int j = 0; j <= dataGridView2.RowCount - 1; j++)
                                        if (dataGridView2.Rows[j].Cells[2].Value != null && dataGridView2.Rows[j].Cells[2].Value.ToString() == label6.Text)
                                            dataGridView2.Rows[j].DefaultCellStyle.BackColor = Color.GreenYellow;
                                    count_green++;
                                    //  dataGridView2.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;

                                }
                                // label2.Text = "Количество паллет прошедшие инвентаризацию: " + dataGridView2.RowCount.ToString();
                                label2.Text = "Количество паллет прошедшие инвентаризацию: " + count_green;


                            }
                            catch
                            {

                            }
                            if (b == true)
                            {
                                b = true;
                                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                                {
                                    dataGridView1.Rows.Remove(row);

                                }
                                label1.Text = "Количество паллет для инвентаризации: " + dataGridView1.RowCount.ToString();
                                u = "ДА" + "," + label6.Text + "," + count_green + "," + count_red+","+count;
                                byte[] data1 = Encoding.Unicode.GetBytes(u);
                                clientStream.Write(data1, 0, data1.Length);
                                txtTalk.AppendText(" Отправлено: Паллет в списке. ");

                            }
                            else
                            {
                                dataGridView2.Rows.Add(null, null, label6.Text);
                                for (int j = 0; j <= dataGridView2.RowCount - 1; j++)
                                {
                                    if (dataGridView2.Rows[j].Cells[2].Value != null && dataGridView2.Rows[j].Cells[2].Value.ToString() == label6.Text)
                                    { dataGridView2.Rows[j].DefaultCellStyle.BackColor = Color.Red; }
                                }
                                count_red++;
                                label7.Text = "Количество паллет не прошедшие инвентаризацию: " + count_red;
                                //  listBox1.Items.Add(textBox2.Text);
                                u = "НЕТ" + "," + label6.Text + "," + count_green + "," + count_red + "," + count;
                                byte[] data1 = Encoding.Unicode.GetBytes(u);
                                clientStream.Write(data1, 0, data1.Length);
                                txtTalk.AppendText(" Отправлено: Паллет не в списке. ");
                            }



                            //    txtTalk.AppendText("послал клиенту: " + encoder1.GetString(msg, 0, bytesRead) + "\r\n");


                        }
                        catch (Exception ex)
                        {
                            txtTalk.AppendText("ERROR: " + ex.Message + "\r\n");
                            //    break;
                        }

                    }
                });

                if (bytesRead == 0)
                {
                    break;
                }

            }

            this.Invoke((MethodInvoker)delegate
            { clientStream.Close();
                tcpClient.Close();
                txtTalk.AppendText("Client disconnected\r\n");
            });
        }
        
        private void log_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Visible = false;
            tableLayoutPanel3.Visible = false;
            panel2.Dock = DockStyle.Fill;
            panel2.Visible = true;
            btback.Visible = true;
        }

        private void btStopServer_Click(object sender, EventArgs e)
        {
            /*Stops the server
          Note that because stop() will cause the listener to terminate with an exception (see above)
          any code after this statement in this event handler will be skipped*/
            tcpListener.Stop(); btStartServer.Enabled = true; btStopServer.Enabled = false;
            /*DON'T PUT ANYTHING HERE, NEVER EXECUTES*/
        }

        private void btback_Click(object sender, EventArgs e)
        {
            btback.Visible = false;
            tableLayoutPanel2.Visible = true;
            tableLayoutPanel3.Visible = true;
            // panel2.Dock = DockStyle.Fill;
            panel2.Visible = false;
        }
        public void save()
        {
            save_Click(dataGridView1, _s1);
            save_Click(dataGridView2, _s2);
        }
        public void read()
        {
        
             Read_Click(dataGridView1, _s1);
             Read_Click(dataGridView2, _s2);
     

        }
        private void button3_Click(object sender, EventArgs e)
        {
            save_Click(dataGridView1, _s1);
            save_Click(dataGridView2,_s2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
          
            //this.Invoke((MethodInvoker)delegate
            //{
                Read_Click(dataGridView1, _s1);
                Read_Click(dataGridView2, _s2);
            //});
        }
    }
}

#endregion