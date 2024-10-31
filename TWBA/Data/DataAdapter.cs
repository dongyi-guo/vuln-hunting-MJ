using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheWeakestBankOfAntarctica.Model;
using TheWeakestBankOfAntarctica.Utility;

namespace TheWeakestBankOfAntarctica.Data
{
    public static class DataAdapter
    {
        private static TWBA twba = null;

        public static void Init(TWBA model)
        {
            twba = model;
        }

        /* CWE-798 : Use of Hard coded Credentials
         * Patched by : Moavia Javed
         * Description : I have deleted the plain-text server, database, username and password variable here to aovid exposing the database to protential
         *               attackers who can visit this code. Instead, the server and database are put in the AppConfig.xml file, and the username and password
         *               will be prompted to the user asking them to input manually. 
         */
        /* CWE-522 : Insufficiently Protected Credentials
         * Patched by : Moavia Javed
         * Description : This CWE exists because of the same reason for CWE-798, the plain-text variables will expose the database server and access to it.
         *               The mitigation is done in the same way as CWE-798.
         */
        static void ConnectToRemoteDB()
        {
            string server = UtilityFunctions.GetValueFromAppConfig("server");
            string database = UtilityFunctions.GetValueFromAppConfig("database");

            Console.WriteLine("Enter the username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter the password: ");
            string password = Console.ReadLine();

            string connectionString = $"Server={server};Database={database};Uid={username};Pwd={password};";
            try
            {
                Console.WriteLine("Connected to MySQL server!");
                // We arent using the Remote DB here; however, this is a valid code in the App
            }
            catch (Exception ex) { }
        }


        public static Account GetAccountByAccountNumber(string accountNumber)
        {
            Account account = (from a in twba.GetAllAccounts()
                       where a.AccountNumber == accountNumber
                       select a).FirstOrDefault();

            return account; 
        }

        public static List<Account> GetAccountOwners(string customerId)
        {
            List<Account> customerAccounts = (from account in twba.GetAllAccounts()
                                    where account.AccountOwners.Contains(customerId)
                                    select account).ToList();

            return customerAccounts;
        }
        public static string CloseAccount(string accountNumber)
        {
            Account account = GetAccountByAccountNumber(accountNumber);
            twba.GetAllAccounts().Remove(account);
            account = null;
            return account.AccountNumber;
        }
    }
}
