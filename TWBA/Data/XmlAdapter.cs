using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheWeakestBankOfAntarctica.Model;
using System.IO;
using System.Xml.Serialization;
using TheWeakestBankOfAntarctica.Utility;

namespace TheWeakestBankOfAntarctica.Data
{
    public static class XmlAdapter
    {
        /* CWE-22: Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')
         * Patched By: Bilal
         * Description: I have taken several steps to ensure that the provide path is valid and should be accessable by the app:
         *              I have created a IsPathValid method in UtilityFunctions class that ensures that the path is not empty or
         *              null and also ensures there are no invalid characters like wild cards * and ? that can be exploited
         *              to browse through the directory.
         *              I have checked the normalised path to ensure that the system is access the root directory of the project 
         *              only, it is not overwritting path to unauthorised directory locations. 
         *              Sandboxing the File structure for the app. 
         */
        private static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.FullName;

        private static string relativeFilePathForCustomerData = Path.Combine(projectDirectory, "BankDataFiles\\Customers\\CustomerData.xml");
        private static string relativeFilePathForTransactions = Path.Combine(projectDirectory, "BankDataFiles\\Transactions\\");
        private static string relativeFilePathForAccountsData = Path.Combine(projectDirectory, "BankDataFiles\\Accounts\\AccountsData.xml");
        public static void SerializeCustomerDataToXml(List<Customer> customers)
        {
            var serializer = new XmlSerializer(typeof(List<Customer>));
            // check if the path is not null and valid
            UtilityFunctions.IsPathValid(relativeFilePathForCustomerData);
            // Normalize the path to get the absolute path (in canonical form)  
            string normalizedFullPath = Path.GetFullPath(relativeFilePathForCustomerData);
            
            if (!normalizedFullPath.StartsWith(projectDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Access denied: Attempted path traversal detected.");
            }

            using (TextWriter writer = new StreamWriter(relativeFilePathForCustomerData))
            {
                serializer.Serialize(writer, customers);
            }
        }

        
        public static bool SerializeAccountDataToXml(List<Account> accounts)
        {
            var serializer = new XmlSerializer(typeof(List<Account>));
            // check if the path is not null and valid
            UtilityFunctions.IsPathValid(relativeFilePathForAccountsData);
            // Normalize the path to get the absolute path (in canonical form)  
            string normalizedFullPath = Path.GetFullPath(relativeFilePathForAccountsData);

            if (!normalizedFullPath.StartsWith(projectDirectory, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Access denied: Attempted path traversal detected.");
                return false;
            }

            using (TextWriter writer = new StreamWriter(relativeFilePathForAccountsData))
            {
                serializer.Serialize(writer, accounts);
            }
            return true;
        }

        public static void SearlizeTransaction(Transaction transaction)
        {
            string updatedPath = Path.Combine(relativeFilePathForTransactions, transaction.TransactionId + ".xml");
            var serializer = new XmlSerializer(typeof(Transaction));
            using (TextWriter writer = new StreamWriter(updatedPath))
            {
                serializer.Serialize(writer, transaction);
            }
        }

        // Method to deserialize a single XML file into a Transaction object
        public static List<Transaction> DeserializeTransaction(string filepath)
        {
            List<Transaction> transactions = new List<Transaction>();

            // Get all XML files in the specified folder
            string[] xmlFiles = Directory.GetFiles(filepath, "*.xml");

            // Create an XmlSerializer for the Transaction class
            XmlSerializer serializer = new XmlSerializer(typeof(Transaction));

            foreach (string filePath in xmlFiles)
            {
                try
                {
                    // Deserialize each XML file into a Transaction object
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        Transaction transaction = (Transaction)serializer.Deserialize(reader);
                        transactions.Add(transaction); // Add the deserialized transaction to the list
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to deserialize file {filePath}: {ex.Message}");
                    // Optionally, you could skip or log errors but continue processing other files
                }
            }

            return transactions; // Return the list of deserialized transactions
        }
    }
}
    


