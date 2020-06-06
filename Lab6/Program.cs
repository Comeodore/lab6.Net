using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    class Program
    {
        public static void Main()
        {
            string connectionString = @"Data Source=VLADMIER\SQLEXPRESS;Initial Catalog=Lab6;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            int menu = 0;
            do
            {
                Console.WriteLine("Выберите пункт меню:\n" +
                    "1. SQL Запросы\n" +
                    "2. Оформление договора\n" +
                    "3. Выход");
                menu = Convert.ToInt32(Console.ReadLine());
                switch(menu)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("SQL Запросы:");
                        Console.WriteLine("1. Вывести содержимое таблицы Products");
                        ShowData("SELECT * FROM products", connectionString);
                        Decor();
                        Console.WriteLine("2. Вывести содержимое таблицы Customs");
                        ShowData("SELECT * FROM customs", connectionString);
                        Decor();
                        Console.WriteLine("3. Вывести содержимое таблицы Contract");
                        ShowData("SELECT * FROM contract", connectionString);
                        Decor();
                        Console.WriteLine("4. Вывести продукты с ценой больше 220");
                        ShowData("SELECT nameProd FROM products WHERE costProd>220", connectionString);
                        Decor();
                        Console.WriteLine("5. Вставка данных в таблицу Customs");
                        SqlCommand command = new SqlCommand("INSERT INTO customs(country,Tax,VAT) VALUES ('Бразилия', 10, 14)", connection);
                        if (command.ExecuteNonQuery() == 1) Done();
                        Decor();
                        Console.WriteLine("6. Изменение данных в таблице Contract");
                        SqlCommand command1 = new SqlCommand("UPDATE contract SET nameCompanyBuy = 'Плюс' WHERE idContract = 1");
                        if (command1.ExecuteNonQuery() == 1) Done();
                        Decor();
                        Console.WriteLine("7. Удаление данных из таблицы Products");
                        SqlCommand command2 = new SqlCommand("DELETE FROM products WHERE nameProd = 'Дерево'");
                        if (command2.ExecuteNonQuery() == 1) Done();
                        Decor();
                        Console.WriteLine("8. Вывести незадействованные в контрактах продукты");
                        ShowData("SELECT nameProd FROM products WHERE NOT EXISTS " +
                            "(SELECT nameProd FROM contract WHERE products.nameProd = contract.nameProd)", connectionString);
                        Decor();
                        Console.WriteLine("9. Вывести максимальную цену конракта");
                        ShowData("SELECT Max(costProd) AS 'Максимальная цена контракта' FROM contract", connectionString);
                        Decor();
                        Console.WriteLine("10. Вывести среднюю цену продукта");
                        ShowData("SELECT AVG(costProd) AS 'Средняя цена продукта' FROM products", connectionString);
                        Decor();
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("Вы перешли к оформлению договора!\n" +
                            "Ваша компания продает или покупает?\n" +
                            "1. Продает\n" +
                            "2. Покупает");
                        int sellorbuy = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введите название компании поставщика/покупателя");
                        string companyName = Console.ReadLine();
                        string countryCompany;
                        if (sellorbuy == 1)
                            countryCompany = "Украина";
                        else
                        {
                            Console.WriteLine("Укажите страну в которой расположена компания");
                            countryCompany = Console.ReadLine();
                        }
                        int exitWhile = 2;
                        List<Product> products = new List<Product>();
                        do
                        {
                            Console.WriteLine("Введите название товара для поставки/продажи:");
                            ShowData("SELECT * FROM products", connectionString);
                            string product = Console.ReadLine();
                            Console.WriteLine("Введите количество товара:");
                            string quantity = Console.ReadLine();
                            Console.WriteLine("Введите дату поставки товара в формате 'DD/MM/YY':");
                            string delivery = Console.ReadLine();
                            SqlCommand command3 = new SqlCommand("SELECT costProd FROM products WHERE nameProd = @nameProd",connection);
                            command3.Parameters.Add("@nameProd", SqlDbType.VarChar).Value = product;
                            int cost = Convert.ToInt32(command3.ExecuteScalar());
                            products.Add(new Product(product, quantity, cost, delivery));
                            Console.WriteLine("Хотите добавить еще один товар?\n" +
                                "1. Да\n" +
                                "2. Нет");
                            exitWhile = Convert.ToInt32(Console.ReadLine());
                        } while (exitWhile != 2);
                        Console.WriteLine("Укажите адрес доставки:");
                        string deliveryAddress = Console.ReadLine();
                        Console.WriteLine("Укажите номер компании поставщика/покупателя:");
                        string phoneCompany = Console.ReadLine();
                        SqlCommand command4 = new SqlCommand("SELECT Tax FROM customs WHERE country = @country", connection);
                        command4.Parameters.Add("@country", SqlDbType.VarChar).Value = countryCompany;
                        int Tax = Convert.ToInt32(command4.ExecuteScalar());
                        SqlCommand command5 = new SqlCommand("SELECT VAT FROM customs WHERE country = @country", connection);
                        command5.Parameters.Add("@country", SqlDbType.VarChar).Value = countryCompany;
                        int VAT = Convert.ToInt32(command5.ExecuteScalar());
                        string nameProd = "", quantityProd = "", deliveryTime = "";
                        int costProd = 0;
                        foreach (Product product in products)
                        {
                            nameProd += product.nameProd + ",";
                            quantityProd += product.quantityProd + ",";
                            deliveryTime += product.deliveryTime + ",";
                            costProd += product.costProd;
                        }
                        nameProd = nameProd.Remove(nameProd.Length - 1);
                        quantityProd = quantityProd.Remove(quantityProd.Length - 1);
                        deliveryTime = deliveryTime.Remove(deliveryTime.Length - 1);
                        SqlCommand command6 = new SqlCommand("SELECT Max(idContract) FROM contract",connection);
                        int maxid = Convert.ToInt32(command6.ExecuteScalar());
                        SqlCommand commandFinal = new SqlCommand("INSERT INTO contract" +
                            "(idContract,dateContract,nameCompany,countryCompany,nameProd," +
                            "quantityProd,costProd,deliveryTime,deliveryAddress,phoneCompany,Tax,VAT) " +
                            "VALUES(@idContract,@dateContract,@nameCompany,@countryCompany,@nameProd," +
                            "@quantityProd,@costProd,@deliveryTime,@deliveryAddress,@phoneCompany,@Tax,@VAT)", connection);
                        DateTime dateTime = DateTime.Now;
                        commandFinal.Parameters.Add("@idContract", SqlDbType.Int).Value = maxid + 1;
                        commandFinal.Parameters.Add("@dateContract", SqlDbType.VarChar).Value = dateTime.ToString();
                        commandFinal.Parameters.Add("@nameCompany", SqlDbType.VarChar).Value = companyName;
                        commandFinal.Parameters.Add("@countryCompany", SqlDbType.VarChar).Value = countryCompany;
                        commandFinal.Parameters.Add("@nameProd", SqlDbType.VarChar).Value = nameProd;
                        commandFinal.Parameters.Add("@quantityProd", SqlDbType.VarChar).Value = quantityProd;
                        commandFinal.Parameters.Add("@costProd", SqlDbType.Int).Value = costProd;
                        commandFinal.Parameters.Add("@deliveryTime", SqlDbType.VarChar).Value = deliveryTime;
                        commandFinal.Parameters.Add("@deliveryAddress", SqlDbType.VarChar).Value = deliveryAddress;
                        commandFinal.Parameters.Add("@phoneCompany", SqlDbType.VarChar).Value = phoneCompany;
                        commandFinal.Parameters.Add("@Tax", SqlDbType.Int).Value = Tax;
                        commandFinal.Parameters.Add("@VAT", SqlDbType.Int).Value = VAT;
                        if (commandFinal.ExecuteNonQuery() == 1) Console.WriteLine("Договор оформлен");
                        break;
                }
            } while (menu != 3);
            connection.Close();
        }
        public class Product
        {
            public string nameProd { get; set; }
            public string quantityProd { get; set; }
            public int costProd { get; set; }
            public string deliveryTime { get; set; }

            public Product(string NameProd, string QuantityProd, int CostProd, string DeliveryTime)
            {
                this.nameProd = NameProd;
                this.quantityProd = QuantityProd;
                this.costProd = CostProd;
                this.deliveryTime = DeliveryTime;
            }
        }
        public static void Done()
        {
            Console.WriteLine("Done");
        }
        public static void Pause()
        {
            Console.ReadLine();
        }
        public static void Decor()
        {
            Console.WriteLine("########################################################################################");
        }
        public static void ShowData(string sqlCommand, string connectionString)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand, connectionString);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);
            DataTable table = dataset.Tables[0];
            foreach (DataColumn column in table.Columns)
            {
                Console.Write("{0, -20}", column.ColumnName);
            }
            Console.WriteLine();
            foreach (DataRow row in table.Rows)
            {
                var cells = row.ItemArray;
                foreach (object cell in cells)
                {
                    Console.Write("{0,-20}", cell);
                }
                Console.WriteLine();
            }
        }

    }
}
