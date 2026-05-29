using System;
using System.Collections.Generic;
using System.Diagnostics;

class Opgave1
{
    static int l = 20;

    // (a) Multiply-Shift
    static ulong a = BuildUlong(new byte[] {
        0b10011100,
        0b00010001,
        0b11101110,
        0b11000110,
        0b00010011,
        0b01100001,
        0b10100011,
        0b11110111
    });

    static ulong MultiplyShift(ulong x)
    {
        return (a * x) >> (64 - l);
    }

    // (b) Multiply-Mod-Prime
   
    struct U89
    {
        public ulong high; // 25 bit
        public ulong low;  // 64 bit
    }

    static U89 BuildU89(byte[] b, bool extraBit)
    {
        ulong high = 0;
        ulong low = 0;

        // 24 bit
        for (int i = 0; i < 3; i++)
            high = (high << 8) | b[i];

        // +1 bit = 25 bit total
        if (extraBit)
            high |= (1UL << 24);

        // 64 bit
        for (int i = 3; i < 11; i++)
            low = (low << 8) | b[i];

        return new U89 { high = high, low = low };
    }

    // 89-bit konstanter (her uden ekstra bit)
    static U89 a_mmp = BuildU89(new byte[] {
        0b00110111,0b11011010,0b10011001,
        0b00111011,0b00100110,0b10111110,
        0b01011011,0b10111110,0b01011001,
        0b00110111,0b00001001
    }, false);

    static U89 b_mmp = BuildU89(new byte[] {
        0b11100110,0b11111111,0b10000110,
        0b00101011,0b11010010,0b10101000,
        0b10011011,0b01010100,0b00011001,
        0b11100110,0b00001101
    }, false);

    static U89 ModP(U89 x)
    {
        ulong overflow = x.high >> 25;
        x.high &= (1UL << 25) - 1;

        x.low += overflow;

        if (x.low < overflow)
            x.high++;

        return x;
    }

    static U89 MultiplyAdd(U89 a, ulong x, U89 b)
    {
        ulong low = a.low * x;
        ulong high = a.high * x;

        U89 res = new U89 { high = high, low = low };

        res.low += b.low;
        if (res.low < b.low)
            res.high++;

        res.high += b.high;

        return ModP(res);
    }

    static ulong MultiplyModPrime(ulong x)
    {
        U89 r = MultiplyAdd(a_mmp, x, b_mmp);
        return r.low & ((1UL << l) - 1);
    }


    // (c) TEST
    
    public static IEnumerable<Tuple<ulong, int>> CreateStream(int n, int l)
    {
        Random rnd = new Random();
        ulong a = 0UL;
        byte[] b = new byte[8];
        rnd.NextBytes(b);

        for (int i = 0; i < 8; ++i)
            a = (a << 8) + (ulong)b[i];

        a = (a & ((1UL << 31) - 1UL)) ^ ((1UL << 30) - 1UL);

        ulong x = 0UL;

        for (int i = 0; i < n / 3; ++i)
        {
            x = x + a;
            yield return Tuple.Create(x & (((1UL << l) - 1UL) << 30), 1);
        }

        for (int i = 0; i < (n + 1) / 3; ++i)
        {
            x = x + a;
            yield return Tuple.Create(x & (((1UL << l) - 1UL) << 30), -1);
        }

        for (int i = 0; i < (n + 2) / 3; ++i)
        {
            x = x + a;
            yield return Tuple.Create(x & (((1UL << l) - 1UL) << 30), 1);
        }
    }



    static void Test(int n)
    {
        ulong sum1 = 0;
        ulong sum2 = 0;

        var sw = Stopwatch.StartNew();

        foreach (var p in CreateStream(n, l))
            sum1 += MultiplyShift(p.Item1);

        sw.Stop();
        Console.WriteLine("Multiply-Shift tid: " + sw.ElapsedMilliseconds + " ms");

        sw.Restart();

        foreach (var p in CreateStream(n, l))
            sum2 += MultiplyModPrime(p.Item1);

        sw.Stop();
        Console.WriteLine("Multiply-Mod-Prime tid: " + sw.ElapsedMilliseconds + " ms");

        Console.WriteLine(sum1);
        Console.WriteLine(sum2);
    }

    public static void Run()
    {
        Test(10_000_000);
    }

    // -----------------------------
    // helper
    // -----------------------------
    static ulong BuildUlong(byte[] b)
    {
        ulong x = 0;
        for (int i = 0; i < b.Length; i++)
            x = (x << 8) | b[i];
        return x;
    }

}