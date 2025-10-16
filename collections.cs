using System;
using System.Collections.Generic;

public class Product
{
    public string Name;
    public decimal Price;
    public int Quantity;

    public Product(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}

public class Order
{
    public int Number;
    public DateTime Date;
    public List<Product> Products;

    public Order(int number, DateTime date)
    {
        Number = number;
        Date = date;
        Products = new List<Product>();
    }

    public decimal GetTotalCost()
    {
        decimal total = 0;
        foreach (Product product in Products)
        {
            total += product.Price * product.Quantity;
        }
        return total;
    }

    public string GetProductsList()
    {
        List<string> productNames = new List<string>();
        foreach (Product product in Products)
        {
            productNames.Add(product.Name);
        }
        return string.Join(", ", productNames);
    }

    public string GetQuantitiesList()
    {
        List<string> quantities = new List<string>();
        foreach (Product product in Products)
        {
            quantities.Add(product.Quantity.ToString());
        }
        return string.Join(", ", quantities);
    }
}

public class Warehouse
{
    public List<Product> Products;

    public Warehouse()
    {
        Products = new List<Product>();
    }

    public void AddProduct(string name, decimal price, int quantity)
    {
        Products.Add(new Product(name, price, quantity));
    }

    public void ShowProducts()
    {
        Console.WriteLine("\nтовары на складе:");
        foreach (Product product in Products)
        {
            Console.WriteLine($"{product.Name} - цена: {product.Price}, количество: {product.Quantity}");
        }
    }

