using System;

namespace Rikrop.Core.Wcf.Security.Server
{
    /// <summary>
    /// ������.
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// ���� � ����� ���������� ������ ��� ������ ������.
        /// </summary>
        DateTime LastCall { get; set; }

        /// <summary>
        /// ������������� ������.
        /// </summary>
        string SessionId { get; }
    }
}