using System;
using System.Collections.Generic;
using System.Text;

namespace System.Text.Json.JsonDiffPatch
{
    /// <summary>
    /// 
    /// </summary>
    public static class DeltaTypes
    {
        // https://github.com/benjamine/jsondiffpatch/blob/master/src/formatters/base.js#L191
        public const int Deleted = 0;
        public const int TextDiff = 2;
        public const int Moved = 3;
    }
}
