// Test script for Day 15
using System;

// Example from problem: should be 5
List<(long a, long m)> congruences = new List<(long a, long m)>();
congruences.Add((0, 5));   // t ≡ 0 (mod 5)
congruences.Add((1, 2));   // t ≡ 1 (mod 2)

long x = congruences[0].a;
long m = congruences[0].m;

Console.WriteLine($"Starting with: x={x}, m={m}");

for (int i = 1; i < congruences.Count; i++)
{
    long a2 = congruences[i].a;
    long m2 = congruences[i].m;
    
    Console.WriteLine($"\nMerging: {a2} (mod {m2})");
    Console.WriteLine($"  Current: {x} (mod {m})");
    
    long diff = ((a2 - x) % m2 + m2) % m2;
    Console.WriteLine($"  diff = {diff}");
    
    // Extended GCD
    long g = GCD(m, m2);
    Console.WriteLine($"  GCD({m}, {m2}) = {g}");
    
    if (diff % g != 0)
    {
        Console.WriteLine("  No solution!");
        break;
    }
    
    // For now just brute force to verify
    long newX = -1;
    for (long k = 0; k < m2; k++)
    {
        if ((x + m * k) % m2 == a2)
        {
            newX = x + m * k;
            Console.WriteLine($"  Found k={k}, x={newX}");
            break;
        }
    }
    
    if (newX >= 0)
    {
        x = newX;
        m = LCM(m, m2);
        x = ((x % m) + m) % m;
        Console.WriteLine($"  After merge: x={x}, m={m}");
    }
}

Console.WriteLine($"\nFinal answer: {x}");
Console.WriteLine($"Verification: {x} mod 5 = {x % 5}, {x} mod 2 = {x % 2}");

long GCD(long a, long b)
{
    while (b != 0)
    {
        long temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

long LCM(long a, long b)
{
    return a / GCD(a, b) * b;
}
