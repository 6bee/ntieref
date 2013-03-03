using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using C1.Silverlight;

namespace ProductManager.Silverlight.Controls
{
    public class SearchBox : C1TextBoxBase
    {
        private DispatcherTimer _textChangedTimer;

        public SearchBox()
        {
            Watermark = "Type here to filter";

            _textChangedTimer = new DispatcherTimer() { Interval = TextChangedDelay };
            _textChangedTimer.Tick += new EventHandler(_textChangedTimer_Tick);
            base.TextChanged += new TextChangedEventHandler(C1SearchBox_TextChanged);
        }

        #region TextChangedDelay

        /// <summary>
        /// Gets or sets the text changed delay.
        /// </summary>
        /// <value>The text changed delay.</value>
        public TimeSpan TextChangedDelay
        {
            get { return (TimeSpan)GetValue(TextChangedDelayProperty); }
            set { SetValue(TextChangedDelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextChangedDelay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextChangedDelayProperty =
            DependencyProperty.Register("TextChangedDelay", typeof(TimeSpan), typeof(SearchBox), new PropertyMetadata(TimeSpan.FromMilliseconds(250), OnTextChangedDelayChanged));

        private static void OnTextChangedDelayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchBox)sender;
            searchBox._textChangedTimer.Interval = (TimeSpan)e.NewValue;
        }

        #endregion

        /// <summary>
        /// Occurs when text has changed.
        /// (Overrides the one in C1TextBoxBase)
        /// </summary>
        public new event EventHandler<TextChangedEventArgs> TextChanged;

        // *** avoid firing to often ***
        // fires the event when TextChangedDelay time passed without textchanged
        private TextChangedEventArgs _lastTextChangedEventArgs;
        void C1SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _lastTextChangedEventArgs = e;
            _textChangedTimer.Start();
        }
        void _textChangedTimer_Tick(object sender, EventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, _lastTextChangedEventArgs);
                _textChangedTimer.Stop();
            }
        }
    }
}
