using Android.Content;
using Android.Views.InputMethods;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WaveEngine.Adapter;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace Noesis
{
    public class WaveNoesisSoftwareKeyboard : SoftwareKeyboard
    {
        private Task<string> textTask;
        private Noesis.TextBox textBox;

        public Noesis.View View { get; set; }

        /// <summary>
        /// Called when a UI element gets the focus and software keyboard should be opened
        /// Should return true if software keyboard is supported and will be shown
        /// </summary>
        public virtual bool Show(UIElement focusedElement)
        {
            //this.textBox = focusedElement as Noesis.TextBox;

            //if (this.textBox != null)
            //{
            //    this.textTask = WaveServices.Platform.ShowTextDialogBoxAsync("", "", this.textBox.Text);
            //    return true;
            //}

            var adapter = (Game.Current.Application.Adapter as Adapter);
            var view = adapter.GameView;
            view.Focusable = true;
            view.FocusableInTouchMode = true;
            view.RequestFocus();

            adapter.GameView.KeyPress -= GameView_KeyPress;
            adapter.GameView.KeyPress += GameView_KeyPress;

            Context c = adapter.Context;

            var keyboard = c.Resources.Configuration.Keyboard;
            if(keyboard == Android.Content.Res.KeyboardType.Nokeys)
            {
                InputMethodManager im = (InputMethodManager)c.GetSystemService(Context.InputMethodService);
                im.ShowSoftInput(view, ShowFlags.Forced);
            }

            return false;
        }

        private void GameView_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            if (e.Event.Action == Android.Views.KeyEventActions.Down)
            {
                uint ch = 0;
                if (e.Event.KeyCode == Android.Views.Keycode.Del)
                {
                    ch = '\b';
                    this.View.KeyDown(Key.Back);
                }
                else if (e.Event.KeyCode == Android.Views.Keycode.Enter)
                {
                    ch = '\n';
                    this.View.KeyDown(Key.Enter);
                }
                else
                {
                    ch = (uint)e.Event.GetUnicodeChar(e.Event.MetaState);
                }

                this.View.Char(ch);
            }
        }

        /// <summary>
        /// Called when UI element loses focus and software keyboard should be closed
        /// </summary>
        public virtual void Hide()
        {
            var adapter = (Game.Current.Application.Adapter as Adapter);

            InputMethodManager imm = (InputMethodManager)adapter.Context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(adapter.GameView.WindowToken, 0);

            adapter.GameView.KeyPress -= GameView_KeyPress;
        }

        /// <summary>
        /// Allows updating UI element contents with the software keyboard text
        /// </summary>
        public virtual void Update()
        {
            //if (this.textTask != null && this.textTask.Status == TaskStatus.RanToCompletion)
            //{
            //    if (this.textTask.Result != null)
            //    {
            //        this.textBox.Text = this.textTask.Result;
            //    }

            //    this.textBox.Keyboard.Focus(null);
            //}
        }
    }
}
