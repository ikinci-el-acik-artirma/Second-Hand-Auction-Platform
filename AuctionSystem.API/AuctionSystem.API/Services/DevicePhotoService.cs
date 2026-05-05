using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AuctionSystem.API.Data;
using AuctionSystem.API.DTOs.DevicePhoto;
using AuctionSystem.API.Models;
using Microsoft.AspNetCore.Http;

namespace AuctionSystem.API.Services
{
    public class DevicePhotoService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public DevicePhotoService(
            ApplicationDbContext context,
            HttpClient httpClient,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
            _environment = environment;
        }

        public async Task<DevicePhotoResponse?> UploadAndDetectAsync(DevicePhotoUploadRequest request)
        {
            if (request.Photo == null || request.Photo.Length == 0)
            {
                return null;
            }

            var detectedDeviceType = await DetectDeviceTypeAsync(request.Photo);
            var photoUrl = await SavePhotoAsync(request.Photo);

            var devicePhoto = new DevicePhoto
            {
                PhotoUrl = photoUrl,
                DetectedDeviceType = detectedDeviceType,
                UserId = request.UserId
            };

            _context.DevicePhotos.Add(devicePhoto);
            await _context.SaveChangesAsync();

            return new DevicePhotoResponse
            {
                Id = devicePhoto.Id,
                PhotoUrl = devicePhoto.PhotoUrl,
                DetectedDeviceType = devicePhoto.DetectedDeviceType,
                UserId = devicePhoto.UserId
            };
        }

        private async Task<string> DetectDeviceTypeAsync(IFormFile photo)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("OpenAI API key is not configured.");
            }

            using var memoryStream = new MemoryStream();
            await photo.CopyToAsync(memoryStream);

            var base64Image = Convert.ToBase64String(memoryStream.ToArray());
            var contentType = string.IsNullOrWhiteSpace(photo.ContentType)
                ? "image/jpeg"
                : photo.ContentType;

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "text",
                                text = "Analyze the image and classify the main electronic device. Return only one word from this list: Phone, Laptop, Tablet, DesktopComputer, Unknown. Do not explain."
                            },
                            new
                            {
                                type = "image_url",
                                image_url = new
                                {
                                    url = $"data:{contentType};base64,{base64Image}"
                                }
                            }
                        }
                    }
                },
                max_tokens = 20
            };

            var json = JsonSerializer.Serialize(requestBody);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions");

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                return "Unknown";
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var aiText = ExtractTextFromOpenAiResponse(responseJson);

            return NormalizeDeviceType(aiText);
        }

        private async Task<string> SavePhotoAsync(IFormFile photo)
        {
            var webRootPath = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var uploadsFolder = Path.Combine(webRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(photo.FileName);

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".jpg";
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await photo.CopyToAsync(fileStream);

            return $"/uploads/{fileName}";
        }

        private static string ExtractTextFromOpenAiResponse(string responseJson)
        {
            using var document = JsonDocument.Parse(responseJson);

            var choices = document.RootElement.GetProperty("choices");

            if (choices.GetArrayLength() == 0)
            {
                return string.Empty;
            }

            var message = choices[0].GetProperty("message");

            if (message.TryGetProperty("content", out var content))
            {
                return content.GetString() ?? string.Empty;
            }

            return string.Empty;
        }

        private static string NormalizeDeviceType(string aiText)
        {
            var value = aiText.Trim().ToLower();

            if (value.Contains("phone") ||
                value.Contains("smartphone") ||
                value.Contains("mobile"))
            {
                return "Phone";
            }

            if (value.Contains("laptop") ||
                value.Contains("notebook"))
            {
                return "Laptop";
            }

            if (value.Contains("tablet") ||
                value.Contains("ipad"))
            {
                return "Tablet";
            }

            if (value.Contains("desktop") ||
                value.Contains("desktopcomputer") ||
                value.Contains("computer") ||
                value.Contains("pc"))
            {
                return "DesktopComputer";
            }

            return "Unknown";
        }
    }
}