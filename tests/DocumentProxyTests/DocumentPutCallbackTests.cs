using DocumentProxy;
using DocumentProxy.Models;
using DocumentProxyTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentProxyTests
{
    [TestClass]
    public class DocumentPutCallbackTests
    {
        [TestMethod]
        public async Task Run_WithValidRequest_ReturnsUpdatesDb()
        {
            // arrange
            var requestStatus = "PROCESSED";
            var requestBody = $"{{\"status\":\"{requestStatus}\", \"detail\":\"processed doc\"}}";
            var req = TestFactory.CreateHttpRequest(requestBody);
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            DocumentDetails documentDetails = new DocumentDetails
            {
                Id = Guid.NewGuid(),
                Body = "doc123.txt",
                CreatedOn = DateTime.UtcNow
            };

            // act
            var result = await DocumentPutCallback.Run(req, documentDetails.Id.ToString(), documentDetails, logger);

            // assert
            result.Should().BeOfType<NoContentResult>();
            documentDetails.Status.Should().HaveCount(1);
            documentDetails.Status[0].Status.Should().Be(requestStatus);
        }

        [TestMethod]
        public async Task Run_InvalidStatus_ReturnsBadRequest()
        {
            // arrange
            var requestStatus = "test";
            var requestBody = $"{{\"status\":\"{requestStatus}\", \"detail\":\"test doc\"}}";
            var req = TestFactory.CreateHttpRequest(requestBody);
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            DocumentDetails documentDetails = new DocumentDetails
            {
                Id = Guid.NewGuid(),
                Body = "doc123.txt",
                CreatedOn = DateTime.UtcNow
            };

            // act
            var result = await DocumentPutCallback.Run(req, documentDetails.Id.ToString(), documentDetails, logger);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
            documentDetails.Status.Should().HaveCount(0);
        }
    }
}
