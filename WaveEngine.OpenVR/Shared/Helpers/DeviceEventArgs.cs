// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Text;

namespace WaveEngine.OpenVR.Helpers
{
    public class DeviceEventArgs : EventArgs
    {
        public uint Index;
        public bool Connected;

        public DeviceEventArgs(uint index, bool connected)
        {
            this.Index = index;
            this.Connected = connected;
        }
    }
}
