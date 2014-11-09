using System;
using System.Linq;

namespace Rikrop.Core.Wcf
{
    internal static class InterfaceExtractor
    {
        public static Type[] GetDirectInterfaces(this Type type)
        {
            var allInterfaces = type.GetInterfaces();
            allInterfaces = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).ToArray();

            if (type.BaseType != null)
            {
                allInterfaces = allInterfaces.Except(type.BaseType.GetInterfaces()).ToArray();
            }
            return allInterfaces;
        }
    }
}