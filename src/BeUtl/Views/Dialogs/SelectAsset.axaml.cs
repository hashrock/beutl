﻿using Avalonia.Interactivity;
using Avalonia.Styling;

using Beutl.Api.Objects;

using BeUtl.ViewModels.Dialogs;

using FluentAvalonia.UI.Controls;

namespace BeUtl.Views.Dialogs;

public partial class SelectAsset : ContentDialog, IStyleable
{
    public SelectAsset()
    {
        InitializeComponent();
    }

    Type IStyleable.StyleKey => typeof(ContentDialog);

    private async void Add_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SelectAssetViewModel viewModel)
        {
            CreateAssetViewModel dialogViewModel = viewModel.CreateAssetViewModel();
            var dialog = new CreateAsset
            {
                DataContext = dialogViewModel,
            };

            Hide();
            await dialog.ShowAsync();

            if (dialogViewModel.Result is Asset asset)
            {
                await viewModel.Refresh.ExecuteAsync();
                SelectAssetViewModel.AssetViewModel? itemViewModel
                    = viewModel.Items.FirstOrDefault(x => x.Model.Id == asset.Id);

                if (itemViewModel != null)
                {
                    viewModel.SelectedItem.Value = itemViewModel;
                }
            }

            await ShowAsync();
        }
    }
}
