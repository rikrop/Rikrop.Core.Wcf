using System;
using System.Diagnostics.Contracts;

namespace Rikrop.Core.Wcf.Security
{
    public class SessionHeaderInfo : ISessionHeaderInfo
    {
        private readonly string _headerNamespace;
        private readonly string _headerName;

        public string Namespace
        {
            get { return _headerNamespace; }
        }

        public string Name
        {
            get { return _headerName; }
        }

        public SessionHeaderInfo(string headerNamespace, string headerName)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(headerNamespace));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(headerName));

            _headerNamespace = headerNamespace;
            _headerName = headerName;
        }
    }
}