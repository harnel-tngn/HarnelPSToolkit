namespace HarnelPSToolkit.Library;

[IncludeIfReferenced]
public static class DeconstructHelper
{
    public static void Deconstruct<T>(this T[] arr, out T v1, out T v2)
        => (v1, v2) = (arr[0], arr[1]);

    public static void Deconstruct<T>(this T[] arr, out T v1, out T v2, out T v3)
        => (v1, v2, v3) = (arr[0], arr[1], arr[2]);

    public static void Deconstruct<T>(this T[] arr, out T v1, out T v2, out T v3, out T v4)
        => (v1, v2, v3, v4) = (arr[0], arr[1], arr[2], arr[3]);

    public static void Deconstruct<T>(this T[] arr, out T v1, out T v2, out T v3, out T v4, out T v5)
        => (v1, v2, v3, v4, v5) = (arr[0], arr[1], arr[2], arr[3], arr[4]);

    public static void Deconstruct<T>(this T[] arr, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6)
        => (v1, v2, v3, v4, v5, v6) = (arr[0], arr[1], arr[2], arr[3], arr[4], arr[5]);

    public static void Deconstruct<T>(this T[] arr, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6, out T v7)
        => (v1, v2, v3, v4, v5, v6, v7) = (arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], arr[6]);

    public static void Deconstruct<T>(this T[] arr, out T v1, out T v2, out T v3, out T v4, out T v5, out T v6, out T v7, out T v8)
        => (v1, v2, v3, v4, v5, v6, v7, v8) = (arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], arr[6], arr[7]);
}
