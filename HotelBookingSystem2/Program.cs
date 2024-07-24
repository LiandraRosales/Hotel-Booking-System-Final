using System;
using System.Collections.Generic;
using System.Linq;

// Customer class
public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
}

// Room class (base class)
public abstract class Room
{
    public int RoomNumber { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}

// SingleRoom class
public class SingleRoom : Room
{
    public string BedSize { get; set; }
    public bool HasBalcony { get; set; }
}

// DoubleRoom class
public class DoubleRoom : Room
{
    public string BedSize { get; set; }
    public bool HasMiniBar { get; set; }
    public int NumberOfBeds { get; set; }
}

// Suite class
public class Suite : Room
{
    public double LivingAreaSize { get; set; }
    public bool HasJacuzzi { get; set; }
    public int NumberOfRooms { get; set; }
}

// Booking class
public class Booking
{
    private static int nextBookingID = 1;

    public int BookingID { get; private set; }
    public Customer Customer { get; set; }
    public Room Room { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string Status { get; set; }
    public decimal TotalPrice { get; set; }

    public Booking()
    {
        BookingID = nextBookingID++;
    }
}

// Hotel class
public class Hotel
{
    public List<Room> Rooms { get; private set; }
    public List<Booking> Bookings { get; private set; }
    public List<Customer> Customers { get; private set; }

    public Hotel()
    {
        Rooms = new List<Room>();
        Bookings = new List<Booking>();
        Customers = new List<Customer>();
    }

    // Method to add a room to the hotel
    public void AddRoom(Room room)
    {
        Rooms.Add(room);
        Console.WriteLine($"Room {room.RoomNumber} added to the hotel successfully.");
    }

    // Method to remove a room from the hotel
    public void RemoveRoom(int roomNumber)
    {
        Room roomToRemove = Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);

        if (roomToRemove == null)
        {
            Console.WriteLine($"Room {roomNumber} not found in the hotel.");
            return;
        }

        // Check if the room is currently booked
        bool isBooked = Bookings.Any(b => b.Room.RoomNumber == roomNumber && b.Status == "Active");

        if (isBooked)
        {
            Console.WriteLine($"Cannot remove Room {roomNumber} because it is currently booked.");
            return;
        }

        Rooms.Remove(roomToRemove);
        Console.WriteLine($"Room {roomNumber} removed from the hotel successfully.");
    }

    // Method to add a customer to the hotel
    public void AddCustomer(Customer customer)
    {
        Customers.Add(customer);
        Console.WriteLine($"Customer {customer.Name} added to the hotel successfully.");
    }

    // Method to remove a customer from the hotel
    public void RemoveCustomer(int customerId)
    {
        Customer customerToRemove = Customers.FirstOrDefault(c => c.CustomerId == customerId);

        if (customerToRemove == null)
        {
            Console.WriteLine($"Customer with ID {customerId} not found in the hotel.");
            return;
        }

        // Check if the customer has an active booking
        bool hasActiveBooking = Bookings.Any(b => b.Customer.CustomerId == customerId && b.Status == "Active");

        if (hasActiveBooking)
        {
            Console.WriteLine($"Cannot remove Customer {customerToRemove.Name} because they have an active booking.");
            return;
        }

        Customers.Remove(customerToRemove);
        Console.WriteLine($"Customer {customerToRemove.Name} removed from the hotel.");
    }

