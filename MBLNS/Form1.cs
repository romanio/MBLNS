using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MBLNS.Models;

namespace MBLNS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Filter = "PVT File|*.*";

            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                OilPVT pvt = new OilPVT();
                pvt.ReadFromFile(FileDialog.FileName);

                chart1.Series[0].Points.AddXY(10, 10);
                chart1.Series[0].Points.AddXY(40, 40);
            }

        }
    }
}
