//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.AppRecording;
using Windows.ApplicationModel.Core;
using ColorCode;

namespace AppUIBasics
{
    /// <summary>
    /// Describes a textual substitution in sample content.
    /// If enabled (default), then $(Key) is replaced with the stringified value.
    /// If disabled, then $(Key) is replaced with the empty string.
    /// </summary>
    public sealed class ControlExampleSubstitution : DependencyObject
    {
        public event TypedEventHandler<ControlExampleSubstitution, object> ValueChanged;

        public string Key { get; set; }

        private object _value = null;
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                ValueChanged?.Invoke(this, null);
            }
        }

        private bool _enabled = true;
        public bool IsEnabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                ValueChanged?.Invoke(this, null);
            }
        }

        public string ValueAsString()
        {
            if (!IsEnabled)
            {
                return string.Empty;
            }

            object value = Value;

            // For solid color brushes, use the underlying color.
            if (value is SolidColorBrush)
            {
                value = ((SolidColorBrush)value).Color;
            }

            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }

    [ContentProperty(Name = "Example")]
    public sealed partial class ControlExample : UserControl
    {
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty ExampleProperty = DependencyProperty.Register("Example", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public object Example
        {
            get { return GetValue(ExampleProperty); }
            set { SetValue(ExampleProperty, value); }
        }

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public object Output
        {
            get { return GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register("Options", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public object Options
        {
            get { return GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public static readonly DependencyProperty XamlProperty = DependencyProperty.Register("Xaml", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string Xaml
        {
            get { return (string)GetValue(XamlProperty); }
            set { SetValue(XamlProperty, value); }
        }

        public static readonly DependencyProperty XamlSourceProperty = DependencyProperty.Register("XamlSource", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public Uri XamlSource
        {
            get { return (Uri)GetValue(XamlSourceProperty); }
            set { SetValue(XamlSourceProperty, value); }
        }

        public static readonly DependencyProperty CSharpProperty = DependencyProperty.Register("CSharp", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string CSharp
        {
            get { return (string)GetValue(CSharpProperty); }
            set { SetValue(CSharpProperty, value); }
        }

        public static readonly DependencyProperty CSharpSourceProperty = DependencyProperty.Register("CSharpSource", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public Uri CSharpSource
        {
            get { return (Uri)GetValue(CSharpSourceProperty); }
            set { SetValue(CSharpSourceProperty, value); }
        }

        public static readonly DependencyProperty SubstitutionsProperty = DependencyProperty.Register("Substitutions", typeof(IList<ControlExampleSubstitution>), typeof(ControlExample), new PropertyMetadata(null));
        public IList<ControlExampleSubstitution> Substitutions
        {
            get { return (IList<ControlExampleSubstitution>)GetValue(SubstitutionsProperty); }
            set { SetValue(SubstitutionsProperty, value); }
        }

        public static readonly DependencyProperty ExampleHeightProperty = DependencyProperty.Register("ExampleHeight", typeof(GridLength), typeof(ControlExample), new PropertyMetadata(new GridLength(1, GridUnitType.Star)));
        public GridLength ExampleHeight
        {
            get { return (GridLength)GetValue(ExampleHeightProperty); }
            set { SetValue(ExampleHeightProperty, value); }
        }

        public static readonly DependencyProperty WebViewHeightProperty = DependencyProperty.Register("WebViewHeight", typeof(int), typeof(ControlExample), new PropertyMetadata(400));
        public int WebViewHeight
        {
            get { return (int)GetValue(WebViewHeightProperty); }
            set { SetValue(WebViewHeightProperty, value); }
        }

        public static readonly DependencyProperty WebViewWidthProperty = DependencyProperty.Register("WebViewWidth", typeof(int), typeof(ControlExample), new PropertyMetadata(800));
        public int WebViewWidth
        {
            get { return (int)GetValue(WebViewWidthProperty); }
            set { SetValue(WebViewWidthProperty, value); }
        }

        public new static readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ControlExample), new PropertyMetadata(HorizontalAlignment.Left));
        public new HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty MinimumUniversalAPIContractProperty = DependencyProperty.Register("MinimumUniversalAPIContract", typeof(int), typeof(ControlExample), new PropertyMetadata(null));
        public int MinimumUniversalAPIContract
        {
            get { return (int)GetValue(MinimumUniversalAPIContractProperty); }
            set { SetValue(MinimumUniversalAPIContractProperty, value); }
        }

        public ControlExample()
        {
            this.InitializeComponent();
            Substitutions = new List<ControlExampleSubstitution>();

            ControlPresenter.RegisterPropertyChangedCallback(ContentPresenter.PaddingProperty, ControlPaddingChangedCallback);
            this.Loaded += ControlExample_Loaded;
        }

        private void ControlExample_Loaded(object sender, RoutedEventArgs e)
        {
            if(!XamlPresenter.IsEmpty && !CSharpPresenter.IsEmpty)
            {
                VisualStateManager.GoToState(this, "SeparatorVisible", false);
            }
        }

        private void rootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (MinimumUniversalAPIContract != 0 && !(ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", (ushort)MinimumUniversalAPIContract)))
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private enum SyntaxHighlightLanguage { Xml, CSharp };

        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            TakeScreenshot();
        }

        private void ScreenshotDelayButton_Click(object sender, RoutedEventArgs e)
        {
            TakeScreenshotWithDelay();
        }

        private async void TakeScreenshot()
        {
            // Using RTB doesn't capture popups; but in the non-delay case, that probably isn't necessary.
            // This method seems more robust than using AppRecordingManager and also will work on non-desktop devices.

            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(ControlPresenter);

            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();

            var file = await UIHelper.ScreenshotStorageFolder.CreateFileAsync(GetBestScreenshotName(), CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var displayInformation = DisplayInformation.GetForCurrentView();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    (uint)rtb.PixelWidth,
                    (uint)rtb.PixelHeight,
                    displayInformation.RawDpiX,
                    displayInformation.RawDpiY,
                    pixels);

                await encoder.FlushAsync();
            }
        }

        public async void TakeScreenshotWithDelay()
        {
            // 3 second countdown
            for (int i = 3; i > 0; i--)
            {
                ScreenshotStatusTextBlock.Text = i.ToString();
                await Task.Delay(1000);
            }
            ScreenshotStatusTextBlock.Text = "Image captured";

            // AppRecordingManager is desktop-only, and its use here is quite hacky,
            // but it is able to capture popups (though not theme shadows).

            bool isAppRecordingPresent = ApiInformation.IsTypePresent("Windows.Media.AppRecording.AppRecordingManager");
            if (!isAppRecordingPresent)
            {
                // Better than doing nothing
                TakeScreenshot();
            }
            else
            {
                var manager = AppRecordingManager.GetDefault();
                if (manager.GetStatus().CanRecord)
                {
                    var result = await manager.SaveScreenshotToFilesAsync(
                        ApplicationData.Current.LocalFolder,
                        "appScreenshot",
                        AppRecordingSaveScreenshotOption.HdrContentVisible,
                        manager.SupportedScreenshotMediaEncodingSubtypes);

                    if (result.Succeeded)
                    {
                        // Open the screenshot back up
                        var screenshotFile = await ApplicationData.Current.LocalFolder.GetFileAsync("appScreenshot.png");
                        using (var stream = await screenshotFile.OpenAsync(FileAccessMode.Read))
                        {
                            var decoder = await BitmapDecoder.CreateAsync(stream);

                            // Find the control in the picture
                            GeneralTransform t = ControlPresenter.TransformToVisual(Window.Current.Content);
                            Point pos = t.TransformPoint(new Point(0, 0));
;
                            if (!CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar)
                            {
                                // Add the height of the title bar, which I really wish was programmatically available anywhere.
                                pos.Y += 32.0;
                            }

                            // Crop the screenshot to the control area
                            var transform = new BitmapTransform() { Bounds = new BitmapBounds() {
                                X = (uint)(Math.Ceiling(pos.X)) + 1, // Avoid the 1px window border
                                Y = (uint)(Math.Ceiling(pos.Y)) + 1,
                                Width = (uint)ControlPresenter.ActualWidth - 1, // Rounding issues -- this avoids capturing the control border
                                Height = (uint)ControlPresenter.ActualHeight - 1} };

                            var softwareBitmap = await decoder.GetSoftwareBitmapAsync(
                                decoder.BitmapPixelFormat,
                                BitmapAlphaMode.Ignore,
                                transform,
                                ExifOrientationMode.IgnoreExifOrientation,
                                ColorManagementMode.DoNotColorManage);

                            // Save the cropped picture
                            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(GetBestScreenshotName(), CreationCollisionOption.ReplaceExisting);
                            using (var outStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, outStream);
                                encoder.SetSoftwareBitmap(softwareBitmap);
                                await encoder.FlushAsync();
                            }
                        }

                        // Delete intermediate file
                        await screenshotFile.DeleteAsync();
                    }
                }
            }

            await Task.Delay(1000);
            ScreenshotStatusTextBlock.Text = "";
        }

        string GetBestScreenshotName()
        {
            string imageName = "Screenshot.png";
            if (XamlSource != null)
            {
                // Most of them don't have this, but the xaml source name is a really good file name
                string xamlSource = XamlSource.LocalPath;
                string fileName = Path.GetFileNameWithoutExtension(xamlSource);
                if (!String.IsNullOrWhiteSpace(fileName))
                {
                    imageName = fileName + ".png";
                }
            }
            else if (!String.IsNullOrWhiteSpace(Name))
            {
                // Put together the page name and the control example name
                UIElement uie = this;
                while (uie != null && !(uie is Page))
                {
                    uie = VisualTreeHelper.GetParent(uie) as UIElement;
                }
                if (uie != null)
                {
                    string name = Name;
                    if (name.Equals("RootPanel"))
                    {
                        // This is the default name for the example; add an index on the end to disambiguate
                        imageName = uie.GetType().Name + "_" + ((Panel)this.Parent).Children.IndexOf(this).ToString() + ".png";
                    }
                    else
                    {
                        imageName = uie.GetType().Name + "_" + name + ".png";
                    }
                }
            }
            return imageName;
        }

        private void ControlPaddingChangedCallback(DependencyObject sender, DependencyProperty dp)
        {
            ControlPaddingBox.Text = ControlPresenter.Padding.ToString();
        }

        private void ControlPaddingBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && !String.IsNullOrWhiteSpace(ControlPaddingBox.Text))
            {
                EvaluatePadding();
            }
        }

        private void ControlPaddingBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EvaluatePadding();
        }

        private void EvaluatePadding()
        {
            // Evaluate the text in the ControlPaddingBox as padding
            string[] strs = ControlPaddingBox.Text.Split(new char[] { ' ', ',' });
            double[] nums = new double[4];
            for (int i = 0; i < strs.Length; i++)
            {
                if (!Double.TryParse(strs[i], out nums[i]))
                {
                    //  Bad format
                    return;
                }
            }

            switch (nums.Length)
            {
                case 1:
                    ControlPresenter.Padding = new Thickness(nums[0]);
                    break;

                case 2:
                    ControlPresenter.Padding = new Thickness(nums[0], nums[1], nums[0], nums[1]);
                    break;

                case 4:
                    ControlPresenter.Padding = new Thickness(nums[0], nums[1], nums[2], nums[3]);
                    break;
            }
        }
    }
}
