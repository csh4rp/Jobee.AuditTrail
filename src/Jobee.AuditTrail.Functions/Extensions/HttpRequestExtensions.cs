using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Jobee.AuditTrail.Functions.Extensions;

public static class HttpRequestExtensions
{
    public static T BindModel<T>(this IQueryCollection queryCollection) where T : new()
    {
        var model = new T();
        
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var propertyInfo in properties)
        {
            if (!queryCollection.TryGetValue(propertyInfo.Name, out var values))
            {
                continue;
            }

            if (propertyInfo.PropertyType.IsArray)
            {
                var type = propertyInfo.PropertyType.GetElementType()!;
                var array = Array.CreateInstance(type, values.Count);

                var index = 0;
                foreach (var value in values)
                {
                    array.SetValue(Convert.ChangeType(value, type), index);
                    index++;
                }
            }
            else
            {
                propertyInfo.SetValue(model, Convert.ChangeType(values, propertyInfo.PropertyType));
            }
        }

        return model;
    }
}