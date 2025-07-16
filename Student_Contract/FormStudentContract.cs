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
    public partial class FormStudentContract : Form
    {
        String id;
        public FormStudentContract(String accountId)
        {
            InitializeComponent();
            id = accountId;
            GetInfoAboutUser();
            getCompanies();
        }

        public void GetInfoAboutUser()
        {
            DB db = new DB();

            DataTable table = new DataTable();// таблица с данными

            MySqlDataAdapter adapter = new MySqlDataAdapter();// адаптер данных

            MySqlCommand command = new MySqlCommand("SELECT * FROM `student` WHERE `AccountId` = @accountId", db.GetConnection());// запрос

            command.Parameters.Add("@accountId", MySqlDbType.VarChar).Value = id;

            adapter.SelectCommand = command;// установка адаптера на новый запрос
            adapter.Fill(table);// заполнение таблицы

            DataRow row = table.Rows[0];
            String FIO = row["FIO"].ToString();
            var ListFio = FIO.Split(' ');
            nameBox.Text = ListFio[0];
            surnameBox.Text = ListFio[1];
            secondSurnameBox.Text = ListFio[2];
            dateOfBirth.Text = row["DateOfBirth"].ToString().Substring(0, 10);

        }

        private void getCompanies()
        {
            DB db = new DB();// класс БД

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT CompanyName FROM company;", db.GetConnection());

            adapter.SelectCommand = command;// выполение запроса

            adapter.Fill(table);

            foreach (DataRow row in table.Rows)
            {
                companiesBox.Items.Add(row[0].ToString());// добавление всех вариантов удаления из БД
            }

        }

        private void getVacancies()
        {
            DB db = new DB();// класс БД

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT VacancyName FROM vacancy WHERE CompanyId = (SELECT CompanyId FROM company WHERE CompanyName	= @companyId) ;", db.GetConnection());

            command.Parameters.Add("@companyId", MySqlDbType.VarChar).Value = companiesBox.Text;

            adapter.SelectCommand = command;// выполение запроса

            adapter.Fill(table);

            foreach (DataRow row in table.Rows)
            {
                vacanciesBox.Items.Add(row[0].ToString());// добавление всех вариантов удаления из БД
            }

        }


        private void formContractButton_Click(object sender, EventArgs e)
        {

            if (Tselevik() == "False")
            {
                if (StatusStudent() == "Продолжает обучение")
                {

                    if (Sireis.Text == "")
                    {
                        MessageBox.Show("Введите серию паспорта");
                        return;
                    }

                    if (Number.Text == "")
                    {
                        MessageBox.Show("Введите номер паспорта");
                        return;
                    }

                    if (whoAndWhenTextBox.Text == "")
                    {
                        MessageBox.Show("Введите кем и когда выдан паспорт");
                        return;
                    }
                    if (adressBox.Text == "")
                    {
                        MessageBox.Show("Введите адрес проживания");
                        return;
                    }

                    if (companiesBox.Text == "")
                    {
                        MessageBox.Show("Выберите компанию");
                        return;
                    }

                    if (vacanciesBox.Text == "")
                    {
                        MessageBox.Show("Выберите вакансию");
                        return;
                    }


                    DB db = new DB();

                    DataTable table = new DataTable();// таблица с данными

                    MySqlDataAdapter adapter = new MySqlDataAdapter();// адаптер данных

                    MySqlCommand command = new MySqlCommand("INSERT INTO `personaldata` (`Series`, `Number`, `WhoAndWhen`,`Adress`) VALUES (@series, @number, @whoAndWhen, @adress);" +
                            "SELECT @@identity;" +
                            "UPDATE `student` SET `PersonalDataId`=@@identity WHERE `AccountId`=@accountId", db.GetConnection());// запрос


                    command.Parameters.Add("@series", MySqlDbType.VarChar).Value = Sireis.Text;
                    command.Parameters.Add("@number", MySqlDbType.VarChar).Value = Number.Text;
                    command.Parameters.Add("@whoAndWhen", MySqlDbType.VarChar).Value = whoAndWhenTextBox.Text;
                    command.Parameters.Add("@adress", MySqlDbType.VarChar).Value = adressBox.Text;
                    command.Parameters.Add("@accountId", MySqlDbType.VarChar).Value = id;

                    Contract();

                    db.openConnection();// открываем подключение к БД

                    if (command.ExecuteNonQuery() != 0)
                    {
                        MessageBox.Show("Договор отправлен на утверждение");
                        (sender as Button).Enabled = false;
                    }
                    else
                        MessageBox.Show("Ошибка формирования договора");
                    db.closeConnection();
                }
                else
                {
                    MessageBox.Show("Вы не можете сформировать ученический договор, так как вы больше не являетесь студентом");
                    (sender as Button).Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("У вас уже есть действующий ученический договор");
                (sender as Button).Enabled = false;
            }


        }

        private void companiesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            vacanciesBox.Items.Clear();
            getVacancies();
        }

        private void vacanciesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DB db = new DB();// класс БД

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT Competencies,Requierements FROM vacancy WHERE VacancyName = @vacancyName ;", db.GetConnection());

            command.Parameters.Add("@vacancyName", MySqlDbType.VarChar).Value = vacanciesBox.Items[vacanciesBox.SelectedIndex];

            adapter.SelectCommand = command;// выполение запроса

            adapter.Fill(table);

            foreach (DataRow row in table.Rows)
            {
                competenciesBox.Text = row[0].ToString();
                requerementsBox.Text = row[1].ToString();
            }

        }

        private void Contract()
        {
            DB db = new DB();

            DataTable table = new DataTable();// таблица с данными

            MySqlDataAdapter adapter = new MySqlDataAdapter();// адаптер данных

            MySqlCommand command = new MySqlCommand("INSERT INTO `contract` (`StudentId`, `VacancyId`,`Status`) VALUES (@studentId, @vacancyId,@status);", db.GetConnection());// запрос

            command.Parameters.Add("@studentId", MySqlDbType.VarChar).Value = StudID();
            command.Parameters.Add("@vacancyId", MySqlDbType.VarChar).Value = VacID();
            command.Parameters.Add("@status", MySqlDbType.VarChar).Value = "Запрос сформирован";

            db.openConnection();// открываем подключение к БД

            if (command.ExecuteNonQuery() != 0)
            {
                MessageBox.Show("Ваши данные записаны");
            }
            else
                MessageBox.Show("Ошибка заполнения данных");
            db.closeConnection();
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


        public string VacID()
        {
            DB db = new DB();// класс БД

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT VacancyID FROM vacancy WHERE VacancyName = @vacancyName ;", db.GetConnection());

            command.Parameters.Add("@vacancyName", MySqlDbType.VarChar).Value = vacanciesBox.Items[vacanciesBox.SelectedIndex];

            adapter.SelectCommand = command;// выполение запроса

            adapter.Fill(table);

            DataRow row = table.Rows[0];

            String ID = row["VacancyID"].ToString();

            return ID;

        }

        public string Tselevik()
        {
            DB db = new DB();

            DataTable table = new DataTable();// таблица с данными

            MySqlDataAdapter adapter = new MySqlDataAdapter();// адаптер данных

            MySqlCommand command = new MySqlCommand("SELECT TargetStudent FROM student WHERE AccountId = @accountId", db.GetConnection());// запрос

            command.Parameters.Add("@accountId", MySqlDbType.VarChar).Value = id;

            adapter.SelectCommand = command;// установка адаптера на новый запрос
            adapter.Fill(table);// заполнение таблицы

            DataRow row = table.Rows[0];

            string tsel = row["TargetStudent"].ToString();

            return tsel;

        }
        public string StatusStudent()
        {
            DB db = new DB();

            DataTable table = new DataTable();// таблица с данными

            MySqlDataAdapter adapter = new MySqlDataAdapter();// адаптер данных

            MySqlCommand command = new MySqlCommand("SELECT Status FROM student WHERE AccountId = @accountId", db.GetConnection());// запрос

            command.Parameters.Add("@accountId", MySqlDbType.VarChar).Value = id;

            adapter.SelectCommand = command;// установка адаптера на новый запрос
            adapter.Fill(table);// заполнение таблицы

            DataRow row = table.Rows[0];

            string tsel = row["Status"].ToString();

            return tsel;

        }
    }
}
