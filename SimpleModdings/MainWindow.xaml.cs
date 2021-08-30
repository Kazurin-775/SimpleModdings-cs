using System.Collections.Generic;
using ModernWpf.Controls;
using SimpleModder;

namespace SimpleModdings
{
    public partial class MainWindow
    {
        private List<string> _patchesList;

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
        }
    }
}
