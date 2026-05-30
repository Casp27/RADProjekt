using System;

class Opgave6
{
    struct U89
    {
        public ulong high;
        public ulong low;
    }

    static U89 BuildU89(byte[] b, bool extraBit)
    {
        ulong high = 0;
        ulong low = 0;

        for (int i = 0; i < 3; i++)
            high = (high << 8) | b[i];

        if (extraBit)
            high |= (1UL << 24);

        for (int i = 3; i < 11; i++)
            low = (low << 8) | b[i];

        return new U89 { high = high, low = low };
    }

    static U89 ModP(U89 x)
    {
        ulong overflow = x.high >> 25;
        x.high &= (1UL << 25) - 1;
        x.low += overflow;

        if (x.low < overflow)
            x.high++;

        return x;
    }

    static U89 Add(U89 a, U89 b)
    {
        a.low += b.low;
        if (a.low < b.low) a.high++;
        a.high += b.high;
        return ModP(a);
    }

    static U89 MultiplyAdd(U89 a, ulong x, U89 b)
    {
        ulong low = a.low * x;
        ulong high = a.high * x;

        U89 res = new U89 { high = high, low = low };

        res.low += b.low;
        if (res.low < b.low) res.high++;
        res.high += b.high;

        return ModP(res);
    }

    static U89 Mul(U89 a, ulong x)
    {
        return MultiplyAdd(a, x, new U89 { high = 0, low = 0 });
    }

    static U89 a0 = BuildU89(new byte[] {
        0b10011010,0b00111110,0b00011110,
        0b00001011,0b01001110,0b11110110,
        0b01010010,0b10110100,0b01001001,
        0b10011101,0b00111110
    }, false);
 
    static U89 a1 = BuildU89(new byte[] {
        0b00111100,0b11011000,0b11000111,
        0b10001001,0b01111000,0b10001000,
        0b01111010,0b01111110,0b11111011,
        0b01000010,0b00100010
    }, false);
 
    static U89 a2 = BuildU89(new byte[] {
        0b11000000,0b01000111,0b01010110,
        0b11010110,0b01111011,0b11111111,
        0b11001010,0b00001010,0b11011001,
        0b10010010,0b11001110
    }, false);
 
    static U89 a3 = BuildU89(new byte[] {
        0b11001001,0b01111010,0b00000011,
        0b00111111,0b01101011,0b10111111,
        0b01100101,0b11111101,0b10101110,
        0b00100100,0b11100001
    }, false);

    static U89 G(ulong x)
    {
        U89 res = a0;
        ulong x1 = x;
        ulong x2 = x * x;
        ulong x3 = x2 * x;

        res = Add(res, Mul(a1, x1));
        res = Add(res, Mul(a2, x2));
        res = Add(res, Mul(a3, x3));

        return res;
    }

    static int t = 10;
    static ulong mask = (1UL << t) - 1;

    static ulong H(ulong x) => G(x).low & mask;

    static int S(ulong x) => ((G(x).high >> 24) & 1) == 1 ? -1 : 1;

    static long[] C = new long[1 << t];

    static void Update(ulong x, long d) => C[H(x)] += S(x) * d;

    static long SumSquares()
    {
        long X = 0;
        foreach (long c in C)
        {
            X += c * c;
        }
        return X;
    }


    public static void Run()
    {
        Console.WriteLine("=== Count Sketch (Opgave 6) ===");
        Update(3,5);
        Update(7,2);

        Console.WriteLine("Stream: Element 3 x5, Element 7 x2");
        Console.WriteLine($"True F2    = {5*5 + 2*2}");
        Console.WriteLine($"Estimate X = {SumSquares()}");
    }
}