﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.Options;
using RapidXamlToolkit.Parsers;

namespace RapidXamlToolkit.Tests.Parsers
{
    [TestClass]
    public class GetCSharpStructsTests : CSharpTestsBase
    {
        [TestMethod]
        public void GetStructAllPropertyOptions()
        {
            var code = @"
namespace tests
{
    struct Str☆uctViewModel
    {
        public string Property1 { get; set; }
        public string Property2 { get; private set; }
    }
}";

            var expectedOutput = "<StackPanel>"
         + Environment.NewLine + "    <TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "    <TextBlock Text=\"Property2\" />"
         + Environment.NewLine + "</StackPanel>";

            var expected = new ParserOutput
            {
                Name = "StructViewModel",
                Output = expectedOutput,
                OutputType = ParserOutputType.Class,
            };

            this.PositionAtStarShouldProduceExpected(code, expected);
        }

        [TestMethod]
        public void GetStructProperty()
        {
            var profile = TestProfile.CreateEmpty();
            profile.Mappings.Add(new Mapping
            {
                Type = "MyStruct",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<MyStruct />",
            });

            var code = @"
namespace tests
{
    class Class1
    {
        ☆public MyStruct Property2 { get; set; }☆
    }

    struct MyStruct
    {
        public string MyProperty1 { get; set; }
        public int MyProperty2 { get; set; }
    }
}";

            var expected = new ParserOutput
            {
                Name = "Property2",
                Output = "<MyStruct />",
                OutputType = ParserOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetListOfStructProperty()
        {
            var profile = TestProfile.CreateEmpty();
            profile.SubPropertyOutput = "<$name$ />";
            profile.Mappings.Add(new Mapping
            {
                Type = "List<MyStruct>",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "$subprops$",
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "string",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<String Name=\"$name$\" />",
            });
            profile.Mappings.Add(new Mapping
            {
                Type = "int",
                IfReadOnly = false,
                NameContains = string.Empty,
                Output = "<Int Name=\"$name$\" />",
            });

            var code = @"
using System.Collections.Generic;

namespace tests
{
    class Class1
    {
        ☆public List<MyStruct> MyListProperty { get; set; }☆
    }

    struct MyStruct
    {
        public string MyProperty1 { get; set; }
        public int MyProperty2 { get; set; }
    }
}";

            var expectedXaml = "<MyProperty1 />"
       + Environment.NewLine + "<MyProperty2 />";

            var expected = new ParserOutput
            {
                Name = "MyListProperty",
                Output = expectedXaml,
                OutputType = ParserOutputType.Property,
            };

            this.EachPositionBetweenStarsShouldProduceExpected(code, expected, profile);
        }

        [TestMethod]
        public void GetSelectionOfStructProperties()
        {
            var code = @"
namespace tests
{
    struct StructViewModel
    {
       ☆ public string Property1 { get; set; }
        public string Property2 { get; private set; }☆
    }
}";

            var expectedOutput = "<TextBox Text=\"{x:Bind Property1, Mode=TwoWay}\" />"
         + Environment.NewLine + "<TextBlock Text=\"Property2\" />";

            var expected = new ParserOutput
            {
                Name = "Property1 and Property2",
                Output = expectedOutput,
                OutputType = ParserOutputType.Selection,
            };

            this.SelectionBetweenStarsShouldProduceExpected(code, expected);
        }
    }
}
