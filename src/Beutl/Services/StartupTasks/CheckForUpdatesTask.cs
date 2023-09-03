﻿using Avalonia.Controls;
using Avalonia.Threading;

using Beutl.Api;

using FluentAvalonia.UI.Controls;

using Serilog;

namespace Beutl.Services.StartupTasks;

public sealed class CheckForUpdatesTask : StartupTask
{
    private readonly ILogger _logger = Log.ForContext<CheckForUpdatesTask>();
    private readonly BeutlApiApplication _beutlApiApplication;

    public CheckForUpdatesTask(BeutlApiApplication beutlApiApplication)
    {
        _beutlApiApplication = beutlApiApplication;
        Task = Task.Run(async () =>
        {
            CheckForUpdatesResponse? response = await CheckForUpdates();
            if (response != null)
            {
                if (!response.Is_latest)
                {
                    NotificationService.ShowInformation(
                        Message.A_new_version_is_available,
                        response.Url,
                        onActionButtonClick: () =>
                        {
                            Process.Start(new ProcessStartInfo(response.Url)
                            {
                                UseShellExecute = true,
                                Verb = "open"
                            });
                        },
                        actionButtonText: Strings.Open);
                }
                else if (response.Must_latest)
                {
                    await ShowDialogAndClose(response);
                }
            }
        });
    }

    public override Task Task { get; }

    private async ValueTask<CheckForUpdatesResponse?> CheckForUpdates()
    {
#pragma warning disable CS0436
        using (await _beutlApiApplication.Lock.LockAsync())
        {
            try
            {
                return await _beutlApiApplication.App.CheckForUpdatesAsync(GitVersionInformation.NuGetVersion);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while checking for updates");
                ex.Handle();
                return null;
            }
        }
#pragma warning restore CS0436
    }

    private static async Task ShowDialogAndClose(CheckForUpdatesResponse response)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var dialog = new ContentDialog
            {
                Title = Message.Must_upgrade_for_continued_use,
                Content = Message.This_version_has_been_discontinued_for_compatibility_reasonsversion,
                PrimaryButtonText = Strings.Yes,
                CloseButtonText = Strings.No,
            };

            await dialog.ShowAsync();

            Process.Start(new ProcessStartInfo(response.Url)
            {
                UseShellExecute = true,
                Verb = "open"
            });

            (App.GetTopLevel() as Window)?.Close();
        });
    }
}
