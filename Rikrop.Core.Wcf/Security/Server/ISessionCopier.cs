namespace Rikrop.Core.Wcf.Security.Server
{
    /// <summary>
    /// ������������� ������. ������������ ��� ����������� ������������� � <a href='InMemorySessionRepository' />.
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public interface ISessionCopier<TSession> where TSession : ISession
    {
        /// <summary>
        /// ���������� ������.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        TSession Copy(TSession session);
    }
}