#region File Description
//-----------------------------------------------------------------------------
// TiledMapOrientationType
//
// Copyright © 2014 Wave Corporation
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
    /// Map orientation. Tiled supports "orthogonal", "isometric" and "staggered" (since 0.9.0) at the moment.
    /// </summary>
    public enum TiledMapOrientationType
    {
        Orthogonal,
        Isometric,
        Staggered
    }
}