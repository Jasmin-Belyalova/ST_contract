using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Student_Contract
{
    public partial class InfoAboutStudentContracts : Form
    {
        String id;
        public InfoAboutStudentContracts(String accountId)
        {
            InitializeComponent();
            id = accountId;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;user=root;password=root;database=contract");
            try
            {

                connection.Open();
                MySqlDataAdapter SDA = new MySqlDataAdapter($"SELECT contract.ContractId, student.FIO, student.DateOfBirth, vacancy.VacancyId, vacancy.VacancyName, contract.DateOfForming, contract.Status FROM contract JOIN student ON contract.StudentId = student.StudentId join vacancy on contract.VacancyId = vacancy.VacancyId WHERE contract.Status = 'Запрос отправлен' AND DATE(contract.DateOfForming) >= '{GetDate(dateFromBox.Value)}' AND DATE(contract.DateOfForming) <= '{GetDate(dateTo.Value)}' AND contract.StudentId = '{StudID()}';", connection);

                DataTable DATA = new DataTable();
                SDA.Fill(DATA);
                requests.DataSource = DATA;
                connection.Close();

                foreach (DataGridViewRow row in requests.SelectedRows)
                {
                    if (!row.IsNewRow)
                        requests.Rows.Remove(row);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }


        }
        public String GetDate(DateTime date)
        {
            var dateForming = date.ToString().Substring(0, 10).Split('.');
            return dateForming[2] + '-' + dateForming[1] + '-' + dateForming[0];
        }


        public string StudID()
        {
            DB db = new DB();

            DataTable table = new DataTable();// таблица с данными

            MySqlDataAdapter adapter = new MySqlDataAdapter();// адаптер данных

            MySqlCommand command = new MySqlCommand("SELECT StudentID FROM student WHERE AccountId = @accountId", db.GetConnection());// запрос

            command.Parameters.Add("@accountId", MySqlDbType.VarChar).Value = id;

            adapter.SelectCommand = command;// установка адаптера на новый запрос
            adapter.Fill(table);// заполнение таблицы

            DataRow row = table.Rows[0];

            String ID = row["StudentID"].ToString();

            return ID;

        }
    }
}
