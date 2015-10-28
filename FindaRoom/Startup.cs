using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FindaRoom.Startup))]
namespace FindaRoom
{
    public partial class Startup
    {
        public class MyProvider : IUserIdProvider
        {
            public string GetUserId(IRequest request)
            {
                var userId = request.User.Identity.Name;
                return userId.ToString();
            }
        }
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new MyProvider());
            ConfigureAuth(app);
                        app.MapSignalR();
        }
    }
}
