using System.Collections.Generic;

namespace AsmForThreadsStream
{
    static class Memory
    {
        private static readonly Dictionary<string, object> _ram = new Dictionary<string, object>();

        public static T Read<T>(string address)
        {
            return (T)_ram[address];
        }

        public static void Write<T>(T obj, string address)
        {
            _ram[address] = obj;
        }
    }
}
