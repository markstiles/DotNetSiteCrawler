using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SiteIndexer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            var serviceCollection = new ServiceCollection();
            IocConfig.Configure(serviceCollection);

            var resolver = new DefaultDependencyResolver(serviceCollection.BuildServiceProvider());
            DependencyResolver.SetResolver(resolver);
        }
    }

    public class DefaultDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _provider;

        public DefaultDependencyResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public object GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _provider.GetServices(serviceType);
        }
    }
}
