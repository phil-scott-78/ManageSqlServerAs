using System;

namespace ManageSqlServerAs.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class PromptCredentialsResult : IPromptCredentialsResult
    {
        public String UserName { get; internal set; }
        public String DomainName { get; internal set; }
        public String Password { get; internal set; }
        public Boolean IsSaveChecked { get; set; }
    }
}