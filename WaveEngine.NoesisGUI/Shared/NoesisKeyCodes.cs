﻿// <auto-generated />

#region Usings Statements
using Noesis;
using System.Collections.Generic;
using WaveEngine.Common.Input;
#endregion

namespace WaveEngine.NoesisGUI
{
    internal class NoesisKeyCodes
    {
        private static Dictionary<Keys, Key> noesisKeyCode;

        static NoesisKeyCodes()
        {
            noesisKeyCode = new Dictionary<Keys, Key>();

            noesisKeyCode.Add(Keys.Back, Key.Back);
            noesisKeyCode.Add(Keys.Tab, Key.Tab);
            noesisKeyCode.Add(Keys.Clear, Key.Clear);
            noesisKeyCode.Add(Keys.Pause, Key.Pause);

            noesisKeyCode.Add(Keys.Escape, Key.Escape);

            noesisKeyCode.Add(Keys.Space, Key.Space);
            noesisKeyCode.Add(Keys.PageUp, Key.Prior);         // prior?
            noesisKeyCode.Add(Keys.PageDown, Key.Next);        // next?
            noesisKeyCode.Add(Keys.End, Key.End);
            noesisKeyCode.Add(Keys.Home, Key.Home);
            noesisKeyCode.Add(Keys.Left, Key.Left);
            noesisKeyCode.Add(Keys.Up, Key.Up);
            noesisKeyCode.Add(Keys.Right, Key.Right);
            noesisKeyCode.Add(Keys.Down, Key.Down);
            // SELECT KEY not defined
            noesisKeyCode.Add(Keys.Print, Key.Print);
            // EXECUTE KEY not defined
            // PRINTSCR KEY not defined
            noesisKeyCode.Add(Keys.Insert, Key.Insert);
            noesisKeyCode.Add(Keys.Delete, Key.Delete);
            noesisKeyCode.Add(Keys.Help, Key.Help);

            //noesisKeyCode.Add(Keys.Alpha0, Key.Alpha0);
            //noesisKeyCode.Add(Keys.Alpha1, Key.Alpha1);
            //noesisKeyCode.Add(Keys.Alpha2, Key.Alpha2);
            //noesisKeyCode.Add(Keys.Alpha3, Key.Alpha3);
            //noesisKeyCode.Add(Keys.Alpha4, Key.Alpha4);
            //noesisKeyCode.Add(Keys.Alpha5, Key.Alpha5);
            //noesisKeyCode.Add(Keys.Alpha6, Key.Alpha6);
            //noesisKeyCode.Add(Keys.Alpha7, Key.Alpha7);
            //noesisKeyCode.Add(Keys.Alpha8, Key.Alpha8);
            //noesisKeyCode.Add(Keys.Alpha9, Key.Alpha9);

            noesisKeyCode.Add(Keys.Number0, Key.NumPad0);
            noesisKeyCode.Add(Keys.Number1, Key.NumPad1);
            noesisKeyCode.Add(Keys.Number2, Key.NumPad2);
            noesisKeyCode.Add(Keys.Number3, Key.NumPad3);
            noesisKeyCode.Add(Keys.Number4, Key.NumPad4);
            noesisKeyCode.Add(Keys.Number5, Key.NumPad5);
            noesisKeyCode.Add(Keys.Number6, Key.NumPad6);
            noesisKeyCode.Add(Keys.Number7, Key.NumPad7);
            noesisKeyCode.Add(Keys.Number8, Key.NumPad8);
            noesisKeyCode.Add(Keys.Number9, Key.NumPad9);
            noesisKeyCode.Add(Keys.Multiply, Key.Multiply);
            //noesisKeyCode.Add(Keys.KeypadPlus, Key.Add);
            // SEPARATOR KEY not defined
            noesisKeyCode.Add(Keys.Subtract, Key.Subtract);
            noesisKeyCode.Add(Keys.Decimal, Key.Decimal);
            noesisKeyCode.Add(Keys.Divide, Key.Divide);
            noesisKeyCode.Add(Keys.Enter, Key.Return);      // same as Return

            noesisKeyCode.Add(Keys.A, Key.A);
            noesisKeyCode.Add(Keys.B, Key.B);
            noesisKeyCode.Add(Keys.C, Key.C);
            noesisKeyCode.Add(Keys.D, Key.D);
            noesisKeyCode.Add(Keys.E, Key.E);
            noesisKeyCode.Add(Keys.F, Key.F);
            noesisKeyCode.Add(Keys.G, Key.G);
            noesisKeyCode.Add(Keys.H, Key.H);
            noesisKeyCode.Add(Keys.I, Key.I);
            noesisKeyCode.Add(Keys.J, Key.J);
            noesisKeyCode.Add(Keys.K, Key.K);
            noesisKeyCode.Add(Keys.L, Key.L);
            noesisKeyCode.Add(Keys.M, Key.M);
            noesisKeyCode.Add(Keys.N, Key.N);
            noesisKeyCode.Add(Keys.O, Key.O);
            noesisKeyCode.Add(Keys.P, Key.P);
            noesisKeyCode.Add(Keys.Q, Key.Q);
            noesisKeyCode.Add(Keys.R, Key.R);
            noesisKeyCode.Add(Keys.S, Key.S);
            noesisKeyCode.Add(Keys.T, Key.T);
            noesisKeyCode.Add(Keys.U, Key.U);
            noesisKeyCode.Add(Keys.V, Key.V);
            noesisKeyCode.Add(Keys.W, Key.W);
            noesisKeyCode.Add(Keys.X, Key.X);
            noesisKeyCode.Add(Keys.Y, Key.Y);
            noesisKeyCode.Add(Keys.Z, Key.Z);

            noesisKeyCode.Add(Keys.F1, Key.F1);
            noesisKeyCode.Add(Keys.F2, Key.F2);
            noesisKeyCode.Add(Keys.F3, Key.F3);
            noesisKeyCode.Add(Keys.F4, Key.F4);
            noesisKeyCode.Add(Keys.F5, Key.F5);
            noesisKeyCode.Add(Keys.F6, Key.F6);
            noesisKeyCode.Add(Keys.F7, Key.F7);
            noesisKeyCode.Add(Keys.F8, Key.F8);
            noesisKeyCode.Add(Keys.F9, Key.F9);
            noesisKeyCode.Add(Keys.F10, Key.F10);
            noesisKeyCode.Add(Keys.F11, Key.F11);
            noesisKeyCode.Add(Keys.F12, Key.F12);
            noesisKeyCode.Add(Keys.F13, Key.F13);
            noesisKeyCode.Add(Keys.F14, Key.F14);
            noesisKeyCode.Add(Keys.F15, Key.F15);

            noesisKeyCode.Add(Keys.NumberKeyLock, Key.NumLock);
            noesisKeyCode.Add(Keys.Scroll, Key.Scroll);

            //noesisKeyCode.Add(Keys.Equals, Key.OemPlus);
            //noesisKeyCode.Add(Keys.Plus, Key.OemPlus);
            noesisKeyCode.Add(Keys.Comma, Key.OemComma);
            //noesisKeyCode.Add(Keys.Minus, Key.OemMinus);
            noesisKeyCode.Add(Keys.Period, Key.OemPeriod);
            noesisKeyCode.Add(Keys.Slash, Key.OemQuestion);

            noesisKeyCode.Add(Keys.BackSlash, Key.OemBackslash);
            //noesisKeyCode.Add(Keys.LeftBracket, Key.OemOpenBrackets);
            //noesisKeyCode.Add(Keys.RightBracket, Key.OemCloseBrackets);
            noesisKeyCode.Add(Keys.Semicolon, Key.OemSemicolon);
            //noesisKeyCode.Add(Keys.Quote, Key.OemQuotes);
        }

        public static Key Convert(Keys key)
        {
            Key noesisKey = 0;
            noesisKeyCode.TryGetValue(key, out noesisKey);

            return noesisKey;
        }
    }
}