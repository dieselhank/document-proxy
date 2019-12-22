using DocumentProxy;
using DocumentProxy.Models;
using DocumentProxyTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentProxyTests
{
    [TestClass]
    public class DocumentStatusRequestTests
    {
        [TestMethod]
        public async Task Run_WithValidRequest_ReturnsCurrentStatus()
        {
            // arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext());
            var logger = TestFactory.CreateLogger(LoggerTypes.List);
            var now = DateTime.UtcNow;

            DocumentDetails documentDetails = new DocumentDetails
            {
                Id = Guid.NewGuid(),
                Body = "doc123.txt",
                CreatedOn = now,
                Status = {
                    new DocumentStatus
                    {
                        Status = "ERROR",
                        Detail = "processing error",
                        CreatedOn = now.AddMinutes(10)
                    }
                }
            };

            // act
            var result = await DocumentStatusRequest.Run(req, documentDetails.Id.ToString(), documentDetails, logger);

            // assert
            result.Should().BeOfType<OkObjectResult>();

            var documentStatusResponse = (DocumentStatusResponse)((OkObjectResult)result).Value;
            documentStatusResponse.CreatedOn.Should().Be(now);
            documentStatusResponse.Status.Should().Be("ERROR");
            documentStatusResponse.UpdatedOn.Should().Be(now.AddMinutes(10));
        }

        [TestMethod]
        public async Task Run_WithDocumentNotFound_ReturnsBadRequest()
        {
            // arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext());
            var logger = TestFactory.CreateLogger(LoggerTypes.List);

            // act
            var result = await DocumentStatusRequest.Run(req, Guid.NewGuid().ToString(), null, logger);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
