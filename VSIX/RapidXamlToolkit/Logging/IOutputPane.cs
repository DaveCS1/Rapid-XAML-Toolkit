﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace RapidXamlToolkit.Logging
{
    public interface IOutputPane
    {
        void Write(string message);

        void Activate();
    }
}
