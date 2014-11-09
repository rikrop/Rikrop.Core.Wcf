using System;
using System.Linq;

namespace Rikrop.Core.Wcf
{
    internal static class ContractNameBuilder
    {
        public static string BuildContractName(Type type)
        {
            string contractName = type.Name;
            if (type.IsGenericType)
            {
                contractName += "Of";
                contractName += string.Join("Of", type.GetGenericArguments().Select(o => o.Name));
            }

            return contractName;
        }
    }
}