    public bool CheckProduct(string name, int requiredQuantity)
    {
        foreach (Product product in Products)
        {
            if (product.Name == name && product.Quantity >= requiredQuantity)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveProduct(string name, int quantity)
    {
        foreach (Product product in Products)
        {
            if (product.Name == name)
            {
                product.Quantity -= quantity;
                break;
            }
        }
    }

    public decimal GetProductPrice(string name)
    {
        foreach (Product product in Products)
        {
            if (product.Name == name)
            {
                return product.Price;
            }
        }
        return 0;
    }

    public bool ProductExists(string name)
    {
        foreach (Product product in Products)
        {
            if (product.Name == name)
            {
                return true;
            }
        }
        return false;
    }
}

public class OrderSystem
{
    public Queue<Order> Orders;

    public OrderSystem()
    {
        Orders = new Queue<Order>();
    }

    public void AddOrder(Order order)
    {
        Orders.Enqueue(order);
    }

    public Order GetNextOrder()
    {
        if (Orders.Count > 0)
        {
            return Orders.Dequeue();
        }
        return null;
    }

    public void ShowAllOrders()
    {
        if (Orders.Count == 0)
        {
            Console.WriteLine("нет заказов в системе");
            return;
        }

        Console.WriteLine("\nвсе заказы в системе:");
        Console.WriteLine(new string('-', 80));
        Console.WriteLine("| {0,-10} | {1,-25} | {2,-20} | {3,-15} |",
                          "номер", "товары", "количество", "итоговая сумма");
        Console.WriteLine(new string('-', 80));

        foreach (Order order in Orders)
        {
            Console.WriteLine("| {0,-10} | {1,-25} | {2,-20} | {3,-15} |",
                              order.Number,
                              order.GetProductsList(),
                              order.GetQuantitiesList(),
                              order.GetTotalCost());
        }
        Console.WriteLine(new string('-', 80));
    }
}

public class OrderProcessor
{
    public Warehouse Warehouse;
    public OrderSystem OrderSystem;

    public OrderProcessor(Warehouse warehouse, OrderSystem orderSystem)
    {
        Warehouse = warehouse;
        OrderSystem = orderSystem;
    }

    public void ProcessOrder()
    {
        Order order = OrderSystem.GetNextOrder();

        if (order == null)
        {
            Console.WriteLine("нет заказов для обработки");
            return;
        }

        Console.WriteLine($"\nобрабатывается заказ №{order.Number} от {order.Date}");

        bool canProcess = true;
        decimal totalCost = 0;

        foreach (Product product in order.Products)
        {
            if (!Warehouse.CheckProduct(product.Name, product.Quantity))
            {
                Console.WriteLine($"товар {product.Name} отсутствует в нужном количестве");
                canProcess = false;
            }
            else
            {
                decimal productTotal = product.Price * product.Quantity;
                totalCost += productTotal;
                Console.WriteLine($"{product.Name} - {product.Quantity} шт. × {product.Price} = {productTotal}");
            }
        }

        if (canProcess)
        {
            foreach (Product product in order.Products)
            {
                Warehouse.RemoveProduct(product.Name, product.Quantity);
            }
            Console.WriteLine($"общая стоимость заказа: {totalCost}");
            Console.WriteLine($"заказ №{order.Number} успешно обработан!");
        }
        else
        {
            Console.WriteLine($"заказ №{order.Number} не может быть обработан");
        }
    }
}

class Program
{
    static void Main()
    {
        Warehouse warehouse = new Warehouse();
        OrderSystem orderSystem = new OrderSystem();
        OrderProcessor processor = new OrderProcessor(warehouse, orderSystem);

        Console.WriteLine("система управления складом и заказами");

        while (true)
        {
            Console.WriteLine("\nвыберите действие:");
            Console.WriteLine("1 - добавить товар на склад");
            Console.WriteLine("2 - показать товары на складе");
            Console.WriteLine("3 - создать заказ");
            Console.WriteLine("4 - показать все заказы");
            Console.WriteLine("5 - обработать заказы");
            Console.WriteLine("6 - выход");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddProductToWarehouse(warehouse);
                    break;

                case "2":
                    warehouse.ShowProducts();
                    break;

                case "3":
                    CreateOrder(warehouse, orderSystem);
                    break;

                case "4":
                    orderSystem.ShowAllOrders();
                    break;

                case "5":
                    ProcessAllOrders(processor);
                    break;

                case "6":
                    Console.WriteLine("выход из системы");
                    return;

                default:
                    Console.WriteLine("неверный выбор");
                    break;
            }
        }
    }

    static void AddProductToWarehouse(Warehouse warehouse)
    {
        Console.Write("введите название товара: ");
        string name = Console.ReadLine();

        Console.Write("введите цену товара: ");
        decimal price = decimal.Parse(Console.ReadLine());

        Console.Write("введите количество товара: ");
        int quantity = int.Parse(Console.ReadLine());

        warehouse.AddProduct(name, price, quantity);
        Console.WriteLine("товар добавлен на склад");
    }

    static void CreateOrder(Warehouse warehouse, OrderSystem orderSystem)
    {
        Console.Write("введите номер заказа: ");
        int orderNumber = int.Parse(Console.ReadLine());

        Order order = new Order(orderNumber, DateTime.Now);
        decimal orderTotal = 0;

        while (true)
        {
            Console.Write("введите название товара для заказа (или 'готово' для завершения): ");
            string productName = Console.ReadLine();

            if (productName.ToLower() == "готово")
                break;

            if (!warehouse.ProductExists(productName))
            {
                Console.WriteLine("такого товара нет на складе");
                continue;
            }

            Console.Write("введите количество: ");
            int quantity = int.Parse(Console.ReadLine());

            decimal price = warehouse.GetProductPrice(productName);
            decimal productTotal = price * quantity;
            orderTotal += productTotal;

            order.Products.Add(new Product(productName, price, quantity));
            Console.WriteLine($"товар добавлен в заказ (цена за шт: {price}, общая стоимость позиции: {productTotal})");
            Console.WriteLine($"текущая сумма заказа: {orderTotal}");
        }

        orderSystem.AddOrder(order);
        Console.WriteLine($"заказ создан и добавлен в очередь. итоговая сумма: {orderTotal}");
    }

    static void ProcessAllOrders(OrderProcessor processor)
    {
        Console.WriteLine("обработка заказов:");

        while (true)
        {
            processor.ProcessOrder();

            if (processor.OrderSystem.Orders.Count == 0)
            {
                Console.WriteLine("все заказы обработаны");
                break;
            }

            Console.Write("обработать следующий заказ? (да/нет): ");
            string answer = Console.ReadLine();

            if (answer.ToLower() != "да")
                break;
        }
    }
}