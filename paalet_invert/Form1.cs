using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace paalet_invert
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
             
            MessageBox.Show("Я - новая кнопка"); button1.BackgroundImage = Properties.Resources.Delete_80_icon_icons_com_57340;
         }
    }
}
