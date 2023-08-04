using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace BocchiTracker.Client.Config.Controls
{
    /// <summary>
    /// PathInput.xaml の相互作用ロジック
    /// </summary>
    public partial class PathInput : UserControl
    {
        public static readonly DependencyProperty IsFolderPickerProperty =
            DependencyProperty.Register("IsFolderPicker", typeof(bool), typeof(PathInput), new PropertyMetadata(false));

        public bool IsFolderPicker
        {
            get { return (bool)GetValue(IsFolderPickerProperty); }
            set { SetValue(IsFolderPickerProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PathInput), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint", typeof(string), typeof(PathInput), new PropertyMetadata(null));

        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        public static readonly DependencyProperty HelperTextProperty =
            DependencyProperty.Register("HelperText", typeof(string), typeof(PathInput), new PropertyMetadata(null));

        public string HelperText
        {
            get { return (string)GetValue(HelperTextProperty); }
            set { SetValue(HelperTextProperty, value); }
        }

        public PathInput()
        {
            InitializeComponent();
            this.BrowseButton.Click += (_, __) => OnBrowse();
        }

        public void OnBrowse()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = IsFolderPicker;
            dialog.Title = HelperText;
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            Text = dialog.FileName;
        }
    }
}
