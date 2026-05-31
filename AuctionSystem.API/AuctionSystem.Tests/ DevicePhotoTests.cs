using AuctionSystem.API.Models;
using Xunit;

namespace AuctionSystem.Tests
{
    public class DevicePhotoTests
    {
        [Fact]
        public void DevicePhoto_ShouldSetDeviceTypePropertiesCorrectly()
        {
            var photo = new DevicePhoto();

            photo.Id = 1;
            photo.PhotoUrl = "/uploads/laptop.png";
            photo.DetectedDeviceType = "Laptop";
            photo.UserId = 5;

            Assert.Equal(1, photo.Id);
            Assert.Equal("/uploads/laptop.png", photo.PhotoUrl);
            Assert.Equal("Laptop", photo.DetectedDeviceType);
            Assert.Equal(5, photo.UserId);
        }

        [Fact]
        public void DevicePhoto_DefaultDeviceType_ShouldBeUnknown()
        {
            var photo = new DevicePhoto();

            Assert.Equal("Unknown", photo.DetectedDeviceType);
        }

        [Fact]
        public void DevicePhoto_DetectedDeviceType_ShouldNotBeEmpty()
        {
            var photo = new DevicePhoto
            {
                PhotoUrl = "/uploads/laptop.png",
                DetectedDeviceType = "Laptop",
                UserId = 1
            };

            Assert.False(string.IsNullOrWhiteSpace(photo.DetectedDeviceType));
        }

        [Theory]
        [InlineData("Phone")]
        [InlineData("Laptop")]
        [InlineData("Tablet")]
        [InlineData("DesktopComputer")]
        [InlineData("Unknown")]
        public void DevicePhoto_DetectedDeviceType_ShouldBeAllowedValue(string deviceType)
        {
            var allowedTypes = new[]
            {
                "Phone",
                "Laptop",
                "Tablet",
                "DesktopComputer",
                "Unknown"
            };

            Assert.Contains(deviceType, allowedTypes);
        }
    }
}