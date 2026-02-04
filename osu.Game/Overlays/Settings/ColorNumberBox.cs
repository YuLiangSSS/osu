using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;

namespace osu.Game.Overlays.Settings
{
    public partial class ColorNumberBox : SettingsNumberBox
    {
        protected override Drawable CreateControl() => new ColorNumberControl
        {
            RelativeSizeAxes = Axes.X,
        };

        private sealed partial class ColorNumberControl : CompositeDrawable, IHasCurrentValue<int?>
        {
            private readonly BindableWithCurrent<int?> current = new BindableWithCurrent<int?>();

            public Bindable<int?> Current
            {
                get => current.Current;
                set => current.Current = value;
            }

            public ColorNumberControl()
            {
                AutoSizeAxes = Axes.Y;

                ColorOutlinedNumberBox numberBox;

                InternalChildren = new Drawable[]
                {
                    numberBox = new ColorOutlinedNumberBox
                    {
                        RelativeSizeAxes = Axes.X,
                        CommitOnFocusLost = true,
                    }
                };

                numberBox.Current.BindValueChanged(e =>
                {
                    if (string.IsNullOrEmpty(e.NewValue))
                    {
                        Current.Value = null;
                        return;
                    }

                    if (int.TryParse(e.NewValue, out int intVal))
                    {
                        // clamp to 0..255
                        if (intVal < 0) intVal = 0;
                        if (intVal > 255) intVal = 255;
                        Current.Value = intVal;
                    }
                    else
                        numberBox.NotifyInputError();

                    // trigger Current again to either restore the previous text box value, or to reformat the new value via .ToString().
                    Current.TriggerChange();
                });

                Current.BindValueChanged(e =>
                {
                    numberBox.Current.Value = e.NewValue?.ToString();
                });
            }

            private partial class ColorOutlinedNumberBox : OutlinedTextBox
            {
                public ColorOutlinedNumberBox()
                {
                    InputProperties = new TextInputProperties(TextInputType.Number, false);
                }

                protected override bool CanAddCharacter(char character) => char.IsAsciiDigit(character);

                public new void NotifyInputError() => base.NotifyInputError();
            }
        }
    }
}
