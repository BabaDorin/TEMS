using System;
using System.Reflection;
using System.Web.Http;

namespace temsAPI.System_Files.Exceptions
{
    public static class GenericExceptionBuilder
    {
        public static HttpError Create(GenericException exception)
        {
            var properties = exception.GetType().GetProperties(BindingFlags.Instance
                                                             | BindingFlags.Public
                                                             | BindingFlags.DeclaredOnly);
            var error = new HttpError();
            
            foreach (var propertyInfo in properties)
                error.Add(propertyInfo.Name, propertyInfo.GetValue(exception, null));

            return error;
        }
    }
}
