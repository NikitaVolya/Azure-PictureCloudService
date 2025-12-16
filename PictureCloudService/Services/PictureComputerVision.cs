using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace PictureCloudService.Services
{
    public class PictureComputerVisionService
    {
        private string subscriptionKey;
        private string endpoint;

        private ComputerVisionClient _client;

        private static readonly List<VisualFeatureTypes?> _features = new List<VisualFeatureTypes?>()
        {
            VisualFeatureTypes.Adult,
            VisualFeatureTypes.Description,
        };

        public PictureComputerVisionService()
        {
            subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_KEY") ?? throw new ArgumentNullException("COMPUTER_VISION_KEY is not set");
            endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT") ?? throw new ArgumentNullException("COMPUTER_VISION_ENDPOINT is not set");

            _client = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey))
            {
                Endpoint = endpoint
            };
        }

        public async Task<ImageAnalysis> AnalyzePictureAsync(IFormFile file)
        {
            ImageAnalysis analysis;

            using (var imageStream = file.OpenReadStream())
            {
                analysis = await _client.AnalyzeImageInStreamAsync(imageStream, _features);
            }

            return analysis;
        }
    }
}
