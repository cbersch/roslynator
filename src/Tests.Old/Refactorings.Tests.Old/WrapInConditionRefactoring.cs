﻿// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class WrapInConditionRefactoring
    {
        public void Foo()
        {
            string s = null;

            switch (0)
            {
                case 0:
                    string s2 = null;
                    break;
            }

            if (true)
                s = null;
        }
    }
}
