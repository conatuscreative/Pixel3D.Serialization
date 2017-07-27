using System;
using System.Linq;
using System.Reflection;

namespace Pixel3D.Serialization.MethodProviders
{
    class FallbackMethodProvider : MethodProvider
    {
        MethodProvider[] providers;

        private FallbackMethodProvider(params MethodProvider[] providers)
        {
            this.providers = providers;
        }

        public static MethodProvider Combine(params MethodProvider[] providers)
        {
            var unwrappedProviders = providers.SelectMany((MethodProvider provider) =>
            {
                var fallbackProvider = provider as FallbackMethodProvider;
                if(fallbackProvider != null)
                    return fallbackProvider.providers;

                if(provider is EmptyMethodProvider)
                    return new MethodProvider[] { };

                return new[] { provider };
            });

            if(unwrappedProviders.Count() == 0)
                return new EmptyMethodProvider();
            if(unwrappedProviders.Count() == 1)
                return unwrappedProviders.First();
            else
                return new FallbackMethodProvider(unwrappedProviders.ToArray());
        }


        public override MethodInfo GetMethodForType(Type type)
        {
            foreach(var provider in providers)
            {
                MethodInfo methodInfo = provider.GetMethodForType(type);
                if(methodInfo != null)
                    return methodInfo;
            }
            return null;
        }
    }
}
