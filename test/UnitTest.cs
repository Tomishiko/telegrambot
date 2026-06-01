namespace test;

using telegramBot.Services;
using telegramBot.Models;
using telegramBot.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;
using FluentAssertions;
using telegramBot.Endpoints;

public class FileProcessingServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _client;
    private readonly Mock<IOptions<ApiOptions>> _optionsMock;
    //private readonly Mock<ILogger<FileProcessingService>> _loggerMock;
    private readonly ApiOptions _apiOptions;

    //public FileProcessingServiceTests()
    //{
    //    _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    //    _client = new HttpClient(_handlerMock.Object);

    //    _apiOptions = new ApiOptions
    //    {
    //        BaseUrl = "https://api.basurl.com",
    //        HookUrl = "https://api.hookurl.com",
    //        botToken = "testToken"
    //    };

    //    _optionsMock = new Mock<IOptions<ApiOptions>>();
    //    _optionsMock.Setup(o => o.Value).Returns(_apiOptions);

    //    //_loggerMock = new Mock<ILogger<FileProcessingService>>();
    //}


    [Fact]
    public async Task ExtractYoutubeLinksAsync_WithEncodedHtml_ReturnsDecodedUniqueLinks()
    {


        // Create the HttpClient using our mocked handler
        var httpClient = new HttpClient();
        var service = new YoutubeExtractorService(httpClient);

        string testUrl = "https://share.google/XG7Zie8JOA2RcOQWr";

        // 2. Act
        var result = await service.ExtractYoutubeLinksAsync(testUrl);

        // 3. Assert
        Assert.NotNull(result);
        var linksList = result.ToList();

        // Expecting exactly 2 links: The distinct watch link and the protocol-fixed embed link
        Assert.Equal(1, linksList.Count);
        foreach (var link in linksList)
        {
            Assert.True(
                link.Contains("youtube.com") || link.Contains("youtu.be"),
                $"Extracted link '{link}' does not match a YouTube domain."
            );

            Assert.StartsWith("https://", link);
        }

        // Output the live links to the test runner console for visibility
        foreach (var link in linksList)
        {
            Console.WriteLine($"Found live link: {link}");
        }

    }

    [Fact]
    public void ExtractUrls_WithMixedContent_ReturnsNormalizedUniqueUrls()
    {
        // Arrange
        // This text simulates a messy chat message with standard links,
        // a 'www.' shorthand link, trailing punctuation, and a duplicate link.
        string message = "Check this out: https://share.google/XG7Zie8JOA2RcOQWr and also www.youtube.com/watch?v=dQw4w9WgXcQ. Don't forget to visit https://share.google/XG7Zie8JOA2RcOQWr!";

        // Act
        var result = message.ExtractUrls().ToList();

        foreach(var res in result){
            Console.WriteLine(res);
        }
        // Assert
        Assert.NotNull(result);

        // Ensure the duplicate link was filtered out
        Assert.Equal(2, result.Count);

        // Verify standard URL extraction and boundary handling (the trailing '!' shouldn't be included)
        Assert.Contains("https://share.google/XG7Zie8JOA2RcOQWr", result);

        // Verify that the 'www.' shorthand was correctly normalized to 'https://'
        Assert.Contains("https://www.youtube.com/watch?v=dQw4w9WgXcQ", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Hello world! This is just a plain text message with no links in it.")]
    [InlineData("Check out my website at google.com without the prefix.")]
    public void ExtractUrls_WithNoValidUrls_ReturnsEmptyCollection(string input)
    {
        // Act
        var result = input.ExtractUrls();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

}

