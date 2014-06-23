using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace BlogWriter.Wpf.Controls
{
    public class SlidingPage : ContentControl
    {
        private sealed class EnterVisibilityAnimation : AnimationTimeline
        {
            public EnterVisibilityAnimation()
            {
                SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(UIElement.Visibility)"));
            }

            public override Type TargetPropertyType
            {
                get { return typeof(Visibility); }
            }

            protected override Freezable CreateInstanceCore()
            {
                return new EnterVisibilityAnimation();
            }

            public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
            {
                return Visibility.Visible;
            }
        }

        private sealed class ExitVisibilityAnimation : AnimationTimeline
        {
            public ExitVisibilityAnimation()
            {
                SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(UIElement.Visibility)"));
            }

            public override Type TargetPropertyType
            {
                get { return typeof(Visibility); }
            }

            protected override Freezable CreateInstanceCore()
            {
                return new ExitVisibilityAnimation();
            }

            public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
            {
                switch (animationClock.CurrentState)
                {
                    case ClockState.Active:
                        return Visibility.Visible;

                    case ClockState.Filling:
                        return Visibility.Collapsed;

                    default:
                        throw new Exception("unexpected clock state: " + animationClock.CurrentState);
                }
            }
        }

        private sealed class SlidingTrigger : Trigger
        {
            private readonly SlidingPage _slidingPage;

            public SlidingTrigger(SlidingPage slidingPage)
            {
                _slidingPage = slidingPage;

                Property = SlidingPage.IsActiveProperty;
                Value = true;

                SetActionTriggers();
            }

            private void SetActionTriggers()
            {
                var width = _slidingPage.ActualWidth;


                EnterActions.Clear();
                ExitActions.Clear();

                var enterStoryboard = new Storyboard();

                var enterMarginAnimation = new ThicknessAnimation(new Thickness(width, 0, -width, 0), new Thickness(0), new Duration(TimeSpan.FromSeconds(.75))) { DecelerationRatio = .9 };
                enterMarginAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.Margin)"));
                enterStoryboard.Children.Add(enterMarginAnimation);

                var enterOpacityAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(.25)));
                enterOpacityAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(UIElement.Opacity)"));
                enterStoryboard.Children.Add(enterOpacityAnimation);

                var enterVisibilityAnimation = new EnterVisibilityAnimation();
                enterStoryboard.Children.Add(enterVisibilityAnimation);

                EnterActions.Add(new BeginStoryboard { Storyboard = enterStoryboard });


                var exitStoryboard = new Storyboard();

                var exitMarginAnimation = new ThicknessAnimation(new Thickness(-width, 0, width, 0), new Duration(TimeSpan.FromSeconds(.5))) { AccelerationRatio = .9 };
                exitMarginAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.Margin)"));
                exitStoryboard.Children.Add(exitMarginAnimation);

                var exitOpacityAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(.5)));
                exitOpacityAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(UIElement.Opacity)"));
                exitStoryboard.Children.Add(exitOpacityAnimation);

                var exitVisibilityAnimation = new ExitVisibilityAnimation();
                exitStoryboard.Children.Add(exitVisibilityAnimation);

                ExitActions.Add(new BeginStoryboard { Storyboard = exitStoryboard });
            }

            internal void OnWidthChanged()
            {
                SetActionTriggers();
            }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(SlidingPage), new PropertyMetadata(true));
        private readonly SlidingTrigger _slidingTrigger;

        public SlidingPage()
        {
            _slidingTrigger = new SlidingTrigger(this);
            var baseStyle = TryFindResource(typeof(SlidingPage)) as Style;
            if (baseStyle == null)
            {
                baseStyle = TryFindResource(typeof(ContentControl)) as Style;
            }
            var style = new Style(typeof(SlidingPage), baseStyle);
            style.Setters.Add(new Setter(SlidingPage.OpacityProperty, .25));
            style.Triggers.Add(_slidingTrigger);
            Style = style;
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (sizeInfo.WidthChanged)
            {
                _slidingTrigger.OnWidthChanged();
            }
        }
    }
}
