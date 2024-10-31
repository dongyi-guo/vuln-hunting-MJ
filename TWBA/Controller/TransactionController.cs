using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using TheWeakestBankOfAntarctica.Data;
using TheWeakestBankOfAntarctica.Model;
using TheWeakestBankOfAntarctica.Utility;
using TheWeakestBankOfAntarctica.View;

namespace TheWeakestBankOfAntarctica.Controller
{
    public static class TransactionController
    {
        private static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.FullName;
        /* CWE-306: Missing Authentication for Critical Function
         * Patched by: Bilal
         * Description: I have created two global variables in AccessController called IsLoggedIn and LoggedInUser
         *              IsLoggedIn is set to True as soon as a valid user logs in, and also sets the login of that user
         *              in LoggedInUser variable
         *              anywhere in the code of this whole app, i can check if the user is logged in or not
         *              by calling AccessController.IsLoggedIn
         *              if its true the critical operation of transfer will execute otherwise it wont.
         */
        /* CWE-20: Improper Input Validation
         * Patched by: Moavia Javed
         * Description: A zero and negative check for the amount argument has been created to refuse
         *              if amount argument is found with negative values, the function returns false to indicate a failed transaction.
         */
        public static bool TransferBetweenAccounts(Account sAccount, Account dAccount, double amount)
        {
            if (amount <= 0) return false;
            if (AccessController.IsLoggedIn)
            {
                Transaction transaction = new Transaction(sAccount.AccountNumber, dAccount.AccountNumber, amount);
                sAccount.AccountBalance = sAccount.AccountBalance - amount;
                dAccount.AccountBalance = dAccount.AccountBalance + amount;
                XmlAdapter.SearlizeTransaction(transaction);

                return true;
            }
            return false;
        }

        public static List<Customer> SearchByAccountNumber(Account account, List<Customer> customers)
        {
            List<Customer> accountOwners = new List<Customer>();
            // Get the list of owner IDs from the account
            List<string> ownerIds = account.AccountOwners;
            // Iterate through the provided customers list and match them by CustomerId
            foreach (var customer in customers)
            {
                // If the customer's ID is found in the account's owner list, add them to the result list
                if (ownerIds.Contains(customer.CustomerId))
                {
                    accountOwners.Add(customer);
                }
            }
            // Return the list of owners (Customer objects)
            return accountOwners;
        }

        /* CWE-20: Improper Input Validation
         * Patched by: Moavia Javed
         * Description: A negative check for the amount argument has been created to refuse any invalid transactions
         *              with negative transferring amounts.
         *              if amount argument is found with negative values, the function returns false to indicate a failed deposit.
         */
        /* CWE-306: Missing Authentication for Critical Function
         * Patched by: Moavia Javed
         * Description: Same as mentioned by Dr. Amin, this action should only happen when
         *              IsLoggedIn is set to True as soon as a valid user logs in,
         *              and also sets the login of that user in LoggedInUser variable.
         *              anywhere in the code of this whole app, i can check if the user is logged in or not by calling
         *              AccessController.IsLoggedIn if its true the critical operation of transfer will execute otherwise it wont.
         */
        public static bool Deposit(Account account, double amount)
        {
            if (amount < 0) return false;
            if (!AccessController.IsLoggedIn) return false;
            Transaction transaction = new Transaction(account.AccountNumber, amount, TypeOfTransaction.Deposit);
            account.AccountBalance = account.AccountBalance + amount;
            XmlAdapter.SearlizeTransaction(transaction);
            return true;
        }

        /* CWE-20: Improper Input Validation
         * Patched by: Moavia Javed
         * Description: A negative check for the amount argument has been created to refuse any invalid transactions 
         *              with negative transferring amounts. If amount argument is found with negative values, 
         *              the function returns false to indicate a failed withdrawl.
         */
        public static bool Withdrawl(Account account, double amount)
        {
            if (amount < 0) return false;
            if (!AccessController.IsLoggedIn) return false;
            Transaction transaction = new Transaction(account.AccountNumber, amount, TypeOfTransaction.Withdrawl);
            account.AccountBalance = account.AccountBalance - amount;
            XmlAdapter.SearlizeTransaction(transaction);
            return true;
        }

        /* CWE-502: Deserialization of Untrusted Data
         * Patched by: Moavia Javed
         * Description: The file needs to be deserialised can be modified or corrupted externally, for XmlAdapter.DeserializeTransaction(), a exception will be
         *              thrown and handled if the deserialisation failed, which kinda mitigate this CWE.
         */
        /* CWE-22: Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')
         * Patched By: Moavia Javed
         * Description: Now, this function is taking a static filepath, but in its potential, future implementation could change this function to take the
         *              filepath as an argument, user uses this function could potentially perform file path traversal, by utilising wildcards ("*" or "?") and
         *              upper layer reference ("..") to access the filesystem where they are not permitted to. Hence, a check on all characters mentioned above,
         *              and transferring the file path as absolute path as always is implemented here.
         *              The UtilityFunctions.IsPathValid() has been updated to cover all situations mentioned above.
         */
        public static List<Transaction> GetAllTransactions()
        {
            string filepath = "C:\\Windows\\Config.sys";

            if (!UtilityFunctions.IsPathValid(filepath))
            {
                throw new FileLoadException("Access denied: Attempted path traversal detected.");
            }

            string fullpath = Path.GetFullPath(filepath);

            if (!fullpath.StartsWith(projectDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new FileLoadException("Access denied: Attempted path traversal detected.");
            }


            if (!System.IO.File.Exists(filepath))
            {
                throw new FileNotFoundException("Transaction Files Not Existed");
            }
            return XmlAdapter.DeserializeTransaction(filepath);

        }
    }
}
