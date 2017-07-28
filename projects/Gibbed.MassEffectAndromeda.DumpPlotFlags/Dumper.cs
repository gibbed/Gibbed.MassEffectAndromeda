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
using System.Linq;
using System.Text;
using Gibbed.Frostbite3.Dynamic;
using EbxInfo = Gibbed.Frostbite3.VfsFormats.Superbundle.EbxInfo;
using PartitionFile = Gibbed.Frostbite3.ResourceFormats.PartitionFile;

namespace Gibbed.MassEffectAndromeda.DumpPlotFlags
{
    internal class Dumper
    {
        #region Logger
        // ReSharper disable InconsistentNaming
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // ReSharper restore InconsistentNaming
        #endregion

        private readonly Dictionary<Guid, Dictionary<string, List<string>>> _Results;
        private readonly Dictionary<string, DumpHandlers.DumpHandler> _Handlers;

        public Dumper()
        {
            this._Results = new Dictionary<Guid, Dictionary<string, List<string>>>();
            this._Handlers = DumpHandlers.Create();
        }

        public Dictionary<Guid, Dictionary<string, List<string>>> Results
        {
            get { return this._Results; }
        }

        public void DumpPartition(EbxInfo ebxInfo, PartitionFile partition)
        {
            string[] unknownTypeNames;
            if (HasPlotFlagId(partition, out unknownTypeNames) == false)
            {
                return;
            }

            if (unknownTypeNames.Length > 0)
            {
                return;
                throw new NotSupportedException();
            }

            using (var reader = new PartitionReader(
                partition,
                typeof(PlotConditionType),
                typeof(PlotActionType),
                typeof(PlotLogicOperator)))
            {
                var allObjects = reader.GetObjects().ToArray();
                foreach (var kv in this._Handlers)
                {
                    var typeName = kv.Key;
                    var handler = kv.Value;
                    foreach (var data in reader.GetObjectsOfType(typeName))
                    {
                        handler(ebxInfo, this, typeName, data);
                    }
                }
            }
        }

        private static bool HasPlotFlagId(PartitionFile partition, out string[] unknownTypeNames)
        {
            var idTypeIndex = partition.TypeDefinitionEntries.FindIndex(tde => tde.Name == "PlotFlagId");
            if (idTypeIndex < 0)
            {
                unknownTypeNames = null;
                return false;
            }

            var referencingFieldIndices =
                partition
                    .FieldDefinitionEntries
                    .FindAllIndices(fde => fde.TypeIndex == idTypeIndex)
                    .ToArray();
            var referencingTypeIndices =
                partition
                    .TypeDefinitionEntries
                    .FindAllIndices(tde => referencingFieldIndices.Any(
                        i => tde.FieldCount > 0 &&
                             i >= tde.FieldStartIndex &&
                             i < tde.FieldStartIndex + tde.FieldCount) == true)
                    .ToArray();

            var queue = new Queue<int>();
            foreach (var referencingTypeIndex in referencingTypeIndices)
            {
                queue.Enqueue(referencingTypeIndex);
            }

            var interestingTypeIndices = new List<int>();
            while (queue.Count > 0)
            {
                var referencingTypeIndex = queue.Dequeue();
                var referencingType = partition.TypeDefinitionEntries[referencingTypeIndex];
                if (referencingType.Flags.DataType == Frostbite3.ResourceFormats.Partition.DataType.List ||
                    TypeHelpers.IsNested(referencingType.Name) == true)
                {
                    var parentFieldIndices =
                        partition
                            .FieldDefinitionEntries
                            .FindAllIndices(
                                fde => fde.TypeIndex == referencingTypeIndex)
                            .ToArray();
                    var parentTypeIndices =
                        partition
                            .TypeDefinitionEntries.FindAllIndices(
                                tde => tde.FieldCount > 0 &&
                                       parentFieldIndices.Any(
                                           i => i >= tde.FieldStartIndex &&
                                                i < tde.FieldStartIndex + tde.FieldCount) == true)
                            .ToArray();
                    foreach (var parentTypeIndex in parentTypeIndices)
                    {
                        queue.Enqueue(parentTypeIndex);
                    }
                    continue;
                }
                interestingTypeIndices.Add(referencingTypeIndex);
            }

            var interestingTypes =
                interestingTypeIndices
                    .Distinct()
                    .Select(i => partition.TypeDefinitionEntries[i])
                    .ToArray();
            var interestingTypeNames = interestingTypes.Select(tde => tde.Name).ToArray();
            unknownTypeNames = interestingTypeNames.Where(v => TypeHelpers.IsKnown(v) == false).ToArray();
            return true;
        }

