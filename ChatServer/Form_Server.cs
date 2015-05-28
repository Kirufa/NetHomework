using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChatServer
{
    public partial class Form_Server : Form
    {
        public Form_Server()
        {
            InitializeComponent();
            Random r = new Random();
            this.richTextBox1.Text = r.Next(49152,65535).ToString(); 
        }

        

    }
}
