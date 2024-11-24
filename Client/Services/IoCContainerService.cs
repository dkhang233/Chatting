using System.Net.Http;
using System.Net.Sockets;
using Autofac;

namespace ChitChat.Services
{
    public static class IoCContainerService
    {
        public static IContainer _container;
        public static void Register()
        {
            var container = new ContainerBuilder();
            container.RegisterType<HttpService>()
                .As<IHttpService>()
                .SingleInstance()
                .WithParameter("httpClient", new HttpClient());

            // Đăng ký dịch vụ.
            container.RegisterType<SocketService>()
                .As<ISocketService>()
                .SingleInstance();

            _container = container.Build();
        }
    }
}
