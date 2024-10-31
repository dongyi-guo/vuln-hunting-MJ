using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheWeakestBankOfAntarctica.Model
{
    class Admin : User
    {
        public DateTime StartDate { get; set; }
        public Position Position { get; set; }
        public Role Role { get; set; }
        public string BranchName { get; set; }
        public string BranchId { get; set; }
        public string AdminId { get; private set; }

        /* CWE-522: Insufficiently Protected Credentials
         * Patched by: Moavia Javed
         * Description: The password passed into this function is plain-text, for security concerns, a hashed password should always
         *              be used for login.
         *              The password attribute for this class and User has been changed into hashedCredentials to store the hashes.
         */
        public Admin(string govId, string name, string lName, string email, string hashedCredentials,
            Position position, Role role, string branchName, string branchId,
            string address, string phoneNumber) :
            base(govId, name, lName, email, hashedCredentials, address, phoneNumber)
        {
            base.GovId = govId;
            base.Name = name;
            base.LastName = lName;
            base.Email = email;
            base.HashedCredentials = hashedCredentials;
            base.Address = Address;
            base.PhoneNumber = phoneNumber;

            StartDate = DateTime.Now;
            Position = position;
            Role = role;
            BranchName = branchName;
            BranchId = branchId;
            AdminId = GenerateAdminId(8); // maximum length of an Id must be 8 characters
        }

        public string GenerateAdminId(int max)
        {
            Random random = new Random();
            string id = "";

            for (int i = 0; i < max; i++)
            {
                id += random.Next(10).ToString();
            }

            return BranchId + "-" + id + "A";
        }
    }

    public enum Position
    {
        manager, specialist, attendent, representative, none
    }

    public enum Role
    {
        Admin, Teller, none
    }

}
