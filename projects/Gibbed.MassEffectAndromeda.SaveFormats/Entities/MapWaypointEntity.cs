/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using Gibbed.MassEffectAndromeda.FileFormats;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Entities
{
    // 0x88FB34 : LogicPrefabReferenceObjectData
    // 0x7B1798 : SpatialPrefabReferenceObjectData
    // 0x3F75DB : MapWaypointManagerEntityData
    [Entity(0xF48A973B)]
    public class MapWaypointEntity : Entity
    {
        #region Fields
        private Vector3? _CurrentWaypoint;
        #endregion

        #region Properties
        public Vector3? CurrentWaypoint
        {
            get { return this._CurrentWaypoint; }
            set { this._CurrentWaypoint = value; }
        }
        #endregion

        internal override void Read0(IBitReader reader)
        {
            base.Read0(reader);
            throw new NotImplementedException();
        }

        internal override void Read1(IBitReader reader)
        {
            base.Read1(reader);
            var hasWaypoint = reader.ReadBoolean();
            if (hasWaypoint == true)
            {
                this._CurrentWaypoint = reader.ReadVector3();
            }
        }
    }
}
