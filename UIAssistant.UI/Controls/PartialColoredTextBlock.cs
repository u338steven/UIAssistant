using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.ComponentModel;

namespace UIAssistant.UI.Controls
{
    public class PartialColoredTextBlock : TextBlock
    {
        public int PartialColoredStart
        {
            get { return (int)GetValue(ColoredStartProperty); }
            set { SetValue(ColoredStartProperty, value); }
        }

        public int PartialColoredLength
        {
            get { return (int)GetValue(ColoredLengthProperty); }
            set { SetValue(ColoredLengthProperty, value); }
        }

        public Brush PartialColoredForeground
        {
            get { return (Brush)GetValue(ColoredForegroundProperty); }
            set { SetValue(ColoredForegroundProperty, value); }
        }

        public Brush PartialColoredBackground
        {
            get { return (Brush)GetValue(ColoredBackgroundProperty); }
            set { SetValue(ColoredBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ColoredStartProperty =
            DependencyProperty.Register(nameof(PartialColoredStart), typeof(int), typeof(PartialColoredTextBlock), new UIPropertyMetadata(0, OnValueChanged));

        public static readonly DependencyProperty ColoredLengthProperty =
            DependencyProperty.Register(nameof(PartialColoredLength), typeof(int), typeof(PartialColoredTextBlock), new UIPropertyMetadata(0, OnValueChanged));

        public static readonly DependencyProperty ColoredForegroundProperty =
            DependencyProperty.Register(nameof(PartialColoredForeground), typeof(Brush), typeof(PartialColoredTextBlock), new UIPropertyMetadata(SystemColors.ControlTextBrush, OnValueChanged));

        public static readonly DependencyProperty ColoredBackgroundProperty =
            DependencyProperty.Register(nameof(PartialColoredBackground), typeof(Brush), typeof(PartialColoredTextBlock), new UIPropertyMetadata(Brushes.Transparent, OnValueChanged));

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PartialColoredTextBlock).HandlePropertyChanged();
        }

        private void HandlePropertyChanged()
        {
            var sourceText = Text;
            if (sourceText.Length == 0)
            {
                return;
            }

            this.Inlines.Clear();

            var startIndex = PartialColoredStart;
            var length = PartialColoredLength;

            if (length <= 0 || startIndex < 0 || sourceText.Length <= startIndex)
            {
                Inlines.Add(sourceText);
            }
            else
            {
                Inlines.Add(sourceText.Substring(0, startIndex));
                Inlines.Add(new Run()
                {
                    Text = sourceText.Substring(startIndex, length),
                    Foreground = PartialColoredForeground,
                    //FontWeight = FontWeights.Bold,
                    Background = PartialColoredBackground
                });
                Inlines.Add(sourceText.Substring(startIndex + length));
            }
        }
    }
}
