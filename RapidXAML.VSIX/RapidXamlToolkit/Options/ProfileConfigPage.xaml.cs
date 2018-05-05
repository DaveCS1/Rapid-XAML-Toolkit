﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

namespace RapidXamlToolkit
{
    public partial class ProfileConfigPage : DialogWindow
    {
        private Profile viewModel;

        public ProfileConfigPage()
        {
            this.InitializeComponent();
        }

        public void SetDataContext(Profile profile)
        {
            this.viewModel = profile;
            this.DataContext = this.viewModel;
        }

        private void OkClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.viewModel.Mappings.Add(Mapping.CreateNew());
                this.viewModel.RefreshMappings();
            }
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        private void CopyClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.DisplayedMappings.SelectedIndex >= 0)
                {
                    var copy = (Mapping)this.viewModel.Mappings[this.DisplayedMappings.SelectedIndex].Clone();

                    this.viewModel.Mappings.Add(copy);
                    this.viewModel.RefreshMappings();
                }
            }
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }

        private void DeleteClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.DisplayedMappings.SelectedIndex >= 0)
                {
                    this.viewModel.Mappings.RemoveAt(this.DisplayedMappings.SelectedIndex);
                    this.viewModel.RefreshMappings();
                }
            }
            catch (Exception exc)
            {
                new RxtLogger().RecordException(exc);
                throw;
            }
        }
    }
}
