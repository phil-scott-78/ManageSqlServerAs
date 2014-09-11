using System;
using System.Security;

namespace ManageSqlServerAs.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class PromptCredentialsSecureStringResult : IPromptCredentialsResult
    {
        public SecureString UserName { get; internal set; }
        public SecureString DomainName { get; internal set; }
        public SecureString Password { get; internal set; }
        public Boolean IsSaveChecked { get; set; }
    }
}