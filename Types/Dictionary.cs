using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArmsFW.Core.Types
{
    /// <summary>
    /// Especialização da classe Dictionary com key e valor string (Dictionary<string, string>)
    /// </summary>
    public class StringList : Dictionary<string, string>
    {
        public StringList() :base()
        {
        }
        public StringList(string chave, string valor) => TryAdd(chave, valor);

        public StringList(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }

        public StringList(IEnumerable<KeyValuePair<string, string>> collection) : base(collection)
        {
        }

        public StringList(IEqualityComparer<string> comparer) : base(comparer)
        {
        }

        public StringList(int capacity) : base(capacity)
        {
        }


        public StringList(IDictionary<string, string> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer)
        {
        }

        public StringList(IEnumerable<KeyValuePair<string, string>> collection, IEqualityComparer<string> comparer) : base(collection, comparer)
        {
        }

        public StringList(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer)
        {
        }

        protected StringList(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
