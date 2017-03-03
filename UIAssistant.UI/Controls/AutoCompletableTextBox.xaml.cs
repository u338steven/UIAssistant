// Based on http://blogs.wankuma.com/kazuki/archive/2008/02/05/121456.aspx
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using System.Reactive.Linq;
using System.ComponentModel;
using Data = System.ComponentModel.DataAnnotations;

using UIAssistant.Infrastructure.Commands;

namespace UIAssistant.UI.Controls
{
    /// <summary>
    /// AutoCompletableTextBox.xaml の相互作用ロジック
    /// </summary>
    public partial class AutoCompletableTextBox : UserControl, IDataErrorInfo
    {
        #region TextProperty
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(AutoCompletableTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion

        #region CandidatesProperty
        public static readonly DependencyProperty CandidatesProperty =
            DependencyProperty.Register(
                nameof(Candidates),
                typeof(IEnumerable<ICandidate>),
                typeof(AutoCompletableTextBox),
                new PropertyMetadata(null));

        public IEnumerable<ICandidate> Candidates
        {
            get { return (IEnumerable<ICandidate>)GetValue(CandidatesProperty); }
            set { SetValue(CandidatesProperty, value); }
        }
        #endregion

        #region CanidatesGeneratorProperty
        public static readonly DependencyProperty CandidatesGeneratorProperty =
            DependencyProperty.Register(
                nameof(CandidatesGenerator),
                typeof(ICandidatesGenerator),
                typeof(AutoCompletableTextBox),
                new PropertyMetadata(null));

        public ICandidatesGenerator CandidatesGenerator
        {
            get { return Dispatcher.Invoke(() => (ICandidatesGenerator)GetValue(CandidatesGeneratorProperty)); }
            set { SetValue(CandidatesGeneratorProperty, value); }
        }
        #endregion

        #region ValidatorProperty
        public static readonly DependencyProperty ValidatorProperty =
            DependencyProperty.Register(
                nameof(Validator),
                typeof(IValidatable<string>),
                typeof(AutoCompletableTextBox),
                new PropertyMetadata(null));

        public IValidatable<string> Validator
        {
            get { return Dispatcher.Invoke(() => (IValidatable<string>)GetValue(ValidatorProperty)); }
            set { SetValue(ValidatorProperty, value); }
        }
        #endregion

        #region IsResidentProperty
        public static readonly DependencyProperty IsResidentProperty =
            DependencyProperty.Register(
                nameof(IsResident),
                typeof(bool),
                typeof(AutoCompletableTextBox),
                new PropertyMetadata(false));

        public bool IsResident
        {
            get { return (bool)GetValue(IsResidentProperty); }
            set { SetValue(IsResidentProperty, value); }
        }
        #endregion

        public string Error
        {
            get
            {
                var results = new List<Data.ValidationResult>();
                if (Data.Validator.TryValidateObject(
                    this,
                    new Data.ValidationContext(this, null, null),
                    results))
                {
                    return string.Empty;
                }
                return string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
            }
        }

        public string this[string columnName]
        {
            get
            {
                var results = new List<Data.ValidationResult>();
                if (Data.Validator.TryValidateProperty(
                    GetType().GetProperty(columnName).GetValue(this, null),
                    new Data.ValidationContext(this, null, null) { MemberName = columnName },
                    results))
                {
                    return Validator?.Validate(textBox.Text)?.ErrorMessage;
                }
                return results.First().ErrorMessage;
            }
        }

        public AutoCompletableTextBox()
        {
            InitializeComponent();
            popupListBox.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(popupListBox_MouseLeftButtonDown), true);

            // GetCandidates
            Observable.FromEvent<TextChangedEventHandler, TextChangedEventArgs>(
                action => (s, ev) => action(ev),
                h => textBox.TextChanged += h,
                h => textBox.TextChanged -= h)
                .Where(_ => IsResident)
                .Select(text => textBox.Text)
                .Select(text => Task.Run(() => CandidatesGenerator?.GenerateCandidates(text)))
                .Switch()
                .ObserveOnDispatcher()
                .Select(candidates => Candidates = candidates)
                .Subscribe(_ => MovePopup());
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Space)
            {
                popup.IsOpen = true;
                MovePopup();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Down || (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.N))
            {
                if (popupListBox.SelectedIndex < popupListBox.Items.Count - 1)
                    ++popupListBox.SelectedIndex;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Up || (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.P))
            {
                if (popupListBox.SelectedIndex > 0)
                    --popupListBox.SelectedIndex;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                popup.IsOpen = false;
                e.Handled = true;
                return;
            }

