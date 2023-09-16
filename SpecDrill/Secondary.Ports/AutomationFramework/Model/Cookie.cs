using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Secondary.Ports.AutomationFramework.Model
{
    public class Cookie
    {
        public interface ICookieFactory<T>{
            public T Create(string name, string value);
            public T Create(string name, string value, string path);

            public T Create(string name, string value, string path, DateTime? expiry);
            public T Create(string name, string value, string domain, string path, DateTime? expiry);
            public T Create(string name, string value, string domain, string path, DateTime? expiry, bool isSecure, bool isHttpOnly, string sameSite);
        }
        private enum ConstructorType {
            name_value,
            name_value_path,
            name_value_path_expiry,
            name_value_domain_path_expiry,
            name_value_domain_path_expiry_issecure_ishttponly_samesite
        }
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string? Path { get; private set; }
        public DateTime? Expiry { get; private set; }
        public string? Domain { get; private set; }
        public bool? IsSecure { get; private set; }
        public bool? IsHttpOnly { get; private set; }
        public string? SameSite { get; private set; }

        private readonly ConstructorType constructorType;
        public Cookie(string name, string value)
            => (constructorType, Name, Value) = (ConstructorType.name_value, name, value);

        public Cookie(string name, string value, string path) : this(name, value)
            => (constructorType, Path) = (ConstructorType.name_value_path, Path);

        public Cookie(string name, string value, string path, DateTime? expiry) : this(name, value, path)
            => (constructorType, Expiry) = (ConstructorType.name_value_path_expiry, expiry);

        public Cookie(string name, string value, string domain, string path, DateTime? expiry) : this(name, value, path, expiry)
            => (constructorType, Domain) = (ConstructorType.name_value_domain_path_expiry, domain);

        public Cookie(string name, string value, string domain, string path, DateTime? expiry, bool isSecure, bool isHttpOnly, string sameSite) : this(name, value, domain, path, expiry)
            => (constructorType, IsSecure, IsHttpOnly, SameSite) = (ConstructorType.name_value_domain_path_expiry_issecure_ishttponly_samesite, isSecure, isHttpOnly, sameSite);

        public T Create<T>(ICookieFactory<T> factory)
            => constructorType switch
            {
                ConstructorType.name_value => factory.Create(Name, Value),
                ConstructorType.name_value_path => factory.Create(Name, Value, Path!),
                ConstructorType.name_value_path_expiry => factory.Create(Name, Value, Path!, Expiry!),
                ConstructorType.name_value_domain_path_expiry => factory.Create(Name, Value, Domain!, Path!, Expiry!),
                ConstructorType.name_value_domain_path_expiry_issecure_ishttponly_samesite => factory.Create(Name, Value, Domain!, Path!, Expiry!, IsSecure ?? false, IsHttpOnly ?? false, SameSite!),
                _ => throw new NotImplementedException("case {} not yet implemented. Please report an issue on framework's github.com repository!")
            };
    }
}
