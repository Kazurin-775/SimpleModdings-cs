using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using ModernWpf.Controls;
using SimpleModder;

namespace SimpleModdings
{
    public partial class MainWindow
    {
        private readonly List<string> _patchesList;
        private readonly DispatcherTimer _patchFilterTimer;

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

            _patchFilterTimer = new DispatcherTimer
            {
                Interval = System.TimeSpan.FromMilliseconds(200),
            };
            _patchFilterTimer.Tick += UpdatePatchSuggestions;
        }

        private void Log(string log)
        {
            LogTextBox.AppendText(log + "\n");
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
            var patchScript = await RawPatchScript.LoadFromFile(path);
            Log($"已加载补丁：{patchScript.Name}");
        }

        private void OnPatchChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            LoadPatchScript(args.SelectedItem.ToString());
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
    }
}
