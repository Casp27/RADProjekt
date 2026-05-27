using System;
using System.Collections.Generic;
using System.Diagnostics;

//Node
class Node
{
    public ulong Key;
    public long Value;
    public Node Next;

    public Node(ulong key, long value)
    {
        Key = key;
        Value = value;
        Next = null;
    }
}

//Hashtable med chaining
class HashTable
{
    private Node[] buckets;
    private Func<ulong, ulong> hashFunc;

    public HashTable(int l, Func<ulong, ulong> hashFunc)
    {
        this.hashFunc = hashFunc;
        this.buckets = new Node[1 << l];
    }

    // (a) get(x)
    // returner værdi for x, eller 0 hvis ikke findes
    public long Get(ulong x)
    {
        int idx = (int)hashFunc(x);
        Node current = buckets[idx];
        while (current != null)
        {
            if (current.Key == x) return current.Value;
            current = current.Next;
        }
        return 0;
    }

    // (b) set(x, v)
    // sæt x's værdi til v
    public void Set(ulong x, long v)
    {
        int idx = (int)hashFunc(x);
        Node current = buckets[idx];
        while (current != null)
        {
            if (current.Key == x) { current.Value = v; return; }
            current = current.Next;
        }
        Node newNode = new Node(x, v);
        newNode.Next = buckets[idx];
        buckets[idx] = newNode;
    }

    // (c) increment(x, d) 
    // læg d til x's værdi
    public void Increment(ulong x, long d)
    {
        int idx = (int)hashFunc(x);
        Node current = buckets[idx];
        while (current != null)
        {
            if (current.Key == x) { current.Value += d; return; }
            current = current.Next;
        }
        Node newNode = new Node(x, d);
        newNode.Next = buckets[idx];
        buckets[idx] = newNode;
    }

    // Opgave 3 ComputeS 
    // beregn S = sum af s(x)^2
    public long ComputeS()
    {
        long S = 0;
        for (int i = 0; i < buckets.Length; i++)
        {
            Node current = buckets[i];
            while (current != null)
            {
                S += current.Value * current.Value;
                current = current.Next;
            }
        }
        return S;
    }
}

//Opgave 2 og 3
class Opgave2og3
{
    // Samme som i opgave 1
    struct U89
    {
        public ulong high;
        public ulong low;
    }

    static ulong a = BuildUlong(new byte[] {
        0b10011100, 0b00010001, 0b11101110, 0b11000110,
        0b00010011, 0b01100001, 0b10100011, 0b11110111
    });

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

    static ulong BuildUlong(byte[] b)
    {
        ulong x = 0;
        for (int i = 0; i < b.Length; i++)
            x = (x << 8) | b[i];
        return x;
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

    static IEnumerable<Tuple<ulong, int>> CreateStream(int n, int l)
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

    static void Main()
    {
        int n = 10_000_000;

        //Manuel test 
        Console.WriteLine("--- TEST ---");
        var testTable = new HashTable(4, x => (a * x) >> (64 - 4));
        testTable.Increment(42, 1);
        testTable.Increment(42, 3);
        testTable.Increment(42, -1); // s(42) = 3
        testTable.Increment(99, 2);  // s(99) = 2
        Console.WriteLine("s(42) = " + testTable.Get(42)); // skal give 3
        Console.WriteLine("s(99) = " + testTable.Get(99)); // skal give 2
        Console.WriteLine("s(77) = " + testTable.Get(77)); // skal give 0
        Console.WriteLine("S = " + testTable.ComputeS());  // skal give 13

        // Opgave 3 eksperimenter
        Console.WriteLine($"\n{"l",-5} {"MS tid (ms)",-20} {"MMP tid (ms)",-20} {"S"}");
        Console.WriteLine(new string('-', 60));

        for (int testL = 1; testL <= 25; testL++)
        {
            //Multiply-Shift
            var msTable = new HashTable(testL, x => (a * x) >> (64 - testL));
            var sw = Stopwatch.StartNew();
            foreach (var p in CreateStream(n, testL))
                msTable.Increment(p.Item1, p.Item2);
            long S_ms = msTable.ComputeS();
            sw.Stop();
            long msTime = sw.ElapsedMilliseconds;

            //Multiply-Mod-Prime
            var mmpTable = new HashTable(testL, x => {
                U89 r = MultiplyAdd(a_mmp, x, b_mmp);
                return r.low & ((1UL << testL) - 1);
            });
            sw.Restart();
            foreach (var p in CreateStream(n, testL))
                mmpTable.Increment(p.Item1, p.Item2);
            long S_mmp = mmpTable.ComputeS();
            sw.Stop();
            long mmpTime = sw.ElapsedMilliseconds;

            Console.WriteLine($"{testL,-5} {msTime,-20} {mmpTime,-20} {S_ms}");

            if (msTime > 5000 || mmpTime > 5000)
            {
                Console.WriteLine($"Stoppede ved l={testL}");
                break;
            }
        }
    }
}