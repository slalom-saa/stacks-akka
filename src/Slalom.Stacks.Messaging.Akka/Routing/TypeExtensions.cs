using System;
using System.Linq;
using Slalom.Stacks.Reflection;

namespace Slalom.Stacks.Messaging.Routing
{
    public static class TypeExtensions
    {
        public static Type GetRequestType(this Type type)
        {
            var actorType = type.GetBaseTypes().FirstOrDefault(
                e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(UseCaseActor<,>));

            return actorType != null ? actorType.GetGenericArguments()[0] : null;
        }
    }
}