using Xunit;
using AuctionSystem.API.Models;

namespace AuctionSystem.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void User_ShouldSetPropertiesCorrectly()
        {
            var testUser = new User();

            testUser.Id = 1;
            testUser.Email = "ornek@mail.com";
            testUser.Role = "Buyer";

            Assert.Equal(1, testUser.Id);
            Assert.Equal("ornek@mail.com", testUser.Email);
            Assert.Equal("Buyer", testUser.Role);
        }
    }
}
