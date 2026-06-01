using System;
using System.Collections.Generic;

class Opgave7
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

    static (U89 a0, U89 a1, U89 a2, U89 a3) BuildRandomValues(Random rnd)
    {
        byte[] b0 = new byte[11];
        byte[] b1 = new byte[11];
        byte[] b2 = new byte[11];
        byte[] b3 = new byte[11];

        rnd.NextBytes(b0);
        rnd.NextBytes(b1);
        rnd.NextBytes(b2);
        rnd.NextBytes(b3);

        return (
            BuildU89(b0, false),
            BuildU89(b1, false),
            BuildU89(b2, false),
            BuildU89(b3, false)
        );
    }

    static int t = 10;
    static ulong mask = (1UL << t) - 1;

    static ulong MeanSquaredError(List<long> Xs, long S)
    {
        ulong sum = 0;

        foreach (long X in Xs)
        {
            sum += (ulong)(X - S) * (ulong)(X - S);
        }

        return sum / 100;
    }

    static double Variance(List<long> Xs)
    {
        double avg = Xs.Average();
        double res = Xs.Sum(x => (x - avg) * (x - avg)) / (double) Xs.Count();

        return Math.Sqrt(res);
    }
    
    public static void Run()
    {
        int experiments = 100;
        int baseSeed = 420;
        int n = 1_000_000;
        int l = 25;

        Console.WriteLine("--- Running Opgave 7 ---");

        List<long> Xs = new List<long>();

        var stream = Stream.CreateStream(n, l);
        
        for (int i = 0; i < experiments; i++)
        {
            Random rnd = new Random(baseSeed + i);

            var (a0, a1, a2, a3) = BuildRandomValues(rnd);

            U89 G(ulong x)
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

            ulong H(ulong x) => G(x).low & mask;

            int S(ulong x) => ((G(x).high >> 24) & 1) == 1 ? -1 : 1;

            long[] C = new long[1 << t];

            void Update(ulong x, long d) => C[H(x)] += S(x) * d;

            long SumSquares()
            {
                long X = 0;
                foreach (long c in C)
                {
                    X += c * c;
                }
                return X;
            }
            foreach(var (value, sign) in stream)
            {
                Update(value, sign);
            }
            Xs.Add(SumSquares());
        }
        
        long s = 1115006;

        // Visualisations

        // Median Results
        List<List<long>> g = new List<List<long>>();
        List<long> m = new List<long>();
        for (int i = 0; i < 9; i++)
        {
            g.Add(new List<long>());
            for (int j = 0; j < 11; j++)
            {
                g[i].Add(Xs[11 * i + j]);
            }
            g[i].Sort();
            m.Add(g[i][5]);
        } 

        m.Sort();

        var plt = new ScottPlot.Plot();
        var index = Enumerable.Range(1, 9).Select(x => (double) x).ToArray();
        plt.Add.Scatter(index, m.ToArray());
        
        var line = plt.Add.HorizontalLine(s);
        line.Color = ScottPlot.Colors.Red;
        line.LineWidth = 2;
        line.LinePattern = ScottPlot.LinePattern.Dashed;

        plt.Title("Median Scatterplot");
        plt.XLabel("i");
        plt.YLabel("M[i]");

        plt.SavePng("median_scatterplot.png", 600, 400);
        Console.WriteLine("Saved to median_scatterplot.png");

        // Sorted Representation
        Xs.Sort();
        
        plt = new ScottPlot.Plot();
        index = Enumerable.Range(1, 100).Select(x => (double) x).ToArray();
        plt.Add.Scatter(index, Xs.ToArray());

        line = plt.Add.HorizontalLine(s);
        line.Color = ScottPlot.Colors.Red;
        line.LineWidth = 2;
        line.LinePattern = ScottPlot.LinePattern.Dashed;

        plt.Title("Scatterplot");
        plt.XLabel("i");
        plt.YLabel("X[i]");

        plt.SavePng("scatterplot.png", 600, 400);
        Console.WriteLine("Saved to scatterplot.png");

        Console.WriteLine($"Mean Squared Error: {MeanSquaredError(Xs, s)}");
        double avg = Xs.Average();

        Console.WriteLine($"E[X] = {Xs.Average()}");
        Console.WriteLine($"Var[X] = {Variance(Xs)}");
        Console.WriteLine($"2s^2/m = {2 * s * s /experiments}");
    }
}