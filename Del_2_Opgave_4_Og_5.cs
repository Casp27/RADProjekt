using System;

class Opgave4og5
{
    // p = 2^89 - 1. Vi bruger 25 + 64 bit
    struct U89
    {
        public ulong high; // 25 bit
        public ulong low;  // 64 bit
    }

    // Build 89-bit tal
    static U89 BuildU89(byte[] b, bool extraBit)
    {
        ulong high = 0;
        ulong low = 0;

        // første 24 bit
        for (int i = 0; i < 3; i++)
            high = (high << 8) | b[i];

        // ekstra bit (bit 24)
        if (extraBit)
            high |= (1UL << 24);

        // næste 64 bit
        for (int i = 3; i < 11; i++)
            low = (low << 8) | b[i];

        return new U89 { high = high, low = low };
    }

    // Mod p = 2^89 - 1
    static U89 ModP(U89 x)
    {
        ulong overflow = x.high >> 25;
        x.high &= (1UL << 25) - 1;

        x.low += overflow;

        if (x.low < overflow)
            x.high++;

        // 0 hvis x==p
        if (x.high == ((1UL << 25) - 1) && x.low == ulong.MaxValue)
        {
            x.high = 0;
            x.low = 0;
        }


        return x;
    }

    // Addition
    static U89 Add(U89 a, U89 b)
    {
        a.low += b.low;
        if (a.low < b.low) a.high++;
        a.high += b.high;
        return ModP(a);
    }

    // Multiplikation (a * x + b)
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

    // Multiplikation U89 * ulong
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

    // OPGAVE 4: g(x)
    static U89 G(ulong x)
    {
        U89 y = a3;

        // Algoritme 1
        y = MultiplyAdd(y, x, a2); // y = y*x + a2 mod p
        y = MultiplyAdd(y, x, a1); // y = y*x + a1 mod p
        y = MultiplyAdd(y, x, a0); // y = y*x + a0 mod p

        return y;
    }

    // OPGAVE 5: h og s
    static int t = 10;
    static ulong mask = (1UL << t) - 1;

    // h(x) = nederste t bits
    static ulong H(ulong x)
    {
        U89 gx = G(x);
        return gx.low & mask;
    }

    // s(x) = ±1 via MSB (bit 88)
    static int S(ulong x)
    {
        U89 gx = G(x);

        bool msb = ((gx.high >> 24) & 1) == 1;

        return msb ? -1 : 1;
    }

    // TEST
    public static void Run()
    {
        ulong x = 1000000;

        Console.WriteLine("g(x) low: " + G(x).low);
        Console.WriteLine("h(x): " + H(x));
        Console.WriteLine("s(x): " + S(x));
    }
}