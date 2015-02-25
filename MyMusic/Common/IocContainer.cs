using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.Common
{
    public static class IocContainer
    {
        /// <summary>The Unity Ioc object</summary>
        public static readonly IUnityContainer Container = new UnityContainer();

        /// <summary>
        /// Gets a concrete implementation of the specified interface. The actual type returned depends on the binding 
        /// which has been established between the interface and a concrete type.
        /// </summary>
        /// <typeparam name="T">The interface for the required type</typeparam>
        /// <returns>Returns a concrete implementation of the specified interface</returns>
        public static T Get<T>() where T : class
        {
            return Get<T>(null);
        }

        /// <summary>
        /// Gets a concrete implementation of the specified interface. The actual type returned depends on the binding 
        /// which has been established between the interface and a concrete type. You may optionally supply constructor 
        /// parameters that will be injected into the resolved type. For example, if the target type constructor is 
        /// defined as ctor(string name, int index) then pass ("name", "my name val", "index", 99) to this method.
        /// </summary>
        /// <typeparam name="T">The interface for the required type</typeparam>
        /// <param name="parameters">Array of parameter names and values to be passed to the type's constructor</param>
        /// <returns>Returns a concrete implementation of the specified interface</returns>
        public static T Get<T>(params object[] parameters) where T : class
        {
            ResolverOverride[] ctorParams = null;

            try
            {
                if (parameters != null)
                {
                    ctorParams = new ResolverOverride[parameters.Length / 2];
                    for (var i = 0; i < parameters.Length; i += 2)
                        ctorParams[i] = new ParameterOverride(parameters[i] as string, parameters[i + 1]);
                }

                var resolvedObject = ctorParams == null ? Container.Resolve<T>() : Container.Resolve<T>(ctorParams);
                if (resolvedObject != null) return resolvedObject;

                throw new Exception();
            }
            catch (Exception ex)
            {
                //Logger.Log(ex, "Fatal exception during IoC resolution of binding for type " + typeof(T).Name);
                return default(T);
            }
        }
    }
}
