using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace Rikrop.Core.Wcf.Security
{
    /// <summary>
    /// Класс-помощник для получения MethodInfo через Expression. Предполагается, что будет использоваться совместно с SessionAuthStrategy.
    /// </summary>
    public static class MethodInfoHelper
    {
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            return GetMethodInfo((LambdaExpression) expression);
        }

        private static MethodInfo GetMethodInfo(LambdaExpression expression)
        {
            Contract.Requires<ArgumentException>(expression.Body is MethodCallExpression);
            return ((MethodCallExpression) expression.Body).Method;
        }
    }
}