using Windows.UI.Xaml;
using System.Threading.Tasks;
using VOLSBB.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.VoiceCommands;
using Microsoft.Services.Store.Engagement;
using Windows.Foundation.Collections;
using VOLSBB.Views;


namespace VOLSBB
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper
    {
        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);
                       

            //    #region Hockeyapp Integration
            //    HockeyClient.Current.Configure("fe89ea0653fd4e2fa99fc385eff8cd0e");
            //    HockeyClient.Current.Configure("fe89ea0653fd4e2fa99fc385eff8cd0e")
            //  .SetExceptionDescriptionLoader((Exception ex) =>
            // {
            //     return "Exception HResult: " + ex.HResult.ToString();
            // });
            //    Microsoft.HockeyApp.HockeyClient.Current.Configure("fe89ea0653fd4e2fa99fc385eff8cd0e",
            //new Microsoft.HockeyApp.TelemetryConfiguration()
            //{
            //    Collectors = WindowsCollectors.Metadata | WindowsCollectors.Session | WindowsCollectors.UnhandledException
            //}); 
            //    #endregion

            #region app settings

            // some settings must be set in app.constructor
            var settings = SettingsService.Instance;
            //RequestedTheme = 
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;
            AutoSuspendAllFrames = true;
            AutoRestoreAfterTerminated = true;
            AutoExtendExecutionSession = true;

            #endregion
        }

        public override UIElement CreateRootElement(IActivatedEventArgs e)
        {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }


        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
           // await Task.Delay(4);
            try
            {
                StorageFile vcdStorageFile = await Package.Current.InstalledLocation.GetFileAsync(@"HomeControlCommands.xml");
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
            }
            catch 
            {
              //  System.Diagnostics.Debug.WriteLine("There was an error registering the Voice Command Definitions", ex);
            }
            StoreServicesEngagementManager engagementManager = StoreServicesEngagementManager.GetDefault();
            await engagementManager.RegisterNotificationChannelAsync();

            // TODO: add your long-running task here

         //    await Windows.Storage.ApplicationData.Current.ClearAsync();

            IPropertySet roamingProperties = ApplicationData.Current.RoamingSettings.Values;
            if (roamingProperties.ContainsKey("HasBeenHereBefore"))
            {
                // The normal case
                await NavigationService.NavigateAsync(typeof(Views.MainPage));
            }
            else
            {
                // The first-time case
                Shell.HamburgerMenu.IsFullScreen = true;
                await NavigationService.NavigateAsync(typeof(Views.HelpPage));
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                 localSettings.Values["oneshot"] = "true";
                roamingProperties["HasBeenHereBefore"] = bool.TrueString; // Doesn't really matter what
            }

          
          
        }
    }
}

