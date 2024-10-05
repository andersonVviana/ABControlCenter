using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ADForm
{
    public partial class DesblockUserForm : Form
    {
        public bool IsConfirmed { get; private set; }

        public DesblockUserForm(string username)
        {
            InitializeComponent();
            label1.Text = $"Você deseja desbloquear o usuário {username}?";
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            IsConfirmed = true;
            this.Close();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }
    }
}
