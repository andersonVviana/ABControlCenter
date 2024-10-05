using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ADForm
{
    public partial class ChangePasswordForm : Form
    {
        private string _username;
        public bool IsConfirmed { get; private set; }

        public ChangePasswordForm(string username)
        {
            InitializeComponent();
            _username = username;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            string newPassword = textBox1.Text;
            string confirmPassword = textBox2.Text;

            // Verifica se as senhas são iguais
            if (newPassword == confirmPassword)
            {
                // Altera a senha no AD
                if (ChangeUserPassword(_username, newPassword))
                {
                    MessageBox.Show("Senha alterada com sucesso.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erro ao alterar a senha no Active Directory.");
                }
            }
            else
            {
                // Exibe mensagem de erro caso as senhas sejam diferentes
                MessageBox.Show("Senhas não estão iguais. Favor, digitar novamente.");
            }
        }

        private bool ChangeUserPassword(string username, string newPassword)
        {
            try
            {
                using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://mappel.local"))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(directoryEntry))
                    {
                        searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
                        SearchResult result = searcher.FindOne();

                        if (result != null)
                        {
                            using (DirectoryEntry userEntry = result.GetDirectoryEntry())
                            {
                                // Altera a senha do usuário
                                userEntry.Invoke("SetPassword", new object[] { newPassword });
                                userEntry.CommitChanges();
                                return true; // Senha alterada com sucesso
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar a senha: {ex.Message}");
            }
            return false; // Falha ao alterar a senha
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }
    }
}

