using BocchiTracker.ServiceClientAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.ServiceClientAdapters
{


    public class PasswordServiceTests
    {
        private const string TestPassword = "Password123";
        private const string TestMacAddress = "12:34:56:78:90:AB";

        [Fact]
        public void EncryptPassword_ValidData_ReturnsEncryptedPassword()
        {
            // Arrange
            var passwordService = new PasswordService();

            // Act
            var encryptedPassword = passwordService.Encrypy(TestPassword, TestMacAddress);

            // Assert
            Assert.NotNull(encryptedPassword);
            Assert.NotEqual(TestPassword, encryptedPassword);
        }

        [Fact]
        public void DecryptPassword_ValidData_ReturnsDecryptedPassword()
        {
            // Arrange
            var passwordService = new PasswordService();
            var encryptedPassword = passwordService.Encrypy(TestPassword, TestMacAddress);

            // Act
            var decryptedPassword = passwordService.Decrypy(encryptedPassword, TestMacAddress);

            // Assert
            Assert.NotNull(decryptedPassword);
            Assert.Equal(TestPassword, decryptedPassword);
        }
    }
}