    // Method to book a room for a customer
    public void BookRoom(Customer customer, Room room, DateTime checkInDate, DateTime checkOutDate)
    {
        if (!room.IsAvailable)
        {
            Console.WriteLine($"Room {room.RoomNumber} is not available for booking.");
            return;
        }
        //Check for overlapping booking
        bool isOverlappingBooking = Bookings.Any(b =>
        b.Room.RoomNumber == room.RoomNumber &&
        b.Status == "Active" &&
        ((checkInDate >= b.CheckInDate && checkInDate < b.CheckOutDate) ||
          (checkOutDate > b.CheckInDate && checkOutDate <= b.CheckOutDate) ||
          (checkInDate <= b.CheckInDate && checkOutDate >= b.CheckOutDate)));

        if (isOverlappingBooking)
        {
            Console.WriteLine($"Room {room.RoomNumber} is already booked for overlapping dates.");
            return;
        }

        Booking newBooking = new Booking
        {
            Customer = customer,
            Room = room,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            Status = "Active"
        };

        room.IsAvailable = false; // Mark the room as booked
        Bookings.Add(newBooking);
        Console.WriteLine($"Room {room.RoomNumber} booked successfully for {customer.Name}.");
    }

    // Method to checkout a room for a customer
    public void CheckoutRoom(Customer customer)
    {
        Booking activeBooking = Bookings.FirstOrDefault(b => b.Customer.CustomerId == customer.CustomerId && b.Status == "Active");

        if (activeBooking == null)
        {
            Console.WriteLine($"No active booking found for Customer {customer.Name}.");
            return;
        }

        // Calculate total price based on number of nights stayed
        int nightsStayed = (int)(DateTime.Now - activeBooking.CheckInDate).TotalDays;
        activeBooking.TotalPrice = nightsStayed * activeBooking.Room.Price;

        // Check if the customer stayed more nights than anticipated
        if (DateTime.Now > activeBooking.CheckOutDate)
        {
            decimal extraCharge = 0.5m * activeBooking.Room.Price * (DateTime.Now - activeBooking.CheckOutDate).Days;
            activeBooking.TotalPrice += extraCharge;
        }

        activeBooking.Status = "Closed";
        activeBooking.Room.IsAvailable = true; // Mark the room as available
        Console.WriteLine($"Room {activeBooking.Room.RoomNumber} checked out successfully for {customer.Name}. Total price: {activeBooking.TotalPrice:C}");
    }

    // Method to list all bookings (both active and closed)
    public void ListAllBookings()
    {
        Console.WriteLine("===== ALL BOOKINGS =====");
        foreach (var booking in Bookings)
        {
            Console.WriteLine($"Booking ID: {booking.BookingID}, Customer: {booking.Customer.Name}, Room Number: {booking.Room.RoomNumber}, Check-in: {booking.CheckInDate}, Check-out: {booking.CheckOutDate}, Status: {booking.Status}");
        }
        Console.WriteLine("========================");
    }

    // Method to list all available rooms
    public void ListAvailableRooms()
    {
        Console.WriteLine("===== AVAILABLE ROOMS =====");
        foreach (var room in Rooms.Where(r => r.IsAvailable))
        {
            Console.WriteLine($"Room Number: {room.RoomNumber}, Price: {room.Price:C}, Type: {room.GetType().Name}");
        }
        Console.WriteLine("==========================");
    }

    // Method to list available rooms by a specific bed size
    public void ListAvailableRoomsByBedSize(string bedSize)
    {
        Console.WriteLine($"===== AVAILABLE {bedSize.ToUpper()} ROOMS =====");
        foreach (var room in Rooms.Where(r => r.IsAvailable && r.GetType() != typeof(Suite) && (r is SingleRoom && ((SingleRoom)r).BedSize == bedSize || r is DoubleRoom && ((DoubleRoom)r).BedSize == bedSize)))
        {
            Console.WriteLine($"Room Number: {room.RoomNumber}, Price: {room.Price:C}, Type: {room.GetType().Name}");
        }
        Console.WriteLine("=====================================");
    }

    // Method to list all closed bookings
    public void ListClosedBookings()
    {
        Console.WriteLine("===== CLOSED BOOKINGS =====");
        foreach (var booking in Bookings.Where(b => b.Status == "Closed"))
        {
            Console.WriteLine($"Booking ID: {booking.BookingID}, Customer: {booking.Customer.Name}, Room Number: {booking.Room.RoomNumber}, Check-in: {booking.CheckInDate}, Check-out: {booking.CheckOutDate}, Total Price: {booking.TotalPrice:C}");
        }
        Console.WriteLine("===========================");
    }
}

// Menu class
public class Menu
{
    private Hotel hotel;

