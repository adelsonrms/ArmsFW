using System;

namespace app.core.Domain.Entities
{
    internal class JsonTableAttribute : Attribute
    {

        public string Name => _Name;

        private readonly string _Name;
        public JsonTableAttribute(string name)
        {
            _Name = name;
        }
    }
}