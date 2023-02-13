namespace Procare.AddressValidation.Tester.Tests
{
    using NUnit.Framework;
    using System.Net;
    using System.Threading.Tasks;

    [TestFixture]
    public class AddressValidationServiceTests
    {
        private int retryNumber;
        private Uri addressValidationBaseUrl;
        [SetUp]
        public void SetUp()
        {
            addressValidationBaseUrl = new Uri("https://addresses.dev-procarepay.com");
        }

        [Test]
        public async Task GetAddressesAsync_ReturnsSuccessfulResponse_WhenRequestIsValid()
        {
            // Arrange

            using HttpClientFactory factory = new HttpClientFactory();
            using AddressValidationService addressService = new AddressValidationService(factory, false, addressValidationBaseUrl);
            var request = new AddressValidationRequest { Line1 = "1125 17th St Ste 1800", City = "Denver", StateCode = "CO", ZipCodeLeading5 = "80202" };

            // Act
            var response = await addressService.GetAddressesAsync(request).ConfigureAwait(false);

            // Assert
            var expectation = "{\"Count\":1,\"Addresses\":[{\"CompanyName\":null,\"Line1\":\"1125 17TH ST\",\"Line2\":\"STE 1800\",\"City\":\"DENVER\",\"StateCode\":\"CO\",\"Urbanization\":null,\"ZipCodeLeading5\":\"80202\",\"ZipCodeTrailing4\":\"2026\"}]}";
            Assert.That(response, Is.EqualTo(expectation));
        }

        [Test]
        public Task GetAddressesAsync_ThrowsException_WhenResponseIsNotSuccessful()
        {
            // Arrange
            using HttpClientFactory factory = new HttpClientFactory();
            using AddressValidationService addressService = new AddressValidationService(factory, false, addressValidationBaseUrl);
            var request = new AddressValidationRequest { Line1 = "invalid address", City = "invalid city", StateCode = "invalid state", ZipCodeLeading5 = "invalid zip" };

            // Act
            var exception = Assert.ThrowsAsync<Exception>(() => addressService.GetAddressesAsync(request));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Unfortunately, after multiple attempts, the API request has failed. Please check your network connection and try again later.\r\n" +
                                    " If the issue persists, kindly contact support with the following error code:[500]. \r\nThis will help us investigate the issue" +
                                    " and resolve it as soon as possible."));
            return Task.CompletedTask;
        }
    }

}