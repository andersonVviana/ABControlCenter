using System.DirectoryServices;

namespace ADForm
{
    public partial class ChoiseUserADForms : Form
    {
        public bool IsConfirmed { get; private set; }

        private ADForm mainForm;

        private string selectedOU;

        private ADUserInfo confirmedUser;

        public ChoiseUserADForms(ADForm form)
        {
            InitializeComponent();
            mainForm = form;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;

            if (!string.IsNullOrEmpty(username))
            {
                // Busca as informações do usuário no AD
                var userInfo = FetchADUserInfo(username);

                if (userInfo != null)
                {
                    // Exibe pop-up de confirmação
                    DialogResult result = MessageBox.Show(
                        $"O Usuário é {userInfo.FullName} - Departamento: {userInfo.Department}",
                        "Confirmação de Usuário",
                        MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        // Armazena as informações do usuário confirmado
                        confirmedUser = userInfo;
                        MessageBox.Show("Usuário confirmado. Insira as informações para criar um novo usuário.");
                    }
                }
                else
                {
                    MessageBox.Show("Usuário não encontrado.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, insira um nome de usuário.");
            }
        }

        private ADUserInfo FetchADUserInfo(string username)
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
                                // Extrai as informações do usuário confirmado
                                return new ADUserInfo
                                {
                                    FullName = userEntry.Properties["cn"].Value?.ToString(),
                                    Department = userEntry.Properties["department"].Value?.ToString(),
                                    Description = userEntry.Properties["description"].Value?.ToString(),
                                    Company = userEntry.Properties["company"].Value?.ToString(),
                                    Telephone = userEntry.Properties["telephoneNumber"].Value?.ToString(),
                                    Email = userEntry.Properties["mail"].Value?.ToString(),
                                    OU = userEntry.Path // Caminho da OU onde o usuário está
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar informações do AD: {ex.Message}");
            }

            return null;
        }

        public void CreateUser(string firstName, string lastName, string logonName)
        {
            try
            {
                // Definir o caminho LDAP base para o domínio "mappel.local"
                string ldapBase = "LDAP://arcade-DC01.mappel.local"; // Use o DC correto, no seu caso arcade-DC01

                // Localizar a OU onde o novo usuário será criado
                string ouPath = "OU=Users,DC=mappel,DC=local"; // Ajuste a OU de acordo com sua estrutura no AD

                // Combinar o caminho LDAP base com a OU
                string fullLdapPath = $"{ldapBase}/{ouPath}";

                // Crie o objeto DirectoryEntry para a OU correta
                using (DirectoryEntry ouEntry = new DirectoryEntry(fullLdapPath))
                {
                    // Definir o Distinguished Name (DN) do novo usuário
                    string userCN = $"CN={firstName} {lastName}";
                    DirectoryEntry newUser = ouEntry.Children.Add(userCN, "user");

                    // Definir propriedades obrigatórias
                    newUser.Properties["sAMAccountName"].Value = logonName; // Nome de logon
                    newUser.Properties["userPrincipalName"].Value = $"{logonName}@arcadebeauty.com"; // Principal Name (com domínio arcadebeauty.com)
                    newUser.Properties["givenName"].Value = firstName; // Primeiro nome
                    newUser.Properties["sn"].Value = lastName; // Sobrenome

                    // Outras propriedades opcionais podem ser adicionadas aqui
                    // newUser.Properties["telephoneNumber"].Value = "123-456-7890"; // Exemplo de telefone

                    newUser.CommitChanges(); // Confirmar mudanças

                    // Habilitar a conta do usuário (UserAccountControl 0x200 = habilitar)
                    newUser.Properties["userAccountControl"].Value = 0x200;
                    newUser.CommitChanges(); // Confirmar a habilitação da conta

                    MessageBox.Show("Novo usuário criado com sucesso.");
                }
            }
            catch (DirectoryServicesCOMException ex)
            {
                MessageBox.Show($"Erro ao criar o usuário no AD: {ex.Message} (Código: {ex.ExtendedError}, Tipo: {ex.ExtendedErrorMessage})");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro genérico ao criar o usuário no AD: {ex.Message}");
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

            // Obter os valores das textboxes onde o nome, sobrenome e logon são informados
            string firstName = textBox2.Text;
            string lastName = textBox3.Text;
            string logonName = textBox5.Text;

            // Chamar o método para criar o usuário no AD
            CreateUser(firstName, lastName, logonName);
        }

        public class ADUserInfo
        {
            public string FullName { get; set; }
            public string Department { get; set; }
            public string Description { get; set; }
            public string Company { get; set; }
            public string Telephone { get; set; }
            public string Email { get; set; }
            public string OU { get; set; }
        }
    }
}



