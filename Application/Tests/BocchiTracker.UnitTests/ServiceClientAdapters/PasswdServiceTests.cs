using BocchiTracker.ServiceClientAdapters;
using Moq;
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

        [Fact]
        public void EncryptPassword_ValidData_ReturnsEncryptedPassword()
        {
            // Arrange
            var passwordService = new PasswordService();

            // Act
            var encryptedPassword = passwordService.Encrypy(TestPassword);

            // Assert
            Assert.NotNull(encryptedPassword);
            Assert.NotEqual(TestPassword, encryptedPassword);
        }

        [Fact]
        public void DecryptPassword_ValidData_ReturnsDecryptedPassword()
        {
            // Arrange
            var passwordService = new PasswordService();

            // Act
            var encryptedPassword = passwordService.Encrypy(TestPassword);
            var decryptedPassword = passwordService.Decrypy(encryptedPassword);

            // Assert
            Assert.NotNull(decryptedPassword);
            Assert.Equal(TestPassword, decryptedPassword);
        }
    }
}
