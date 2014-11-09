namespace Rikrop.Core.Wcf.Security
{
    public interface ISessionHeaderInfo
    {
        string Namespace { get; }
        string Name { get; }
    }
}