using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            //dataGridView1.Columns.Add("tn_ob0", "Имя синонима");
            //dataGridView1.Columns.Add("tn_ob1", "SFK-ID");
            //dataGridView1.Columns.Add("tn_ob2", "ID-паллеты");
            //dataGridView1.Columns.Add("tn_ob3", "Позиция");
            //dataGridView1.Columns.Add("tn_ob4", "Вес нетто"); // Задаем имя второго столбца
            //dataGridView1.Columns.Add("tn_ob5", "Количество"); // Задаем имя второго столбца
            //dataGridView1.Columns.Add("tn_ob6", "Дата паллетирования"); // Задаем имя второго столбца
             //"Provider=SQLOLEDB;"
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

                    List<string[]> data = new List<string[]>();

                    while (reader.Read())
                    {
                        data.Add(new string[6]);

                        data[data.Count - 1][0] = reader[0].ToString();
                        data[data.Count - 1][1] = reader[1].ToString();
                        data[data.Count - 1][2] = reader[2].ToString();
                        data[data.Count - 1][3] = reader[3].ToString();
                        data[data.Count - 1][4] = reader[4].ToString();
                        data[data.Count - 1][5] = reader[5].ToString();
                    }

                    reader.Close();

                    myConnection.Close();

                    foreach (string[] s in data)
                        dataGridView2.Rows.Add(s);
                }
            }
         }
    }
}
