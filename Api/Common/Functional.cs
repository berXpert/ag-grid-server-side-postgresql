namespace Api.Common;

public static class Functional
{
    public static Func<T1, Func<T2, T3>> Curry<T1, T2, T3>(
     this Func<T1, T2, T3> @this) =>
         (T1 x) => (T2 y) => @this(x, y);

    public static Func<T1, Func<T2, Func<T3, T4>>> Curry<T1, T2, T3, T4>(
     this Func<T1, T2, T3, T4> @this) =>
         (T1 x) => (T2 y) => (T3 z) => @this(x, y, z);

    public static Func<T1, Func<T2, Func<T3, Func<T4, T5>>>> Curry<T1, T2, T3, T4, T5>(
     this Func<T1, T2, T3, T4, T5> @this) =>
         (T1 x) => (T2 y) => (T3 z) => (T4 a) => @this(x, y, z, a);


    // 4 parameters to 1
    public static Func<T4, TOut> Partial<T1, T2, T3, T4, TOut>(

     this Func<T1, T2, T3, T4, TOut> f,
     T1 one, T2 two, T3 three) => (T4 four) => f(one, two, three, four);

    // 4 parameters to 2
    public static Func<T3, T4, TOut> Partial<T1, T2, T3, T4, TOut>(

     this Func<T1, T2, T3, T4, TOut> f,
     T1 one, T2 two) => (T3 three, T4 four) => f(one, two, three, four);

    // 2 parameters to 1
    public static Func<T2, TOut> Partial<T1, T2, TOut>(
     this Func<T1, T2, TOut> f, T1 one) =>
      (T2 two) => f(one, two);
}