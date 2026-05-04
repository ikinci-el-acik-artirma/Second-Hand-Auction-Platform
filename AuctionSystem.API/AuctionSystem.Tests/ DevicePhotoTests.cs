using AuctionSystem.API.Models;
using Xunit;

namespace AuctionSystem.Tests
{
    public class DevicePhotoTests
    {
        [Fact]
        public void DevicePhoto_ShouldSetPropertiesCorrectly()
        {
            var photo = new DevicePhoto();

            photo.Id = 1;
            photo.PhotoUrl = "https://example.com/phone.jpg";
            photo.AiAssessmentResult = "Ekran sağlam, kasada çizikler var.";
            photo.UserId = 5;

            Assert.Equal(1, photo.Id);
            Assert.Equal("https://example.com/phone.jpg", photo.PhotoUrl);
            Assert.Equal("Ekran sağlam, kasada çizikler var.", photo.AiAssessmentResult);
            Assert.Equal(5, photo.UserId);
        }
    }
}