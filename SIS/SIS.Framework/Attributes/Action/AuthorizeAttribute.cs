namespace SIS.Framework.Attributes.Action
{
    using System;
    using System.Linq;

    using Security.Contracts;

    public class AuthorizeAttribute : Attribute
    {
        private readonly string[] roles;

        public AuthorizeAttribute()
        {
            this.roles = new string[0];
        }

        public AuthorizeAttribute(params string[] roles)
        {
            this.roles = roles;
        }

        public bool IsAuthorized(IIdentity identity) =>
            this.roles == null || !this.roles.Any() ? this.IsIdentityPresent(identity) : this.IsIdentityInRole(identity);

        private bool IsIdentityPresent(IIdentity identity) => identity != null;

        private bool IsIdentityInRole(IIdentity identity) =>
            this.IsIdentityPresent(identity) && identity.Roles.Any(r => this.roles.Any(role => role == r));
    }
}
