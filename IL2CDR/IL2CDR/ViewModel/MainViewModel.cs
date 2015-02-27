﻿using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using IL2CDR.Model;
using IL2CDR.Properties;

namespace IL2CDR.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Config = Properties.Settings.Default.Config;
            var messages = (Application.Current as App).AppLogDataService.LogMessages;
            LogMessages = String.Join( Environment.NewLine, messages);
            messages.CollectionChanged += messages_CollectionChanged;
        }

        void messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UI.Dispatch(() => LogMessages += String.Join(Environment.NewLine, e.NewItems));
        }


        private RelayCommand _selectRootFolder;

        /// <summary>
        /// Gets the SelectRootFolder.
        /// </summary>
        public RelayCommand SelectRootFolder
        {
            get
            {
                return _selectRootFolder
                    ?? (_selectRootFolder = new RelayCommand(
                    () =>
                    {
                        Config.RootFolderDialog();
                    }));
            }
        }

        /// <summary>
        /// The <see cref="IsWindowReopen" /> property's name.
        /// </summary>
        public const string IsWindowReopenPropertyName = "IsWindowReopen";

        private bool _isWindowReopen = false;

        /// <summary>
        /// Sets and gets the IsWindowReopen property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWindowReopen
        {
            get
            {
                return _isWindowReopen;
            }

            set
            {
                if (_isWindowReopen == value)
                {
                    return;
                }

                _isWindowReopen = value;
                RaisePropertyChanged(IsWindowReopenPropertyName);
            }
        }
        private RelayCommand _exitApplication;

        /// <summary>
        /// Gets the ExitApplication.
        /// </summary>
        public RelayCommand ExitApplication
        {
            get
            {
                return _exitApplication
                    ?? (_exitApplication = new RelayCommand(
                    () =>
                    {
                        if (!IsWindowReopen)
                        {
                            Properties.Settings.Default.Save();
                            Application.Current.Shutdown();
                        }
                        IsWindowReopen = false;
                    }));
            }
        }

        /// <summary>
        /// The <see cref="Config" /> property's name.
        /// </summary>
        public const string ConfigPropertyName = "Config";

        private Config _config = null;

        /// <summary>
        /// Sets and gets the Config property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Config Config
        {
            get
            {
                return _config;
            }

            set
            {
                if (_config == value)
                {
                    return;
                }

                _config = value;
                RaisePropertyChanged(ConfigPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="LogMessages" /> property's name.
        /// </summary>
        public const string LogMessagesPropertyName = "LogMessages";

        private string _logMessages = null;

        /// <summary>
        /// Sets and gets the LogMessages property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LogMessages
        {
            get
            {
                return _logMessages;
            }

            set
            {
                if (_logMessages == value)
                {
                    return;
                }

                _logMessages = value;
                RaisePropertyChanged(LogMessagesPropertyName);
            }
        }

        private RelayCommand _enableChatLogMonitor;

        /// <summary>
        /// Gets the EnableChatLogMonitor.
        /// </summary>
        public RelayCommand EnableChatLogMonitor
        {
            get
            {
                return _enableChatLogMonitor
                    ?? (_enableChatLogMonitor = new RelayCommand(
                    () =>
                    {
                        
                    }));
            }
        }

        private RelayCommand _enableMissionLogMonitor;

        /// <summary>
        /// Gets the EnableMissionLogMonitor.
        /// </summary>
        public RelayCommand EnableMissionLogMonitor
        {
            get
            {
                return _enableMissionLogMonitor
                    ?? (_enableMissionLogMonitor = new RelayCommand(
                    () =>
                    {
                        var missionLogService = (Application.Current as App).MissionLogDataService;
                        if (missionLogService == null)
                            return;

                        if( !Config.IsMissionLogMonitorEnabled )
                            missionLogService.Stop();
                        else
                            missionLogService.Start();
                    }));
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}