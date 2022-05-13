namespace System.Text.Json.JsonDiffPatch
{
    static partial class JsonValueComparer
    {
        private static bool AreDoubleClose(double x, double y)
        {
            // https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
            const double epsilon = 2.22044604925031E-16;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (x == y)
                return true;

            var tolerance = (Math.Abs(x) + Math.Abs(y) + 10.0) * epsilon;
            var difference = x - y;
            
            if (-tolerance < difference)
                return tolerance > difference;

            return false;
        }

        internal static int CompareDouble(double x, double y)
        {
            return AreDoubleClose(x, y) ? 0 : x.CompareTo(y);
        }
    }
}

