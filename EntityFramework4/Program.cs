using Microsoft.EntityFrameworkCore;

namespace EntityFramework4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new BankDbContext();
            bool exit = false;
            
            // Helping by AI 

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("National Bank Management");
                Console.WriteLine("===========================");
                Console.WriteLine("1) Add a new Customer");
                Console.WriteLine("2) Open a new Account for a Customer");
                Console.WriteLine("3) Update Account Status (Active / Closed)");
                Console.WriteLine("4) Remove an Account from a Customer");
                Console.WriteLine("5) List all Customers (with accounts)");
                Console.WriteLine("0) Exit");
                Console.Write("Enter choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddNewCustomer(context);
                        break;
                    case "2":
                        OpenNewAccount(context);
                        break;
                    case "3":
                        UpdateAccountStatus(context);
                        break;
                    case "4":
                        RemoveAccountFromCustomer(context);
                        break;
                    case "5":
                        ListAllCustomers(context);
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("Exiting application...");
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Please enter a number between 0 and 5.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                }
            }
        }
        static void AddNewCustomer(BankDbContext context)
        {
            Console.WriteLine("\n--- Add New Customer ---");
            Console.Write("Full Name: ");
            string name = Console.ReadLine();

            Console.Write("National ID: ");
            string nationalId = Console.ReadLine();

            Console.Write("Date of Birth (yyyy-MM-dd): ");
            DateTime dob = DateTime.Parse(Console.ReadLine());

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();

            Console.Write("Address: ");
            string address = Console.ReadLine();

            Console.WriteLine("Customer Type:\n1) Individual\n2) Business");
            Console.Write("Choice: ");
            string typeChoice = Console.ReadLine();
            string customerType = (typeChoice == "2") ? "Business" : "Individual";

            var customer = new Customer
            {
                FullName = name,
                NationalId = nationalId,
                DateOfBirth = dob,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                CustomerType = customerType
            };

            context.Customers.Add(customer);
            context.SaveChanges();

            Console.WriteLine($"Customer created successfully. CustomerId = {customer.Id}");
        }
        // Helping by AI
        static void OpenNewAccount(BankDbContext context)
        {
            Console.WriteLine("\n--- Open New Account ---");
            Console.Write("Account Number: ");
            string accNumber = Console.ReadLine();

            Console.WriteLine("Account Type:\n1) Savings\n2) Current\n3) Business");
            Console.Write("Choice: ");
            string typeChoice = Console.ReadLine();
            string accType = typeChoice switch { "1" => "Savings", "2" => "Current", "3" => "Business", _ => "Savings" };

            Console.Write("Branch Code: ");
            string branchCode = Console.ReadLine();

            Console.Write("Customer Id: ");
            int customerId = int.Parse(Console.ReadLine());

            Console.WriteLine("Ownership Role:\n1) Primary\n2) CoHolder");
            Console.Write("Choice: ");
            string roleChoice = Console.ReadLine();
            string role = roleChoice == "2" ? "CoHolder" : "Primary";

            Console.WriteLine($"Validating branch '{branchCode}' and customer #{customerId}...");

            var branch = context.Branches.Find(branchCode);
            var customer = context.Customers.Find(customerId);

            if (branch == null || customer == null)
            {
                Console.WriteLine("Error: Branch or Customer not found!");
                return;
            }

            var account = new Account
            {
                AccountNumber = accNumber,
                AccountType = accType,
                CurrentBalance = 0,
                OpeningDate = DateTime.Now,
                BranchCode = branchCode
            };

            context.Accounts.Add(account);

            var customerAccount = new CustomerAccount
            {
                CustomerId = customerId,
                AccountNumber = accNumber,
                OwnershipType = role,
                OwnershipStartDate = DateTime.Now,
                AccountStatus = "Active"
            };

            context.CustomerAccounts.Add(customerAccount);
            context.SaveChanges();

            Console.WriteLine($"Account {accNumber} created and linked to customer {customerId} as {role} owner.");
        }

        static void UpdateAccountStatus(BankDbContext context)
        {
            Console.WriteLine("\n--- Update Account Status ---");
            Console.Write("Account Number: ");
            string accNumber = Console.ReadLine();

            Console.Write("Customer Id: ");
            int customerId = int.Parse(Console.ReadLine());

            var link = context.CustomerAccounts
                .FirstOrDefault(ca => ca.AccountNumber == accNumber && ca.CustomerId == customerId);

            if (link == null)
            {
                Console.WriteLine("Account link not found for this customer.");
                return;
            }

            Console.WriteLine("New Status:\n1) Active\n2) Closed");
            Console.Write("Choice: ");
            string choice = Console.ReadLine();
            link.AccountStatus = choice == "2" ? "Closed" : "Active";

            context.SaveChanges();
            Console.WriteLine($"Status updated to {link.AccountStatus}.");
        }

        static void RemoveAccountFromCustomer(BankDbContext context)
        {
            Console.WriteLine("\n--- Remove Account From Customer ---");
            Console.Write("Account Number: ");
            string accNumber = Console.ReadLine();

            Console.Write("Customer Id: ");
            int customerId = int.Parse(Console.ReadLine());

            var link = context.CustomerAccounts
                .FirstOrDefault(ca => ca.AccountNumber == accNumber && ca.CustomerId == customerId);

            if (link == null)
            {
                Console.WriteLine("Link not found.");
                return;
            }

            context.CustomerAccounts.Remove(link);

            bool hasOtherOwners = context.CustomerAccounts.Any(ca => ca.AccountNumber == accNumber && ca.CustomerId != customerId);

            if (!hasOtherOwners)
            {
                var account = context.Accounts.Find(accNumber);
                if (account != null)
                {
                    context.Accounts.Remove(account);
                    Console.WriteLine("Ownership link deleted. That was the last owner — account was also removed.");
                }
            }
            else
            {
                Console.WriteLine("Ownership link deleted.");
            }

            context.SaveChanges();
        }

        static void ListAllCustomers(BankDbContext context)
        {
            Console.WriteLine("\n--- All Customers ---");

            var customers = context.Customers
                .Include(c => c.CustomerAccounts)
                    .ThenInclude(ca => ca.Account)
                        .ThenInclude(a => a.Branch)
                .ToList();

            int index = 1;
            foreach (var c in customers)
            {
                Console.WriteLine($"#{index++} {c.FullName} ({c.CustomerType})");

                if (!c.CustomerAccounts.Any())
                {
                    Console.WriteLine("    (no accounts)");
                }
                else
                {
                    foreach (var ca in c.CustomerAccounts)
                    {
                        var acc = ca.Account;
                        Console.WriteLine($"    {acc.AccountNumber} Current Balance: {acc.CurrentBalance:N2} {ca.OwnershipType} {ca.AccountStatus} @ {acc.Branch?.Name}");
                    }
                }
            }
        }
    }
}
