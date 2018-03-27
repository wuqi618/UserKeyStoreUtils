using System;
using System.Security.Cryptography;
using System.Text;
using NDesk.Options;

namespace UserKeyStoreUtils
{
    class Program
    {
        private static OptionSet _options;

        private static Operation _operation;
        private static string _name;
        private static string _value;

        static void Main(string[] args)
        {
            _options = new OptionSet
            {
                {"h|?|help", "show help and exit", _ => ShowHelp()},
                {"save", "encrypt and save", _ => SetOperation(Operation.Save)},
                {"get", "encrypt and save", _ => SetOperation(Operation.Get)},
                {"name=", SetName},
                {"value=", SetValue}
            };

            try
            {
                _options.Parse(args);

                if (_operation == Operation.Save)
                {
                    Save(_name, _value);
                }
                else if (_operation == Operation.Get)
                {
                    Console.Write(Get(_name));
                }
                else
                {
                    _options.WriteOptionDescriptions(Console.Out);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid arguments");
                _options.WriteOptionDescriptions(Console.Out);
            }
        }

        private static void ShowHelp()
        {
            _options.WriteOptionDescriptions(Console.Out);
        }

        private static void SetOperation(Operation operation)
        {
            _operation = operation;
        }

        private static void SetName(string name)
        {
            _name = name;
        }

        private static void SetValue(string value)
        {
            _value = value;
        }

        private static void Save(string name, string value)
        {
            if(string.IsNullOrEmpty(name)) return;

            var buffer = ProtectedData.Protect(Encoding.UTF8.GetBytes(value), null, DataProtectionScope.CurrentUser);
            
            Environment.SetEnvironmentVariable(name, Convert.ToBase64String(buffer), EnvironmentVariableTarget.User);
        }

        private static string Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);

            if (value == null) return null;

            var buffer = ProtectedData.Unprotect(Convert.FromBase64String(value), null, DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(buffer);
        }
    }

    public enum Operation
    {
        None,
        Save,
        Get
    }
}
