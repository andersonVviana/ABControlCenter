using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace ADForm
{
    public partial class ADForm : Form
    {
        private float initialFormWidth;
        private float initialPanelWidth;
        public ADForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            txtSearchUser.KeyDown += new KeyEventHandler(txtSearchUser_KeyDown);
            listView1.View = View.List;

            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;

            label1.Visible = true;

            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logo.ico");

            // Verifica se o arquivo de �cone existe
            if (File.Exists(iconPath))
            {
                notifyIcon1.Icon = new Icon(iconPath);
            }
            else
            {
                // Caso o arquivo n�o seja encontrado, use um �cone padr�o
                notifyIcon1.Icon = SystemIcons.Application;
            }

            notifyIcon1.BalloonTipText = "AD App foi minimizado para a �rea de notifica��o";
            notifyIcon1.BalloonTipTitle = "Arcade Beauty AD App";
            notifyIcon1.Text = "Arcade Beauty - Gerenciador do Active Directory";
            notifyIcon1.Visible = false;

            // Evento para restaurar o formul�rio ao clicar no �cone da bandeja
            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;


        }

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParan, int lParam);

        private void Form1_Load(object sender, EventArgs e)
        {
            iconButton3.Enabled = false;
            iconButton2.Enabled = false;
            iconButton4.Enabled = false;
            iconButton1.Enabled = false;

            initialFormWidth = this.Width;
            initialPanelWidth = panel2.Width;
        }

        #region SearchUser

        private void txtSearchUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Pegar o texto da TextBox e buscar informa��es no AD
                string searchUser = txtSearchUser.Text;
                FetchADUserInfo(searchUser);
                iconButton3.Enabled = true;
                iconButton2.Enabled = true;
                iconButton1.Enabled = true;
                iconButton4.Enabled = false;
            }
        }

        #endregion

        #region ADUsersInfo
        private void FetchADUserInfo(string username)
        {
            try
            {
                // Cria uma conex�o com o AD
                DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://mappel.local");
                DirectorySearcher searcher = new DirectorySearcher(directoryEntry);

                // Filtro para encontrar o usu�rio com base no nome fornecido
                searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
                SearchResult result = searcher.FindOne();

                if (result != null)
                {
                    // Preencher as TextBoxes com as informa��es do AD, verificando antes se o valor existe
                    txtbName.Text = result.Properties["givenName"].Count > 0 ? result.Properties["givenName"][0].ToString() : string.Empty;
                    txtbSurname.Text = result.Properties["sn"].Count > 0 ? result.Properties["sn"][0].ToString() : string.Empty;
                    txtbDisplayName.Text = result.Properties["displayName"].Count > 0 ? result.Properties["displayName"][0].ToString() : string.Empty;
                    txtbDescription.Text = result.Properties["description"].Count > 0 ? result.Properties["description"][0].ToString() : string.Empty;
                    txtbOffice.Text = result.Properties["physicalDeliveryOfficeName"].Count > 0 ? result.Properties["physicalDeliveryOfficeName"][0].ToString() : string.Empty;
                    txtbNumber.Text = result.Properties["telephoneNumber"].Count > 0 ? result.Properties["telephoneNumber"][0].ToString() : string.Empty;
                    txtbEmail.Text = result.Properties["mail"].Count > 0 ? result.Properties["mail"][0].ToString() : string.Empty;
                    txtbStreet.Text = result.Properties["streetAddress"].Count > 0 ? result.Properties["streetAddress"][0].ToString() : string.Empty;
                    txtbCity.Text = result.Properties["l"].Count > 0 ? result.Properties["l"][0].ToString() : string.Empty;
                    txtbState.Text = result.Properties["st"].Count > 0 ? result.Properties["st"][0].ToString() : string.Empty;
                    txtbPostaCode.Text = result.Properties["postalCode"].Count > 0 ? result.Properties["postalCode"][0].ToString() : string.Empty;
                    txtbCountry.Text = result.Properties["co"].Count > 0 ? result.Properties["co"][0].ToString() : string.Empty;
                    txtbLogonName.Text = result.Properties["sAMAccountName"].Count > 0 ? result.Properties["sAMAccountName"][0].ToString() : string.Empty;
                    txtbDomain.Text = result.Properties["userPrincipalName"].Count > 0 ? "@" + result.Properties["userPrincipalName"][0].ToString().Split('@')[1] : string.Empty;
                    txtbTitle.Text = result.Properties["title"].Count > 0 ? result.Properties["title"][0].ToString() : string.Empty;
                    txtbCompany.Text = result.Properties["company"].Count > 0 ? result.Properties["company"][0].ToString() : string.Empty;
                    txtbManager.Text = result.Properties["manager"].Count > 0 ? GetManagerName(result.Properties["manager"][0].ToString()) : string.Empty;
                    txtbDeparment.Text = result.Properties["department"].Count > 0 ? result.Properties["department"][0].ToString() : string.Empty;

                    string accountStatus = GetAccountStatus(result);
                    txtStatus.Text = accountStatus;

                    CheckPasswordExpiration(result);

                    // Preencher ListView com grupos de que o usu�rio faz parte (ordenados por ordem alfab�tica)
                    listView1.Items.Clear();
                    if (result.Properties["memberOf"].Count > 0)
                    {
                        var groups = result.Properties["memberOf"].Cast<string>()
                                       .Select(group => group.Split(',')[0].Replace("CN=", ""))
                                       .OrderBy(group => group);
                        foreach (var group in groups)
                        {
                            listView1.Items.Add(new ListViewItem(group));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Usu�rio n�o encontrado no Active Directory.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar informa��es do AD: {ex.Message}");
            }
        }
        #endregion

        #region ManagerName
        private string GetManagerName(string managerDn)
        {
            try
            {
                DirectoryEntry managerEntry = new DirectoryEntry($"LDAP://{managerDn}");
                return managerEntry.Properties["displayName"].Value.ToString();
            }
            catch
            {
                return "Gerente n�o encontrado";
            }
        }
        #endregion

        #region AcconutStatus

        private string GetAccountStatus(SearchResult result)
        {
            int userAccountControl = result.Properties["userAccountControl"].Count > 0 ? (int)result.Properties["userAccountControl"][0] : 0;
            long lockoutTime = result.Properties["lockoutTime"].Count > 0 ? (long)result.Properties["lockoutTime"][0] : 0;

            // Verificar se o usu�rio est� desativado
            if ((userAccountControl & 0x0002) != 0)
            {
                txtStatus.BackColor = Color.Red; // Fundo Vermelho
                txtStatus.ForeColor = Color.White; // Letra Branca
                return " Desativado";
            }
            // Verificar se o usu�rio est� bloqueado
            else if (lockoutTime > 0)
            {
                txtStatus.BackColor = Color.Orange; // Fundo Laranja
                txtStatus.ForeColor = Color.White; // Letra Branca
                return " Bloqueado";
            }
            else
            {
                txtStatus.BackColor = Color.Green; // Fundo Verde
                txtStatus.ForeColor = Color.White; // Letra Branca
                return " Ativo";
            }
        }

        #endregion

        #region PasswordExpiration
        private void CheckPasswordExpiration(SearchResult result)
        {
            const int daysUntilExpirationThreshold = 7; // N�mero de dias para considerar "perto da expira��o"

            // Verificar se o usu�rio est� desativado
            int userAccountControl = result.Properties["userAccountControl"].Count > 0 ? (int)result.Properties["userAccountControl"][0] : 0;
            bool isDisabled = (userAccountControl & 0x0002) != 0; // 0x0002 = ACCOUNTDISABLE

            // Se o usu�rio estiver desabilitado, limpar a textBox e sair
            if (isDisabled)
            {
                txtExpiration.Text = string.Empty;
                txtExpiration.BackColor = Color.White; // Cor padr�o
                txtExpiration.ForeColor = Color.Black; // Cor padr�o
                return;
            }

            // Obt�m a data da �ltima altera��o de senha
            long pwdLastSet = result.Properties["pwdLastSet"].Count > 0 ? (long)result.Properties["pwdLastSet"][0] : 0;

            // Calcular se a senha nunca expira
            bool passwordNeverExpires = (userAccountControl & 0x10000) != 0; // 0x10000 = PASSWORD_DONT_EXPIRE

            // Calcular a data de expira��o da senha
            int maxPwdAge = 90; // Exemplo: 90 dias (ajuste conforme necess�rio)
            DateTime passwordSetDate = DateTime.FromFileTime(pwdLastSet);
            DateTime passwordExpirationDate = passwordSetDate.AddDays(maxPwdAge);
            int daysUntilExpiration = (passwordExpirationDate - DateTime.Now).Days;

            // Exibir a quantidade de dias at� a expira��o
            if (passwordNeverExpires)
            {
                txtExpiration.Text = "Senha nunca expira";
                txtExpiration.BackColor = Color.Orange; // Fundo Laranja
                txtExpiration.ForeColor = Color.White; // Letra Branca
            }
            else
            {
                txtExpiration.Text = daysUntilExpiration > 0 ? $" {daysUntilExpiration} dias" : "Expirada";

                // Configura cores para a TextBox de expira��o de senha
                if (daysUntilExpiration < daysUntilExpirationThreshold && daysUntilExpiration >= 0)
                {
                    txtExpiration.BackColor = Color.Red; // Fundo Vermelho
                    txtExpiration.ForeColor = Color.White; // Letra Branca
                }
                else
                {
                    txtExpiration.BackColor = Color.White; // Fundo Branco (ou outra cor padr�o)
                    txtExpiration.ForeColor = Color.Black; // Letra Preta (ou outra cor padr�o)
                }
            }
        }

        #endregion

        #region BlockUser

        private void iconButton3_Click(object sender, EventArgs e)
        {
            string username = txtSearchUser.Text;

            var confirmDesForm = new ConfirmDeactivationForm(username);
            var confirmActForm = new ConfirmActivationForm(username);

            if (IsUserDisabled(username)) // M�todo que verifica se o usu�rio est� desabilitado
            {
                using (var confirmForm = new ConfirmActivationForm(username))
                {
                    confirmForm.ShowDialog(); // Mostra o pop-up como um di�logo

                    if (confirmForm.IsConfirmed)
                    {
                        ActivateUser(username);
                        MessageBox.Show($"Usu�rio {username} ativado com sucesso.");
                    }
                }
            }
            else
            {
                using (var confirmForm = new ConfirmDeactivationForm(username))
                {
                    confirmForm.ShowDialog(); // Mostra o pop-up como um di�logo

                    if (confirmForm.IsConfirmed)
                    {
                        DeactivateUser(username);
                        MessageBox.Show($"Usu�rio {username} desativado com sucesso.");
                    }
                }
            }
        }

        private bool IsUserDisabled(string username)
        {
            // Similar � l�gica de busca do usu�rio
            using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://mappel.local"))
            using (DirectorySearcher searcher = new DirectorySearcher(directoryEntry))
            {
                searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
                SearchResult result = searcher.FindOne();

                if (result != null)
                {
                    int userAccountControl = (int)result.Properties["userAccountControl"][0];
                    return (userAccountControl & 0x0002) != 0; // Verifica a flag ACCOUNTDISABLE
                }
            }

            return false; // Usu�rio n�o encontrado ou n�o est� desabilitado
        }

        private void DeactivateUser(string username)
        {
            try
            {
                // Cria uma conex�o com o Active Directory
                using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://mappel.local"))
                {
                    // Cria um DirectorySearcher para encontrar o usu�rio
                    using (DirectorySearcher searcher = new DirectorySearcher(directoryEntry))
                    {
                        searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
                        SearchResult result = searcher.FindOne();

                        if (result != null)
                        {
                            // Obt�m o objeto do usu�rio
                            using (DirectoryEntry userEntry = result.GetDirectoryEntry())
                            {
                                // Desativa o usu�rio
                                int userAccountControl = (int)userEntry.Properties["userAccountControl"].Value;
                                userAccountControl |= 0x0002; // 0x0002 = ACCOUNTDISABLE
                                userEntry.Properties["userAccountControl"].Value = userAccountControl;

                                // Salva as altera��es
                                userEntry.CommitChanges();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Usu�rio n�o encontrado no Active Directory.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao desativar o usu�rio: {ex.Message}");
            }
        }

        #endregion

        #region ActivateUser
        private void ActivateUser(string username)
        {
            try
            {
                // Cria uma conex�o com o Active Directory
                using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://mappel.local"))
                {
                    // Cria um DirectorySearcher para encontrar o usu�rio
                    using (DirectorySearcher searcher = new DirectorySearcher(directoryEntry))
                    {
                        searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
                        SearchResult result = searcher.FindOne();

                        if (result != null)
                        {
                            // Obt�m o objeto do usu�rio
                            using (DirectoryEntry userEntry = result.GetDirectoryEntry())
                            {
                                // Ativa o usu�rio
                                int userAccountControl = (int)userEntry.Properties["userAccountControl"].Value;
                                userAccountControl &= ~0x0002; // Remove a flag ACCOUNTDISABLE
                                userEntry.Properties["userAccountControl"].Value = userAccountControl;

                                // Salva as altera��es
                                userEntry.CommitChanges();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Usu�rio n�o encontrado no Active Directory.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao ativar o usu�rio: {ex.Message}");
            }
        }

        #endregion

        #region UnlockUser
        private void UnlockUser(string username)
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
                                // Desbloqueia o usu�rio independentemente do valor de lockoutTime
                                userEntry.Properties["lockoutTime"].Value = 0; // Reseta o valor de lockoutTime
                                userEntry.CommitChanges();
                                MessageBox.Show($"Usu�rio {username} desbloqueado com sucesso.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Usu�rio n�o encontrado no Active Directory.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao desbloquear o usu�rio: {ex.Message}");
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            string username = txtSearchUser.Text; // Obt�m o nome de usu�rio do textbox

            if (IsUserLocked(username)) // Verifica se o usu�rio est� bloqueado
            {
                using (var desblockForm = new DesblockUserForm(username))
                {
                    desblockForm.ShowDialog(); // Mostra o formul�rio de confirma��o

                    if (desblockForm.IsConfirmed)
                    {
                        UnlockUser(username); // Desbloqueia o usu�rio se confirmado
                    }
                }
            }
            else
            {
                MessageBox.Show("Usu�rio n�o est� bloqueado.");
            }
        }

        private bool IsUserLocked(string username)
        {
            using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://mappel.local"))
            using (DirectorySearcher searcher = new DirectorySearcher(directoryEntry))
            {
                searcher.Filter = $"(&(objectClass=user)(sAMAccountName={username}))";
                SearchResult result = searcher.FindOne();

                if (result != null)
                {
                    // Verifica se o "lockoutTime" � maior que 0, indicando que o usu�rio est� bloqueado
                    if (result.Properties["lockoutTime"].Count > 0)
                    {
                        long lockoutTime = (long)result.Properties["lockoutTime"][0];
                        return lockoutTime > 0; // Retorna true se o usu�rio estiver bloqueado
                    }
                }
            }

            return false;
        }

        #endregion

        #region ChangePassword
        private void iconButton1_Click(object sender, EventArgs e)
        {
            // Pegue o nome do usu�rio digitado no textbox de busca, por exemplo
            string username = txtSearchUser.Text;

            // Verifica se o nome de usu�rio foi preenchido
            if (!string.IsNullOrEmpty(username))
            {
                // Instancia e exibe o ChangePasswordForm, passando o nome do usu�rio
                ChangePasswordForm changePasswordForm = new ChangePasswordForm(username);
                changePasswordForm.ShowDialog(); // Abre o formul�rio como modal
            }
            else
            {
                MessageBox.Show("Por favor, insira um nome de usu�rio.");
            }
        }

        #endregion

        #region UserGroups
        public void UpdateUserGroups(List<string> groups)
        {
            listView1.Items.Clear();
            groups.Sort();
            foreach (string group in groups)
            {
                listView1.Items.Add(new ListViewItem(group));
            }
        }
        #endregion

        #region AddUser
        private void iconButton4_Click(object sender, EventArgs e)
        {
            ChoiseUserADForms choiseForm = new ChoiseUserADForms(this);
            choiseForm.Show();
        }

        #endregion

        #region NotifyIcon
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            // Restaura o formul�rio ao clicar duas vezes no �cone da bandeja
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        #endregion

        #region MinimizeIcon

        private void iconButton7_Click(object sender, EventArgs e)
        {
            // Minimiza o formul�rio e o esconde
            this.WindowState = FormWindowState.Minimized;
            this.Hide();

            // Exibe o NotifyIcon e a mensagem de bal�o
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(1000); // Exibe a mensagem por 1 segundo
        }

        #endregion

        #region Panels
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        #endregion
    }
}
