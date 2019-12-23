using DocumentProxy;
using DocumentProxy.Models;
using DocumentProxyTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentProxyTests
{
    [TestClass]
    public class DocumentRequestTests
    {
        private Mock<IAsyncCollector<DocumentDetails>> _documentDetailsCollector;

        [TestInitialize]
        public void Init()
        {
            _documentDetailsCollector = new Mock<IAsyncCollector<DocumentDetails>>();
        }

        [TestMethod]
        public async Task Run_WithValidRequest_ReturnsIdAndSavesToDb()
        {
            // arrange
            var requestBody = "doc123.txt";
            var req = TestFactory.CreateHttpRequest($"{{\"body\": \"{requestBody}\"}}");
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            DocumentDetails documentDetails = null;
            _documentDetailsCollector.Setup(x => x.AddAsync(It.IsAny<DocumentDetails>(), default(CancellationToken)))
                .Callback((DocumentDetails details, CancellationToken token) => documentDetails = details);

            // act
            var result = await DocumentRequest.Run(req, _documentDetailsCollector.Object, logger);

            // assert
            result.Should().BeOfType<OkObjectResult>();
            var resultBody = (string)((OkObjectResult)result).Value;
            resultBody.Should().NotBeNullOrEmpty();
            documentDetails.Should().NotBeNull();
            documentDetails.Id.Should().Be(resultBody);
            documentDetails.Body.Should().Be(requestBody);
        }

        [TestMethod]
        public async Task Run_WithNoBodyInRequest_ReturnsBadRequest()
        {
            // arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext());
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            DocumentDetails documentDetails = null;
            _documentDetailsCollector.Setup(x => x.AddAsync(It.IsAny<DocumentDetails>(), default(CancellationToken)))
                .Callback((DocumentDetails details, CancellationToken token) => documentDetails = details);

            // act
            var result = await DocumentRequest.Run(req, _documentDetailsCollector.Object, logger);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestMethod]
        public async Task Run_DbSaveError_ReturnsStatus500InternalServerError()
        {
            // arrange
            var requestBody = "doc123.txt";
            var req = TestFactory.CreateHttpRequest($"{{\"body\": \"{requestBody}\"}}");
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            _documentDetailsCollector.Setup(x => x.AddAsync(It.IsAny<DocumentDetails>(), default(CancellationToken)))
                .Throws(new Exception("Error calling Cosmos DB"));

            // act
            var result = await DocumentRequest.Run(req, _documentDetailsCollector.Object, logger);

            // assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusCode = ((StatusCodeResult)result).StatusCode;
            statusCode.Should().Be(500);
        }
    }
}
