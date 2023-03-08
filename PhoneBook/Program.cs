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
                SearchContact(args[1]);
                break;
        }
    }

    private static void SearchContact(string name)
    {
        if (name.IsNullOrEmpty())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Specify a name");
            Console.ForegroundColor = ConsoleColor.White;
        }

        List<ContactDTO> result = DbAccess.SearchByName(name);

        if (result.Count == 0)
        {
            Console.WriteLine("No contacts with that name found");
            return;
        }

        foreach (var item in result)
        {
            Console.WriteLine($"{item.Name} - {item.Number}");
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
        List<ContactDTO> result = DbAccess.LoadAll();

        foreach (ContactDTO item in result)
        {
            Console.WriteLine($"{item.Name} - {item.Number}");
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

    public static List<ContactDTO> LoadAll()
    {
        try
        {
            using PhoneBookContext context = new();

            var list = context.Contacts
                              .Select(p => new ContactDTO()
                              {
                                  Name = p.Name,
                                  Number = p.PhoneNumber
                              }).ToList();

            return list;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static List<ContactDTO> SearchByName(string name)
    {
        using PhoneBookContext context = new();

        var list = context.Contacts
            .Where(n => n.Name == name)
            .Select(p => new ContactDTO
            {
                Name = p.Name,
                Number = p.PhoneNumber
            })
            .ToList();

        return list;
    }
}

public class ContactDTO
{
    public string? Name { get; set; }
    public string? Number { get; set; }
}
