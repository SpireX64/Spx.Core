using System;

namespace Spx.Collections.UnitTests.Utils
{
    public static class TestUtils
    {
        public static void RunIt(Action action) => action.Invoke();
    }
}