        private void AddResult(EbxInfo ebxInfo, Guid guid, string line)
        {
            Dictionary<string, List<string>> resultsByPaths;
            if (this._Results.TryGetValue(guid, out resultsByPaths) == false)
            {
                this._Results[guid] = resultsByPaths = new Dictionary<string, List<string>>();
            }
            List<string> resultsByPath;
            if (resultsByPaths.TryGetValue(ebxInfo.Name, out resultsByPath) == false)
            {
                resultsByPaths[ebxInfo.Name] = resultsByPath = new List<string>();
            }
            resultsByPath.Add(line);
        }

        public void AddGuidResult(EbxInfo ebxInfo, dynamic parent, Guid guid, params string[] typeParts)
        {
            this.AddGuidResult(ebxInfo, parent, guid, null, typeParts);
        }

        public void AddGuidResult(EbxInfo ebxInfo,
                                  dynamic parent,
                                  Guid guid,
                                  Func<string> callback,
                                  params string[] typeParts)
        {
            if (typeParts.Length < 1)
            {
                throw new ArgumentOutOfRangeException("typeParts");
            }

            if (guid == Guid.Empty)
            {
                return;
            }

            var sb = new StringBuilder();
            Guid dataGuid = parent.__GUID;
            if (dataGuid != Guid.Empty)
            {
                sb.Append("[`");
                sb.Append(dataGuid);
                sb.Append("`] ");
            }
            sb.Append(string.Join(".", typeParts));
            if (callback != null)
            {
                var extra = callback();
                if (string.IsNullOrEmpty(extra) == false)
                {
                    sb.Append(": ");
                    sb.Append(extra);
                }
            }
            AddResult(ebxInfo, guid, sb.ToString());
        }

        public void AddActionResult(EbxInfo ebxInfo, dynamic parent, dynamic data, params string[] typeParts)
        {
            this.AddActionResult(ebxInfo, parent, data, null, typeParts);
        }

        public void AddActionResult(EbxInfo ebxInfo,
                                    dynamic parent,
                                    dynamic data,
                                    Func<string> callback,
                                    params string[] typeParts)
        {
            if (typeParts.Length < 1)
            {
                throw new ArgumentOutOfRangeException("typeParts");
            }

            var guid = (Guid)data.PlotFlagReference.PlotFlagId.Guid;
            if (guid == Guid.Empty)
            {
                return;
            }

            var actionType = (PlotActionType)data.ActionType;
            if (actionType == PlotActionType.Custom)
            {
                throw new NotSupportedException();
            }

            var sb = new StringBuilder();
            Guid dataGuid = parent.__GUID;
            if (dataGuid == Guid.Empty)
            {
                dataGuid = data.__GUID;
            }
            if (dataGuid != Guid.Empty)
            {
                sb.Append("[`");
                sb.Append(dataGuid);
                sb.Append("`] ");
            }
            sb.AppendFormat("{0} => {1}", string.Join(".", typeParts), actionType);
            if (callback != null)
            {
                var extra = callback();
                if (string.IsNullOrEmpty(extra) == false)
                {
                    sb.Append(": ");
                    sb.Append(extra);
                }
            }
        }

        public void AddConditionResult(EbxInfo ebxInfo, dynamic parent, dynamic data, params string[] typeParts)
        {
            this.AddConditionResult(ebxInfo, parent, data, null, typeParts);
        }

        public void AddConditionResult(EbxInfo ebxInfo,
                                       dynamic parent,
                                       dynamic data,
                                       Func<string> callback,
                                       params string[] typeParts)
        {
            if (typeParts.Length < 1)
            {
                throw new ArgumentOutOfRangeException("typeParts");
            }

            var guid = (Guid)data.PlotFlagReference.PlotFlagId.Guid;
            if (guid == Guid.Empty)
            {
                return;
            }

            var conditionType = (PlotConditionType)data.ConditionType;
            var desiredValue = (bool)data.DesiredValue;

            if (conditionType == PlotConditionType.Expression || conditionType == PlotConditionType.Custom)
            {
                throw new NotSupportedException();
            }

            var sb = new StringBuilder();
            Guid dataGuid = parent.__GUID;
            if (dataGuid == Guid.Empty)
            {
                dataGuid = data.__GUID;
            }
            if (dataGuid != Guid.Empty)
            {
                sb.Append("[`");
                sb.Append(dataGuid);
                sb.Append("`] ");
            }
            sb.AppendFormat("{0} => {1}, {2}", string.Join(".", typeParts), conditionType, desiredValue);
            if (callback != null)
            {
                var extra = callback();
                if (string.IsNullOrEmpty(extra) == false)
                {
                    sb.Append(": ");
                    sb.Append(extra);
                }
            }
            this.AddResult(ebxInfo, guid, sb.ToString());
        }
    }
}
