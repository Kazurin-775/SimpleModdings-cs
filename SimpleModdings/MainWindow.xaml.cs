using System.Collections.Generic;
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
    }
}
