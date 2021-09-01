﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ModernWpf.Controls;
using SimpleModder;

namespace SimpleModdings
{
    public partial class MainWindow
    {
        private readonly List<string> _patchesList;
        private readonly DispatcherTimer _patchFilterTimer;
        private PatchScript _patchScript;

        public MainWindow()
        {
            InitializeComponent();
            _patchesList = PatchesList.Obtain();
            if (_patchesList.Count == 0)
            {
                new ContentDialog
                {
                    Title = "找不到补丁",
                    Content = "我们没有在 patches 目录中发现任何补丁。\n请检查程序的配置是否正确。",
                    CloseButtonText = "确定",
                }.ShowAsync();
            }

#pragma warning disable 0162
            TestMode.IsOn = TestConfig.TestModeByDefault;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (TestConfig.PreloadPatch != null)
            {
                PatchesBox.Text = TestConfig.PreloadPatch;
                LoadPatchScript(TestConfig.PreloadPatch);
            }
#pragma warning restore 0162

            _patchFilterTimer = new DispatcherTimer
            {
                Interval = System.TimeSpan.FromMilliseconds(200),
            };
            _patchFilterTimer.Tick += UpdatePatchSuggestions;

            Logger.OnMessage += BackgroundLog;
        }

        private void Log(string log)
        {
            LogTextBox.AppendText(log + "\n");
        }

        private void BackgroundLog(string log)
        {
            LogTextBox.Dispatcher.Invoke(() => Log(log));
        }

        private IEnumerable<string> FilterPatch(string query)
        {
            query = query.ToLower();
            return _patchesList.Where(x => x.ToLower().Contains(query));
        }

        private void OnPatchSearchChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
                return;
            _patchFilterTimer.Stop();
            _patchFilterTimer.Start();
        }

        private void UpdatePatchSuggestions(object sender, object args)
        {
            PatchesBox.ItemsSource = FilterPatch(PatchesBox.Text);
        }

        private async void LoadPatchScript(string filename)
        {
            var path = Path.Combine("patches", filename);
            var rawPatchScript = await RawPatchScript.LoadFromFile(path);
            _patchScript = new PatchScript(rawPatchScript);
            ProgramDir.Text = _patchScript.DefaultPath;
            Log($"已加载补丁：{_patchScript.Name}");
        }

        private void OnPatchSearchTriggered(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var enumerator = FilterPatch(args.QueryText).GetEnumerator();
            if (enumerator.MoveNext())
            {
                var filename = enumerator.Current;
                sender.Text = filename;
                LoadPatchScript(filename);
            }

            enumerator.Dispose();
        }

        private async void OnExecute(object sender, RoutedEventArgs e)
        {
            if (_patchScript == null)
                return;
            ExecuteBtn.IsEnabled = false;
            var programDir = ProgramDir.Text;
            await Task.Run(() => _patchScript.DryRun(programDir));
            ExecuteBtn.IsEnabled = true;
        }
    }
}
