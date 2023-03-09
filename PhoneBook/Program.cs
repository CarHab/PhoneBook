using Microsoft.IdentityModel.Tokens;
using PhoneBook.DataAccess;
using PhoneBook.Models;

internal class Program
{
    private static string[] operations = { "add", "list", "search", "update", "delete" };
    private static void Main(string[] args)
    {
        if (args.Length == 0 || !operations.Contains(args[0]))
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid operation");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Use 'add' option create a new contact");
            Console.WriteLine("Use 'list' option create a new contact");
            Console.WriteLine("Use 'search NAME' option create a new contact");
            Console.WriteLine("Use 'update NAME' option create a new contact");
            Console.WriteLine("Use 'delete NUMBER' option create a new contact");
        }

        switch (args[0])
        {
            case "add":
                CreateContact();
                break;
            case "list":
                ListAll();
                break;
            case "search":
                SearchContact(args.Length > 1 ? args[1] : "");
                break;
            case "update":
                UpdateContact(args.Length > 1 ? args[1] : "");
                break;
            case "delete":
                DeleteContact(args.Length > 1 ? args[1] : "");
                break;
        }
    }

    private static void DeleteContact(string name)
    {
        if (name.IsNullOrEmpty() || !name.All(Char.IsLetter))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid name");
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        List<Contact> result = DbAccess.SearchByName(name);

        if (result.Count == 0)
        {
            Console.WriteLine("No contacts found with that name.");
        }
        else if (result.Count > 1)
        {
            Console.WriteLine("There's more than one contact with that name.");
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {result[i].Name} - {result[i].PhoneNumber}");
            }

            bool valid = false;
            int choice;
            while (!valid)
            {
                Console.Write($"Specify which one({1}-{result.Count}): ");
                string? choiceString = Console.ReadLine();
                if (!int.TryParse(choiceString, out _))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }

                choice = Convert.ToInt32(choiceString);



                DbAccess.Delete(result[choice - 1]);

                valid = true;
            }
        }
        else
        {
            DbAccess.Delete(result[0]);
        }
    }

    private static void UpdateContact(string name)
    {
        if (name.IsNullOrEmpty() || !name.All(Char.IsLetter))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid name");
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        List<Contact> result = DbAccess.SearchByName(name);

        ContactUpdateModel contactUpdate = new();

        if (result.Count == 0)
        {
            Console.WriteLine("No contacts found with that name.");
        }
        else if (result.Count > 1)
        {
            Console.WriteLine("There's more than one contact with that name.");
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {result[i].Name} - {result[i].PhoneNumber}");
            }

            bool valid = false;
            int choice;
            while (!valid)
            {
                Console.Write($"Specify which one({1}-{result.Count}): ");
                string? choiceString = Console.ReadLine();
                if (!int.TryParse(choiceString, out _))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }

                choice = Convert.ToInt32(choiceString);
                contactUpdate.OldName = result[choice - 1].Name;
                contactUpdate.OldNumber = result[choice - 1].PhoneNumber;

                Console.Write("New name: ");
                contactUpdate.NewName = Console.ReadLine();

                Console.Write("New number: ");
                contactUpdate.NewNumber = Console.ReadLine();

                DbAccess.Update(contactUpdate);

                valid = true;
            }
        }
        else
        {
            contactUpdate.OldName = result[0].Name;
            contactUpdate.OldNumber = result[0].PhoneNumber;

            Console.Write("New name: ");
            contactUpdate.NewName = Console.ReadLine();

            Console.Write("New number: ");
            contactUpdate.NewNumber = Console.ReadLine();

            DbAccess.Update(contactUpdate);
        }
    }

    private static void SearchContact(string name)
    {
        if (name.IsNullOrEmpty())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Specify a name");
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        List<Contact> result = DbAccess.SearchByName(name);

        if (result.Count == 0)
        {
            Console.WriteLine("No contacts with that name found");
            return;
        }

        foreach (var item in result)
        {
            Console.WriteLine($"{item.Name} - {item.PhoneNumber}");
        }
    }

    private static void CreateContact()
    {
        Contact newContact = new();

        Console.Write("Name: ");
        newContact.Name = Console.ReadLine();

        Console.Write("Number: ");
        newContact.PhoneNumber = Console.ReadLine();

        DbAccess.InsertContact(newContact);
    }

    private static void ListAll()
    {
        List<Contact> result = DbAccess.LoadAll();

        foreach (Contact item in result)
        {
            Console.WriteLine($"{item.Name} - {item.PhoneNumber}");
        }
    }
}

public class DbAccess
{
    public static void InsertContact(Contact newContact)
    {
        try
        {
            using PhoneBookContext context = new();

            context.Contacts.Add(newContact);
            context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static List<Contact> LoadAll()
    {
        try
        {
            using PhoneBookContext context = new();

            var list = context.Contacts.ToList();

            return list;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static List<Contact> SearchByName(string name)
    {
        using PhoneBookContext context = new();

        var list = context.Contacts
            .Where(n => n.Name == name)
            .ToList();

        return list;
    }

    public static void Update(ContactUpdateModel contactToUpdate)
    {
        using PhoneBookContext context = new();

        Contact contact = context.Contacts.Single(c => c.Name == contactToUpdate.OldName && c.PhoneNumber == contactToUpdate.OldNumber);
        contact.Name = contactToUpdate.NewName;
        contact.PhoneNumber = contactToUpdate.NewNumber;
        context.SaveChanges();
    }

    public static void Delete(Contact contact)
    {
        using PhoneBookContext context = new();

        context.Contacts.Remove(contact);
        context.SaveChanges();
    }
}

public class ContactDTO
{
    public string? Name { get; set; }
    public string? Number { get; set; }
}

public class ContactUpdateModel
{
    public string? OldName { get; set; }
    public string? OldNumber { get; set; }
    public string? NewName { get; set; }
    public string? NewNumber { get; set; }
}