    public Menu(Hotel hotel)
    {
        this.hotel = hotel;
    }

    public void DisplayMenu()
    {
        Console.WriteLine("===== HOTEL BOOKING SYSTEM MENU =====");
        Console.WriteLine("1. Rooms");
        Console.WriteLine("2. Customers");
        Console.WriteLine("3. Booking");
        Console.WriteLine("4. Exit");
        Console.WriteLine("====================================");

        Console.Write("Enter your choice: ");
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice))
        {
            Console.WriteLine("Invalid input. Please enter a number from System Menu.");
            Console.WriteLine("Enter your choice.");

        }

        switch (choice)
        {
            case 1:
                RoomMenu();
                break;
            case 2:
                CustomerMenu();
                break;
            case 3:
                BookingMenu();
                break;
            case 4:
                Console.WriteLine("Exiting the program...");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    private void RoomMenu()
    {
        Console.WriteLine("===== ROOMS MENU =====");
        Console.WriteLine("1. Add Room");
        Console.WriteLine("2. Remove Room");
        Console.WriteLine("3. Display all rooms");
        Console.WriteLine("======================");

        Console.Write("Enter your choice: ");
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice))
        {
            Console.WriteLine("Invalid input. Please enter a number from Rooms Menu.");
            Console.Write("Enter your choice: ");
        }

        switch (choice)
        {
            case 1:
                AddRoom();
                break;
            case 2:
                RemoveRoom();
                break;
            case 3:
                DisplayAllRooms();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    private void AddRoom()
    {
        Console.WriteLine("Enter Room Type (Single, Double, Suite): ");
        string roomType = Console.ReadLine();

        Console.Write("Enter Room Number (with 3 digits): ");
        int roomNumber;
        while (!int.TryParse(Console.ReadLine(), out roomNumber))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            Console.Write("Enter Room Number: ");
        }

        Console.Write("Enter Price per Night: ");
        decimal price;
        while (!decimal.TryParse(Console.ReadLine(), out price))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            Console.Write("Enter Price per Night: ");
        }

        Room newRoom = null;

        switch (roomType.ToLower())
        {
            case "single":
                Console.Write("Enter Bed Size (e.g., Twin, Queen): ");
                string bedSizeSingle = Console.ReadLine();
                Console.Write("Has Balcony? (true/false): ");
                bool hasBalcony = bool.Parse(Console.ReadLine());
                newRoom = new SingleRoom
                {
                    RoomNumber = roomNumber,
                    Price = price,
                    IsAvailable = true,
                    BedSize = bedSizeSingle,
                    HasBalcony = hasBalcony
                };
                break;
            case "double":
                Console.Write("Enter Bed Size (e.g., Queen, King): ");
                string bedSizeDouble = Console.ReadLine();
                Console.Write("Has Mini Bar? (true/false): ");
                bool hasMiniBar = bool.Parse(Console.ReadLine());
                Console.Write("Enter Number of Beds: ");
                int numberOfBeds = int.Parse(Console.ReadLine());
                newRoom = new DoubleRoom
                {
                    RoomNumber = roomNumber,
                    Price = price,
                    IsAvailable = true,
                    BedSize = bedSizeDouble,
                    HasMiniBar = hasMiniBar,
                    NumberOfBeds = numberOfBeds
                };
                break;
            case "suite":
                Console.Write("Enter Living Area Size (in square meters): ");
                double livingAreaSize = double.Parse(Console.ReadLine());
                Console.Write("Has Jacuzzi? (true/false): ");
                bool hasJacuzzi = bool.Parse(Console.ReadLine());
                Console.Write("Enter Number of Rooms in Suite: ");
                int numberOfRooms = int.Parse(Console.ReadLine());
                newRoom = new Suite
                {
                    RoomNumber = roomNumber,
                    Price = price,
                    IsAvailable = true,
                    LivingAreaSize = livingAreaSize,
                    HasJacuzzi = hasJacuzzi,
                    NumberOfRooms = numberOfRooms
                };
                break;
            default:
                Console.WriteLine("Invalid room type.");
                return;
        }

