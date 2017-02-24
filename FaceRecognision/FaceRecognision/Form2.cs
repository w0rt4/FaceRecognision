using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceRecognision
{
    public partial class Form2 : Form
    {
        public string name = "";
        public Form2()
        {
            InitializeComponent();
            
        }

        private void ApproveSampleName(object sender, EventArgs e)
        {
            name = textBox1.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
