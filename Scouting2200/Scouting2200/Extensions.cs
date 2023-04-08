using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Scouting2200
{
    public static class Extensions
    {
        public static V GetOrDefault<K, V>(this Dictionary<K, V> dict, K key, V def)
        {
            if (dict.TryGetValue(key, out V found))
            {
                return found;
            }
            return def;
        }
        public static bool Overflows(this int num, int addend) => addend > (int.MaxValue - num);
        public static Zipper<T, int> Enumerate<T>(this IEnumerator<T> iter) => new Zipper<T, int>(iter, new Step());
        public static Zipper<T, int> Enumerate<T>(this IEnumerable<T> iter) => new Zipper<T, int>(iter.GetEnumerator(), new Step());
        public static Enumewrapper<T> Wrap<T>(this IEnumerator<T> iter) => new Enumewrapper<T>(iter);
        public static SingleViewAdder Adder(this View view) => new SingleViewAdder(view);
        public static MultiViewAdder With(this View view, params View[] other) => new MultiViewAdder(Enumerable.Concat(new One<View>(view).Wrap(), other));
        public static List<View> Collect(this IViewAdder vadder)
        {
            List<View> views = new List<View>();
            vadder.AddTo(views);
            return views;
        }
        public static MultiBuilder With(this ILayoutBuilder builder, params ILayoutBuilder[] other) => new MultiBuilder(Enumerable.Concat(new One<ILayoutBuilder>(builder).Wrap(), other));
        public static Color Times(this Color l, double r) => new Color(
            l.R * r,
            l.G * r,
            l.B * r
        );
        public static Color Minus(this Color l, Color r) => new Color(
            l.R - r.R,
            l.G - r.G,
            l.B - r.B
        );
        public static Color Plus(this Color l, Color r) => new Color(
            l.R + r.R,
            l.G + r.G,
            l.B + r.B
        );
        public static int Negativent(this int i) => i < 0 ? 0 : i;
        public static int Clamp(this int i, int min, int max) => i < min ? min : i > max ? max : i;
        public static bool WithinII(this int i, int low, int hi) => i >= low && i <= hi;
        public static bool WithinXX(this int i, int low, int hi) => i > low && i < hi;
        public static bool WithinIX(this int i, int low, int hi) => i >= low && i < hi;
        public static bool WithinXI(this int i, int low, int hi) => i > low && i <= hi;
    }
}
