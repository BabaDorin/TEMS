using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.System_Files.TEMSInitializer
{
    public static class Initializer
    {
        /// <summary>
        /// Runs a list of actions in order to make sure the project is ready to be used.
        /// </summary>
        public static void PrepareSolution()
        {
            var initializerActionTypes = GetAllTypesThatImplementInterface<IInitializerAction>();

            foreach(Type action in initializerActionTypes)
            {
                var actionInstance = (IInitializerAction)Activator.CreateInstance(action);
                actionInstance.Start();
            }
        }

        private static IEnumerable<Type> GetAllTypesThatImplementInterface<T>()
        {
            return System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
        }
    }
}
