using System.Reflection;

namespace Rikrop.Core.Wcf.Security.Server
{
    public interface IAuthStrategy
    {
        bool CheckAccess(MethodInfo methodInfo);
    }
}