            if ((e.Key == Key.Enter || e.Key == Key.Tab) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                if (popup.IsOpen)
                    Complete();
                else
                    textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Space)
            {
                if (popup.IsOpen)
                    Complete();
            }
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (IsResident)
            {
                popup.IsOpen = true;
                Candidates = CandidatesGenerator?.GenerateCandidates(textBox.Text);
            }
            MovePopup();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (sender as TextBox);
            popup.IsOpen = false;
            textBox.Text = textBox.Text.TrimEnd();
        }

        private void MovePopup()
        {
            if (!IsResident && !popup.IsOpen)
            {
                return;
            }

            if (!IsResident)
            {
                Candidates = CandidatesGenerator?.GenerateCandidates(textBox.Text);
            }

            if (popupListBox.Items.Count != 0)
            {
                if (popupListBox.SelectedIndex == -1)
                {
                    popupListBox.SelectedIndex = 0;
                }
                popup.IsOpen = true;
                popup.PlacementTarget = textBox;
                popup.PlacementRectangle = textBox.GetRectFromCharacterIndex(textBox.CaretIndex);
                popupListBox.UpdateLayout();
                AdjustPopupLocation();
            }
            else
            {
                popup.IsOpen = false;
            }
        }

        private void AdjustPopupLocation()
        {
            if (SystemParameters.MenuDropAlignment)
            {
                // left-aligned
                popup.Placement = PlacementMode.RelativePoint;
                popup.HorizontalOffset = popupListBox.ActualWidth;
                popup.VerticalOffset = textBox.ActualHeight;
            }
            else
            {
                popup.Placement = PlacementMode.Bottom;
            }
        }

        private static readonly char[] _delimiter = { ' ', '\r', '\n', '\t' };
        private static string GetCurrentWord(TextBox textBox)
        {
            if (textBox.CaretIndex == 0)
            {
                return string.Empty;
            }

            var lastWordTail = textBox.CaretIndex - 1;
            var lastWordHead = textBox.Text.LastIndexOfAny(_delimiter, lastWordTail) + 1;
            var lastWordLength = textBox.CaretIndex - lastWordHead;

            return textBox.Text.Substring(lastWordHead, lastWordLength);
        }

        private void Complete()
        {
            if (popupListBox.SelectedItem == null)
            {
                return;
            }
            var caretIndex = textBox.CaretIndex;
            var currentWord = GetCurrentWord(textBox);
            var selectedText = (popupListBox.SelectedItem as ICandidate).Name;

            var tmpText = textBox.Text.Remove(caretIndex - currentWord.Length, currentWord.Length);
            textBox.BeginChange();
            textBox.Text = tmpText.Insert(caretIndex - currentWord.Length, selectedText);
            textBox.CaretIndex = caretIndex + selectedText.Length;
            textBox.EndChange();

            if (!IsResident)
            {
                popup.IsOpen = false;
            }
        }

        private void popupListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            textBox.Focus();
            if (e.Key == Key.Escape)
            {
                popup.IsOpen = false;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                Complete();
                e.Handled = true;
                return;
            }
        }

        private void popupListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Complete();
            textBox.Focus();
        }
    }
}
