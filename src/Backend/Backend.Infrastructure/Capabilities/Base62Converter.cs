using Backend.Application.Capabilities;

namespace Backend.Infrastructure.Capabilities
{
    public class Base62Converter : IBase62Converter
    {
        private readonly string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private readonly long Divisor = 62;

        public string Execute(long request)
        {
            if(request < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(request), request, $"({request}) cannot be less than 0");
            }

            if(request == 0)
            {
                return Alphabet[0].ToString();
            }

            // Use stackalloc to allocate a fixed-size buffer on the stack
            // We use 11 because a 64 bit integer can be represented in base
            // 62 with a maximum of 11 characters (since 62^11 > long.MaxValue)
            Span<char> buffer = stackalloc char[11];
            int index = buffer.Length;
            long remaining = request;

            while(remaining > 0)
            {
                long remainder = remaining % Divisor;
                buffer[--index] = Alphabet[(int)remainder];
                remaining /= Divisor;
            }

            return new string(buffer[index..]);
        }
    }
}
