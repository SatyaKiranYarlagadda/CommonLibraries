using System.Reflection;
using System.Threading.Tasks;

namespace Common.Foundation.Interceptors
{
    public static class InterceptorUtils
    {
        public static bool IsAsyncMethod(MethodInfo method)
        {
            return method.ReturnType == typeof(Task) ||
                   method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }
    }
}