        hotel.AddRoom(newRoom);
    }

    private void RemoveRoom()
    {
        Console.Write("Enter Room Number to Remove: ");
        int roomNumber = int.Parse(Console.ReadLine());
        hotel.RemoveRoom(roomNumber);
    }

    private void DisplayAllRooms()
    {
        Console.WriteLine("===== ALL ROOMS =====");
        foreach (var room in hotel.Rooms)
        {
            Console.WriteLine($"Room Number: {room.RoomNumber}, Price: {room.Price:C}, Available: {(room.IsAvailable ? "Yes" : "No")}, Type: {room.GetType().Name}");
        }
        Console.WriteLine("======================");
    }

    private void CustomerMenu()
    {
        Console.WriteLine("===== CUSTOMERS MENU =====");
        Console.WriteLine("1. Add Customer");
        Console.WriteLine("2. Remove Customer");
        Console.WriteLine("3. List all customers");
        Console.WriteLine("==========================");

        Console.Write("Enter your choice: ");
        int choice = int.Parse(Console.ReadLine());

        switch (choice)
        {
            case 1:
                AddCustomer();
                break;
            case 2:
                RemoveCustomer();
                break;
            case 3:
                ListAllCustomers();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    private void AddCustomer()
    {
        Console.Write("Enter Customer Name: ");
        string name = Console.ReadLine();
        Customer newCustomer = new Customer { CustomerId = hotel.Customers.Count + 1, Name = name };
        hotel.AddCustomer(newCustomer);
    }

    private void RemoveCustomer()
    {
        Console.Write("Enter Customer ID to Remove: ");
        int customerId = int.Parse(Console.ReadLine());
        hotel.RemoveCustomer(customerId);
    }

    private void ListAllCustomers()
    {
        Console.WriteLine("===== ALL CUSTOMERS =====");
        foreach (var customer in hotel.Customers)
        {
            Console.WriteLine($"Customer ID: {customer.CustomerId}, Name: {customer.Name}");
        }
        Console.WriteLine("==========================");
    }

    private void BookingMenu()
    {
        Console.WriteLine("===== BOOKING MENU =====");
        Console.WriteLine("1. Book Room");
        Console.WriteLine("2. Checkout Room");
        Console.WriteLine("3. List All Bookings");
        Console.WriteLine("4. List Available Rooms");
        Console.WriteLine("5. List Available Rooms by Bed Size");
        Console.WriteLine("6. List Closed Bookings");
        Console.WriteLine("========================");

        Console.Write("Enter your choice: ");
        int choice = int.Parse(Console.ReadLine());

        switch (choice)
        {
            case 1:
                BookRoom();
                break;
            case 2:
                CheckoutRoom();
                break;
            case 3:
                ListAllBookings();
                break;
            case 4:
                ListAvailableRooms();
                break;
            case 5:
                ListAvailableRoomsByBedSize();
                break;
            case 6:
                ListClosedBookings();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    private void BookRoom()
    {
        Console.Write("Enter Customer ID: ");
        int customerId = int.Parse(Console.ReadLine());
        Customer customer = hotel.Customers.FirstOrDefault(c => c.CustomerId == customerId);

        if (customer == null)
        {
            Console.WriteLine($"Customer with ID {customerId} not found.");
            return;
        }

        Console.Write("Enter Room Number to Book: ");
        int roomNumber = int.Parse(Console.ReadLine());
        Room room = hotel.Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);

        if (room == null)
        {
            Console.WriteLine($"Room with Number {roomNumber} not found.");
            return;
        }

        Console.Write("Enter Check-in Date (yyyy-MM-dd): ");
        DateTime checkInDate = DateTime.Parse(Console.ReadLine());

        Console.Write("Enter Check-out Date (yyyy-MM-dd): ");
        DateTime checkOutDate = DateTime.Parse(Console.ReadLine());

        hotel.BookRoom(customer, room, checkInDate, checkOutDate);
    }

    private void CheckoutRoom()
    {
        Console.Write("Enter Customer ID to Checkout: ");
        int customerId = int.Parse(Console.ReadLine());
        Customer customer = hotel.Customers.FirstOrDefault(c => c.CustomerId == customerId);

        if (customer == null)
        {
            Console.WriteLine($"Customer with ID {customerId} not found.");
            return;
        }

        hotel.CheckoutRoom(customer);
    }

    private void ListAllBookings()
    {
        hotel.ListAllBookings();
    }

    private void ListAvailableRooms()
    {
        hotel.ListAvailableRooms();
    }

    private void ListAvailableRoomsByBedSize()
    {
        Console.Write("Enter Bed Size (Twin, FullDouble, Queen, King): ");
        string bedSize = Console.ReadLine().Trim();
        hotel.ListAvailableRoomsByBedSize(bedSize);
    }

    private void ListClosedBookings()
    {
        hotel.ListClosedBookings();
    }
}

// Main Program
class Program
{
    static void Main(string[] args)
    {
        // Create a new hotel object
        Hotel hotel = new Hotel();

        // Adding sample rooms
        hotel.AddRoom(new SingleRoom { RoomNumber = 101, Price = 100, IsAvailable = true, BedSize = "Twin", HasBalcony = false });
        hotel.AddRoom(new SingleRoom { RoomNumber = 102, Price = 115, IsAvailable = true, BedSize = "FullDouble", HasBalcony = true });
        hotel.AddRoom(new DoubleRoom { RoomNumber = 201, Price = 200, IsAvailable = true, BedSize = "King", HasMiniBar = true, NumberOfBeds = 2 });
        hotel.AddRoom(new Suite { RoomNumber = 301, Price = 400, IsAvailable = true, LivingAreaSize = 40.5, HasJacuzzi = true, NumberOfRooms = 3 });
        hotel.AddRoom(new Suite { RoomNumber = 302, Price = 400, IsAvailable = true, LivingAreaSize = 40.5, HasJacuzzi = true, NumberOfRooms = 3 });
        hotel.AddRoom(new DoubleRoom { RoomNumber = 202, Price = 200, IsAvailable = true, BedSize = "Queen", HasMiniBar = true, NumberOfBeds = 2 });
        hotel.AddRoom(new SingleRoom { RoomNumber = 103, Price = 115, IsAvailable = true, BedSize = "Twin", HasBalcony = true });

        // Adding sample customers
        hotel.AddCustomer(new Customer { CustomerId = 1, Name = "Rhaenyra" });
        hotel.AddCustomer(new Customer { CustomerId = 2, Name = "Alicent" });
        hotel.AddCustomer(new Customer { CustomerId = 3, Name = "Daemond" });
        hotel.AddCustomer(new Customer { CustomerId = 4, Name = "Rhaenys" });

        // Adding sample bookings
        DateTime today = DateTime.Today;
        DateTime tomorrow = today.AddDays(1);
        DateTime yesterday = today.AddDays(-1);

        hotel.BookRoom(hotel.Customers[0], hotel.Rooms[0], today, tomorrow); // Book room 101 for Alicent
        hotel.BookRoom(hotel.Customers[1], hotel.Rooms[1], yesterday, today); // Book room 201 for Rhaenys

        // Create a menu and start the application
        Menu menu = new Menu(hotel);
        while (true)
        {
            menu.DisplayMenu(); // Display the main menu
        }
    }
}

//class Program
//{
//  static void Main(string[] args)
//{
//  Hotel hotel = new Hotel();  // Create a new hotel object
//Menu menu = new Menu(hotel);  // Pass the hotel object to the menu
//
//while (true)
//{
//      menu.DisplayMenu();  // Display the main menu
//    }
//  }
//}
