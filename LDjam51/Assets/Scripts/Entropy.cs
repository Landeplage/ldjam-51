using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entropy
{
    public long seed;

    public Entropy() {
        seed = 0;
    }

    public Entropy(Entropy copy) {
        seed = copy.seed;
    }

    public float Next()
    {
        while (seed < 0) {
            seed += 12345;
        }
        seed = (seed * 1103515245 + 12345) % 2147483648;
        return seed / 2147483648.1f;
    }

    public void Apply(int n)
    {
        seed += n * 12345;
    }
}
