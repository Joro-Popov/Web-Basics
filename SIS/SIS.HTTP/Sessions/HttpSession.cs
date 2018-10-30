namespace SIS.HTTP.Sessions
{
    using System.Collections.Generic;

    using Contracts;

    public class HttpSession : IHttpSession
    {
        public string Id { get; }

        private readonly Dictionary<string, object> parameters;

        public HttpSession(string id)
        {
            this.Id = id;
            this.parameters = new Dictionary<string, object>();
        }

        public object GetParameter(string name)
        {
            return !this.ContainsParameter(name) ? null : this.parameters[name];
        }

        public bool ContainsParameter(string name)
        {
            return this.parameters.ContainsKey(name);
        }

        public void AddParameter(string name, object parameter)
        {
            this.parameters[name] = parameter;
        }

        public void ClearParameters()
        {
            this.parameters.Clear();
        }
    }
}
