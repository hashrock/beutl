using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BEditor.Views.ManagePlugins
{
    public partial class Library : UserControl
    {
        public Library()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
