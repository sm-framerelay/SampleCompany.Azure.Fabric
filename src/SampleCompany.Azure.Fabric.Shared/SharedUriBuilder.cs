using System;
using System.Fabric;

namespace SampleCompany.Azure.Fabric.Shared
{
    public class SharedUriBuilder
    {
        public SharedUriBuilder(string serviceInstance)
        {
            ActivationContext = FabricRuntime.GetActivationContext();
            ServiceInstance = serviceInstance;
        }

        public SharedUriBuilder(ICodePackageActivationContext context, string serviceInstance)
        {
            ActivationContext = context;
            ServiceInstance = serviceInstance;
        }

        public SharedUriBuilder(ICodePackageActivationContext context, string applicationInstance, string serviceInstance)
        {
            ActivationContext = context;
            ApplicationInstance = applicationInstance;
            ServiceInstance = serviceInstance;
        }

        public string ApplicationInstance { get; set; }

        public string ServiceInstance { get; set; }

        public ICodePackageActivationContext ActivationContext { get; set; }

        public Uri ToUri()
        {
            string applicationInstance = ApplicationInstance;

            if (string.IsNullOrEmpty(applicationInstance))
            {
                applicationInstance = ActivationContext.ApplicationName.Replace("fabric:/", string.Empty);
            }

            return new Uri("fabric:/" + applicationInstance + "/" + ServiceInstance);
        }
    }
}
