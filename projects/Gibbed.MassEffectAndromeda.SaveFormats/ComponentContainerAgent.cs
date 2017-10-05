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
using System.Collections.Generic;
using Gibbed.MassEffectAndromeda.FileFormats;

namespace Gibbed.MassEffectAndromeda.SaveFormats
{
    public abstract class ComponentContainerAgent<TType, TAttribute> : IComponentContainerAgent
        where TType : class, IComponent
        where TAttribute : Attribute, IGameTypeAttribute
    {
        public abstract string AgentName { get; }

        public ushort AgentVersion
        {
            get { return 2; }
        }

        private abstract class ComponentFactory : GameTypeFactory<TType, TAttribute>
        {
        }

        public class ComponentContainer
        {
            private bool _Unknown1;
            private string _Unknown2;
            private readonly List<TType> _Components;

            public ComponentContainer()
            {
                this._Components = new List<TType>();
            }

            public bool Unknown1
            {
                get { return this._Unknown1; }
                set { this._Unknown1 = value; }
            }

            public string Unknown2
            {
                get { return this._Unknown2; }
                set { this._Unknown2 = value; }
            }

            public List<TType> Components
            {
                get { return this._Components; }
            }
        }

        #region Fields
        private readonly Dictionary<uint, ComponentContainer> _ComponentContainers;
        #endregion

        protected ComponentContainerAgent()
        {
            this._ComponentContainers = new Dictionary<uint, ComponentContainer>();
        }

        #region Properties
        public Dictionary<uint, ComponentContainer> ComponentContainers
        {
            get { return this._ComponentContainers; }
        }
        #endregion

        public virtual void Read(IBitReader reader, ushort version)
        {
            this._ComponentContainers.Clear();

            if (version == 2)
            {
                var componentContainerVersion = reader.ReadUInt16();

                reader.PushFrameLength(24);

                var componentContainerCount = reader.ReadUInt16();
                for (int i = 0; i < componentContainerCount; i++)
                {
                    reader.PushFrameLength(24);

                    var componentContainerId = reader.ReadUInt32();
                    ComponentContainer componentContainer = null;

                    var hasComponents = reader.ReadBoolean();
                    if (hasComponents == true)
                    {
                        componentContainer = new ComponentContainer();

                        if (componentContainerVersion >= 2)
                        {
                            componentContainer.Unknown1 = reader.ReadBoolean();
                        }

                        if (componentContainerVersion >= 3)
                        {
                            componentContainer.Unknown2 = reader.ReadString();
                        }

                        var componentCount = reader.ReadUInt16();
                        for (int j = 0; j < componentCount; j++)
                        {
                            reader.PushFrameLength(24);

                            var componentNameHash = reader.ReadUInt32();

                            var component = ComponentFactory.Create(componentNameHash);
                            if (component == null)
                            {
                                throw new InvalidOperationException();
                            }

                            component.Read(reader, componentContainerVersion);
                            componentContainer.Components.Add(component);

                            reader.PopFrameLength();
                        }
                    }

                    this._ComponentContainers.Add(componentContainerId, componentContainer);

                    reader.PopFrameLength();
                }

                reader.PopFrameLength();
            }
        }

        public void Write(IBitWriter writer)
        {
            const ushort componentContainerVersion = 5;
            writer.WriteUInt16(componentContainerVersion);

            writer.PushFrameLength(24);

            writer.WriteUInt16((ushort)this._ComponentContainers.Count);
            foreach (var kv in this._ComponentContainers)
            {
                var componentContainer = kv.Value;

                writer.PushFrameLength(24);

                writer.WriteUInt32(kv.Key);
                writer.WriteBoolean(componentContainer != null);

                if (componentContainer != null)
                {
                    writer.WriteBoolean(componentContainer.Unknown1);
                    writer.WriteString(componentContainer.Unknown2);

                    writer.WriteUInt16((ushort)componentContainer.Components.Count);
                    foreach (var component in componentContainer.Components)
                    {
                        writer.PushFrameLength(24);
                        var componentNameHash = ComponentFactory.GetNameHash(component.ComponentName);
                        writer.WriteUInt32(componentNameHash);
                        component.Write(writer, componentContainerVersion);
                        writer.PopFrameLength();
                    }
                }

                writer.PopFrameLength();
            }

            writer.PopFrameLength();
        }
    }
}
