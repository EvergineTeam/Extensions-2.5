#region File Description
//-----------------------------------------------------------------------------
// TiledMapRenderOrderType
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// The order in which tiles on tile layers are rendered. 
    /// Valid values are right-down (the default), right-up, left-down and left-up. 
    /// In all cases, the map is drawn row-by-row. 
    /// </summary>
    public enum TiledMapRenderOrderType
    {
        Right_Down,
        Right_Up,
        Left_Down,
        Left_Up
    }
}
