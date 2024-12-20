﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Moq.Contrib.HttpClient;
using Autofac.Extras.Moq;
using ChitChat.Services;
using ChitChat.Models;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using System.Net.Http.Json;
using ChitChat.Helper.Exceptions;

namespace Chit_Chat_Tests
{
    public class HttpServiceTests
    {
        [Fact]
        public async Task PostLoginCredentialsAsync_ShouldReturnUserModel_IfCredentialsAreFound()
        {
            var mockedUserModel = new UserModel("Jack", "ProfilePicture", "ConnectionID");
            var mockedUserCredentials = new UserCredentials("foo", "bar");
            var userResponseModel = new UserResponseModel(mockedUserModel, "Works");
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Post, "https://localhost:5001/api/chat/Login")
                    .ReturnsResponse(JsonConvert.SerializeObject(userResponseModel), "application/json");
                var cls = mock.Create<HttpClient>();
                ISocketService httpService = new SocketService(cls);

                var expected = mockedUserModel;

                var actual = await httpService.PostLoginCredentialsAsync(mockedUserCredentials);

                mock.Mock<HttpMessageHandler>().VerifyRequest("https://localhost:5001/api/chat/Login", Times.Once());
                Assert.NotNull(actual);
                Assert.Equal(expected.DisplayName, actual.DisplayName);
                Assert.Equal(expected.ProfilePicture, actual.ProfilePicture);
                Assert.Equal(expected.ConnectionID, actual.ConnectionID);
            }
        }

        [Fact]
        public async Task PostLoginCredentialsAsync_ShouldThrow_IfCredentialsAreNotFound()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var mockedUserModel = new UserModel();
                var mockedUserCredentials = new UserCredentials("foo", "bar");
                var userResponseModel = new UserResponseModel(mockedUserModel, "Not Found");
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Post, "https://localhost:5001/api/chat/Login")
                  .ReturnsResponse(HttpStatusCode.NotFound, JsonConvert.SerializeObject(userResponseModel), "application/json");
                var cls = mock.Create<HttpClient>();
                ISocketService httpService = new SocketService(cls);



                await Assert.ThrowsAsync<LoginException>(() => httpService.PostLoginCredentialsAsync(mockedUserCredentials));
            }
        }

        [Fact]
        public async Task PostRegisterCredentialsAsync_ShouldBeCalled()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var mockedUserCredentials = new UserCredentials("foo", "bar");
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Post, "https://localhost:5001/api/chat/PostUser").
                ReturnsResponse(HttpStatusCode.OK);


                var cls = mock.Create<HttpClient>();
                ISocketService httpService = new SocketService(cls);
                await httpService.PostRegisterCredentialsAsync(mockedUserCredentials);

                mock.Mock<HttpMessageHandler>().VerifyRequest("https://localhost:5001/api/chat/PostUser", Times.Once());
            }
        }

        [Fact]
        public async Task PostRegisterCredentialsAsync_ShouldThrow_IfResponseNotOk()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var mockedUserCredentials = new UserCredentials("foo", "bar");
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Post, "https://localhost:5001/api/chat/PostUser").
                ReturnsResponse(HttpStatusCode.BadRequest);


                var cls = mock.Create<HttpClient>();
                ISocketService httpService = new SocketService(cls);

                await Assert.ThrowsAsync<RegistrationException>(() => httpService.PostRegisterCredentialsAsync(mockedUserCredentials));
            }
        }
        
        [Fact]
        public async Task PostRecoveryDataAsync_ShouldBeCalled()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Post, "https://localhost:5001/api/chat/PostEmail").
                ReturnsResponse(HttpStatusCode.OK);
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Post, "https://localhost:5001/api/chat/PostPassword").
                ReturnsResponse(HttpStatusCode.OK);


                var cls = mock.Create<HttpClient>();
                ISocketService httpService = new SocketService(cls);
                await httpService.PostRecoveryDataAsync("PostEmail", null);
                await httpService.PostRecoveryDataAsync("PostPassword", null);


                mock.Mock<HttpMessageHandler>().VerifyRequest("https://localhost:5001/api/chat/PostEmail",  Times.Once());
                mock.Mock<HttpMessageHandler>().VerifyRequest("https://localhost:5001/api/chat/PostPassword", Times.Once());
            }
        }

        [Fact]
        public async Task PostRecoveryDataAsync_ShouldThrow_IfResponseNotOK()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Post, "https://localhost:5001/api/chat/PostEmail").
                ReturnsResponse(HttpStatusCode.BadRequest);
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Post, "https://localhost:5001/api/chat/PostPassword").
                ReturnsResponse(HttpStatusCode.BadRequest);


                var cls = mock.Create<HttpClient>();
                ISocketService httpService = new SocketService(cls);

                await Assert.ThrowsAsync<RecoveryException>(() => httpService.PostRecoveryDataAsync("PostEmail", null));
                await Assert.ThrowsAsync<RecoveryException>(() => httpService.PostRecoveryDataAsync("PostPassword", null));
            }
        }

        [Fact]
        public async Task PostProfileImage_ShouldReturnPictureLink()
        {
            var imageUpdateModel = new ImageUploadDataModel("h54Bhau4jkI", null);
            var expected = "https://imgur.com/a/RandomImageLink";

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Post, "https://localhost:5001/api/chat/PostImage")
                    .ReturnsResponse(expected);
                var cls = mock.Create<HttpClient>();
                ISocketService httpService = new SocketService(cls);


                var actual = await httpService.PostProfileImage(imageUpdateModel);


                mock.Mock<HttpMessageHandler>().VerifyRequest("https://localhost:5001/api/chat/PostImage", Times.Once());
                Assert.Equal(expected, actual);
            }
        }
        [Fact]
        public async Task DeleteDataAsync_ShouldBeCalled()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var mockedObjectToDelete = new MessageModel();
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Delete, "https://localhost:5001/api/chat/DeleteMessage")
                    .ReturnsResponse(HttpStatusCode.OK);

                var cls = mock.Create<HttpClient>();
                ISocketService httpservice = new SocketService(cls);

                await httpservice.DeleteDataAsync(mockedObjectToDelete);

                mock.Mock<HttpMessageHandler>().VerifyRequest("https://localhost:5001/api/chat/DeleteMessage", Times.Once());
            }
        }

        [Theory]
        [InlineData("ChatHub")]
        [InlineData("Register")]
        [InlineData("Login")]
        [InlineData("GetHeartBeat")]
        [InlineData("Test1TestTest")]
        [InlineData("Test3Tes2tT3est")]
        [InlineData("PassingTestsAreTheBest")]
        public async Task PostDataAsync_ShouldCallCorrectEndPoint(string expected)
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<HttpMessageHandler>().SetupRequest(HttpMethod.Post, $"https://localhost:5001/api/chat/{expected}").
                ReturnsResponse(HttpStatusCode.OK);


                var cls = mock.Create<HttpClient>();
                ISocketService httpService = new SocketService(cls);
                await httpService.PostDataAsync(expected, null);

                mock.Mock<HttpMessageHandler>().VerifyRequest($"https://localhost:5001/api/chat/{expected}", Times.Once());
            }
        }

    }
}
