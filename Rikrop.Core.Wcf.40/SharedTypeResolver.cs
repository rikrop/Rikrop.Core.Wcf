using System;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System.Xml;

namespace Rikrop.Core.Wcf
{
    public class SharedTypeResolver : DataContractResolver
    {
        public static void AddToContract(ContractDescription contract)
        {
            foreach (var operation in contract.Operations)
            {
                var serializerBehavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (serializerBehavior == null)
                {
                    serializerBehavior = new DataContractSerializerOperationBehavior(operation);
                    operation.Behaviors.Add(serializerBehavior);
                }

                serializerBehavior.DataContractResolver = new SharedTypeResolver();
                serializerBehavior.MaxItemsInObjectGraph = int.MaxValue;
            }
        }

        public override bool TryResolveType(Type dataContractType, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if (!knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(dataContractType.FullName);
                typeNamespace = dictionary.Add(dataContractType.Assembly.FullName);
            }
            return true;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            var type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            if (type == null)
            {
                type = Type.GetType(typeName + ", " + typeNamespace);
            }
            return type;
        }
        
    }
}