Console.WriteLine("Задание 1. Счетчик с порогом");
Counter counter = new Counter(5);
counter.ThresholdReached += DisplayMessage;

for (int i = 0; i < 7; i++)
{
    counter.Increment();
}

void DisplayMessage(Counter sender, CounterEventArgs e)
{
    Console.WriteLine($"Достигнут порог {e.Threshold}! Текущее значение: {e.Value}");
}


Console.WriteLine();
Console.WriteLine("Задание 2. Датчик температуры");
const double MaxTemperature = 30;

TemperatureSensor sensor = new TemperatureSensor("Датчик №1", 20);
sensor.TemperatureChanged += ShowTemperature;
sensor.TemperatureChanged += CheckOverheat;

sensor.Measure(22.5);
sensor.Measure(22.5);
sensor.Measure(31);
sensor.Measure(25);

void ShowTemperature(TemperatureSensor sender, TemperatureEventArgs e)
{
    Console.WriteLine($"{sender.Name}: температура изменилась с {e.OldTemperature} до {e.NewTemperature} °C");
}

void CheckOverheat(TemperatureSensor sender, TemperatureEventArgs e)
{
    if (e.NewTemperature > MaxTemperature)
    {
        Console.WriteLine($"ВНИМАНИЕ! Превышена допустимая температура {MaxTemperature} °C!");
    }
}

Console.WriteLine();
Console.WriteLine("Задание 3. Оформление заказа");
Order order = new Order(1, "Иванов Иван", 1500);
order.StatusChanged += ShowMessage;
order.StatusChanged += (sender, e) =>
    Console.WriteLine($"Уведомление для {sender.Customer}: статус заказа - {e.Status}");

order.Pay(1500);
order.Place();
order.Pay(1000);
order.Pay(1500);

void ShowMessage(Order sender, OrderEventArgs e)
{
    Console.WriteLine($"[Заказ №{sender.Number}] {e.Message}");
}


class CounterEventArgs
{
    public int Value { get; }
    public int Threshold { get; }

    public CounterEventArgs(int value, int threshold)
    {
        Value = value;
        Threshold = threshold;
    }
}

class Counter
{
    public delegate void CounterHandler(Counter sender, CounterEventArgs e);
    public event CounterHandler? ThresholdReached;

    private int _value;
    private readonly int _threshold;

    public int Value => _value;

    public Counter(int threshold)
    {
        _threshold = threshold;
    }

    public void Increment()
    {
        _value++;
        Console.WriteLine($"Значение счетчика: {_value}");
        if (_value == _threshold)
        {
            ThresholdReached?.Invoke(this, new CounterEventArgs(_value, _threshold));
        }
    }
}


class TemperatureEventArgs
{
    public double OldTemperature { get; }
    public double NewTemperature { get; }

    public TemperatureEventArgs(double oldTemperature, double newTemperature)
    {
        OldTemperature = oldTemperature;
        NewTemperature = newTemperature;
    }
}

class TemperatureSensor
{
    public delegate void TemperatureHandler(TemperatureSensor sender, TemperatureEventArgs e);
    public event TemperatureHandler? TemperatureChanged;

    private double _temperature;

    public string Name { get; }

    public double Temperature
    {
        get => _temperature;
        private set
        {
            if (_temperature != value)
            {
                double old = _temperature;
                _temperature = value;
                TemperatureChanged?.Invoke(this, new TemperatureEventArgs(old, value));
            }
        }
    }

    public TemperatureSensor(string name, double startTemperature)
    {
        Name = name;
        _temperature = startTemperature;
    }

    public void Measure(double value)
    {
        Temperature = value;
    }
}


class OrderEventArgs
{
    public string Message { get; }
    public string Status { get; }

    public OrderEventArgs(string message, string status)
    {
        Message = message;
        Status = status;
    }
}

class Order
{
    public delegate void OrderHandler(Order sender, OrderEventArgs e);
    public event OrderHandler? StatusChanged;

    public int Number { get; }
    public string Customer { get; }
    public decimal Amount { get; }
    public string Status { get; private set; }

    public Order(int number, string customer, decimal amount)
    {
        Number = number;
        Customer = customer;
        Amount = amount;
        Status = "Новый";
    }

    public void Place()
    {
        Status = "Оформлен";
        StatusChanged?.Invoke(this, new OrderEventArgs($"Заказ №{Number} оформлен", Status));
    }

    public void Pay(decimal sum)
    {
        if (Status != "Оформлен")
        {
            StatusChanged?.Invoke(this, new OrderEventArgs("Сначала нужно оформить заказ", Status));
        }
        else if (sum < Amount)
        {
            StatusChanged?.Invoke(this, new OrderEventArgs($"Недостаточно средств. Сумма заказа: {Amount}", Status));
        }
        else
        {
            Status = "Оплачен";
            StatusChanged?.Invoke(this, new OrderEventArgs($"Заказ №{Number} оплачен", Status));
        }
    }
}