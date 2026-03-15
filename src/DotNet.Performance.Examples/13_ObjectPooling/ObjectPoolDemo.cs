// ============================================================
// Concept  : Object Pooling
// Summary  : Reuses expensive objects via a thread-safe pool to eliminate repeated allocation
// When to use   : Objects that are expensive to construct, frequently needed, and easily reset
// When NOT to use: Objects that hold unmanaged resources or state that is hard to clear safely
// ============================================================

using System.Collections.Concurrent;
using System.Text;

namespace DotNet.Performance.Examples.ObjectPooling;

/// <summary>
/// A minimal, thread-safe generic object pool backed by a <see cref="ConcurrentBag{T}"/>.
/// Objects are created on demand via the supplied <c>factory</c> delegate and returned
/// to the bag for reuse when the caller is done.
/// </summary>
/// <typeparam name="T">The type of objects to pool.</typeparam>
public sealed class SimpleObjectPool<T>
{
    private readonly ConcurrentBag<T> _bag = new();
    private readonly Func<T> _factory;

    /// <summary>
    /// Initialises a new pool with the given object factory.
    /// </summary>
    /// <param name="factory">Factory delegate used to create new instances on demand.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is null.</exception>
    public SimpleObjectPool(Func<T> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _factory = factory;
    }

    /// <summary>
    /// Returns an object from the pool, or creates a new one if the pool is empty.
    /// </summary>
    /// <returns>A pooled or newly created instance of <typeparamref name="T"/>.</returns>
    public T Rent() => _bag.TryTake(out T? item) ? item : _factory();

    /// <summary>
    /// Returns <paramref name="obj"/> to the pool so it can be reused by future callers.
    /// </summary>
    /// <param name="obj">The object to return. Must not be used after this call.</param>
    public void Return(T obj) => _bag.Add(obj);
}

/// <summary>
/// Compares creating <see cref="StringBuilder"/> instances on-the-fly (GC pressure)
/// against renting them from a <see cref="SimpleObjectPool{T}"/> (minimal allocations).
/// </summary>
public sealed class ObjectPoolDemo
{
    private static readonly SimpleObjectPool<StringBuilder> _pool =
        new SimpleObjectPool<StringBuilder>(() => new StringBuilder());

    /// <summary>
    /// Creates a fresh <see cref="StringBuilder"/> for each of the <paramref name="count"/>
    /// formatting operations, accumulating the total character count.
    /// Every iteration allocates a new object, adding GC pressure.
    /// </summary>
    /// <param name="count">Number of formatting operations to perform.</param>
    /// <returns>Total number of characters produced across all operations.</returns>
    public static int ProcessWithNew(int count)
    {
        int totalChars = 0;
        for (int i = 0; i < count; i++)
        {
            var sb = new StringBuilder();
            sb.Append("Item").Append(i);
            totalChars += sb.Length;
        }
        return totalChars;
    }

    /// <summary>
    /// Rents a <see cref="StringBuilder"/> from the shared pool for each of the
    /// <paramref name="count"/> formatting operations, clears it after use, and returns it.
    /// Avoids repeated heap allocations by reusing the same instance(s).
    /// </summary>
    /// <param name="count">Number of formatting operations to perform.</param>
    /// <returns>Total number of characters produced across all operations.</returns>
    public static int ProcessWithPool(int count)
    {
        int totalChars = 0;
        for (int i = 0; i < count; i++)
        {
            StringBuilder sb = _pool.Rent();
            try
            {
                sb.Clear();
                sb.Append("Item").Append(i);
                totalChars += sb.Length;
            }
            finally
            {
                _pool.Return(sb);
            }
        }
        return totalChars;
    }
}
