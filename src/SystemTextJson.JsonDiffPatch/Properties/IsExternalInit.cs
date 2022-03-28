// https://stackoverflow.com/questions/64749385/predefined-type-system-runtime-compilerservices-isexternalinit-is-not-defined
// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives
#if !NET
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}
#endif