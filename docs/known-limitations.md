# Known Limitations

Due to the way the Rapid XAML Toolkit works there are some things it is not able to do. It may be useful to know this when evaluating it prior to use or when you think you may have found a bug.

## XAML Generation

- When run on source code that does not compile the XAML output is not guaranteed. The produced output may or may not match what you expect. If in doubt, get the code compiling before generating XAML. Even if used on code that does not compile the tool should not crash or throw an exception. If it does, please report this as a bug.
- Sub-properties are not type aware. When the mapping for a property type should output all the properties for a type (using the **$subprops$** placeholder) the underlying analyzer is not able to access the property's type and customize output accordingly. This is why it has a custom mapping.
- The Rapid XAML Toolkit is designed to generate XAML from source code. If your code references 3rd party libraries or precompiled assemblies then the information available to the internal analyzer is limited (compared to the compiler) and it may not correctly detect property types or accessibility. This may lead to properties not being included in the generated XAML or properties using the FallBack mapping even though a more specific mapping exists.
- There is currently no support for generic types that take multiple type parameters. If you need this, please raise an issue that explains your use case, what your VM looks like and what you expect the generated XAML to be.

## XAML Analysis

- XAML Analysis is only enabled when a .xaml file is opened as part of a project. If the document is opened on its own then full analysis is not possible. You may get incomplete, or no issue reported.
- Only open files are analyzed. (There is no 'solution-wide' analysis.)
- XAML Analysis only operates on a single file at a time. It is not possible to perform analysis that relates to more than one file at a time.
