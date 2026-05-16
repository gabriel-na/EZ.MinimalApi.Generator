using System;
using Microsoft.AspNetCore.Authorization;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Add authorization requirements to an endpoint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizationAttribute : Attribute
    {
        public string[] PolicyNames { get; }
        public IAuthorizeData[] AuthorizeData { get; set; }
        public AuthorizationPolicy AuthorizationPolicy { get; set; }

        public AuthorizationAttribute(string policyName)
        {
            PolicyNames = new[] { policyName ?? throw new ArgumentNullException(nameof(policyName)) };
        }

        public AuthorizationAttribute(params string[] policyNames)
        {
            PolicyNames = policyNames ?? throw new ArgumentNullException(nameof(policyNames));
        }

        public AuthorizationAttribute(IAuthorizeData[] authorizeData)
        {
            AuthorizeData = authorizeData ?? throw new ArgumentNullException(nameof(authorizeData));
        }
        
        public AuthorizationAttribute(AuthorizationPolicy authorizationPolicy)
        {
            AuthorizationPolicy = authorizationPolicy ?? throw new ArgumentNullException(nameof(authorizationPolicy));
        }
    }
}
