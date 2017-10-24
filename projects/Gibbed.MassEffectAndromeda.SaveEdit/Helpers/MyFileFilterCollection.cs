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
using System.Globalization;
using System.Linq;

namespace Gibbed.MassEffectAndromeda.SaveEdit
{
    internal class MyFileFilterCollection
    {
        private readonly List<MyFilterBuilder> _Filters = new List<MyFilterBuilder>();
        private MyFilterBuilder _DefaultFilterBuilder;

        public string DefaultFilter { get; private set; }

        /// <summary>
        /// Adds a filter for the given <paramref name="filter"/>. By default, the first filter will be the default in the dialog
        /// </summary>
        /// <param name="filter">the file extension</param>
        /// <param name="isDefault">sets this filter as the default filter in the dialog</param>
        /// <example>AddFilter("xml")</example>
        /// <returns></returns>
        public MyFilterBuilder AddFilter(string filter, bool isDefault = false)
        {
            var filterBuilder = new MyFilterBuilder(this, filter);
            this._Filters.Add(filterBuilder);

            if (isDefault == true)
            {
                this._DefaultFilterBuilder = filterBuilder;
            }

            if (string.IsNullOrEmpty(this.DefaultFilter) == true || isDefault)
            {
                this.DefaultFilter = filter;
            }

            return filterBuilder;
        }

        /// <summary>
        /// Adds a filter for all files, i.e *.*
        /// </summary>
        /// <returns></returns>
        public MyFileFilterCollection AddAllFilesFilter(bool isDefault = false)
        {
            var filterBuilder = this.AddFilter("*.*", isDefault);
            filterBuilder.WithDescription("All Files");
            return this;
        }

        /// <summary>
        /// Adds a filter for the list of given <paramref name="filters"/>
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public MyFilterBuilder AddFilter(params string[] filters)
        {
            var filterBuilder = new MyFilterBuilder(this, filters);
            this._Filters.Add(filterBuilder);
            return filterBuilder;
        }

        /// <summary>
        /// Creates the filter expression for the dialog. If no filter was added, a filter expression for alles files will be returned.
        /// </summary>
        /// <returns></returns>
        public string CreateFilterExpression()
        {
            if (this._Filters.Any() == false)
            {
                return "All Files (*.*)|*.*";
            }

            return string.Join("|",
                               this._Filters.Select(x => string.Format(
                                   CultureInfo.InvariantCulture,
                                   "{0}|{1}",
                                   x.Description,
                                   x.FilterExpression)));
        }

        public int GetFilterIndex()
        {
            if (this._Filters.Any() == false ||
                this._DefaultFilterBuilder == null)
            {
                return 0;
            }

            var index = this._Filters.IndexOf(this._DefaultFilterBuilder);
            if (index < 0)
            {
                return 0;
            }

            return 1 + index;
        }

        public class MyFilterBuilder
        {
            public MyFilterBuilder(MyFileFilterCollection fileFilterCollection, params string[] filters)
            {
                if (fileFilterCollection == null)
                {
                    throw new ArgumentNullException("fileFilterCollection");
                }

                if (filters.Any() == false || filters.All(string.IsNullOrWhiteSpace))
                {
                    throw new ArgumentException("you must specify at least one filter");
                }

                this.FileFilterCollection = fileFilterCollection;
                this.Filters = filters;
            }

            internal MyFileFilterCollection FileFilterCollection { get; private set; }
            internal string[] Filters { get; private set; }
            internal string Description { get; private set; }

            internal string FilterExpression
            {
                get { return string.Join(";", this.Filters); }
            }

            /// <summary>
            /// Sets the descripion of the filter. File extensions are automatically added to the description unless <paramref name="appendExtensions"/> is false.
            /// </summary>
            /// <param name="description">the description</param>
            /// <param name="appendExtensions">append the extension(s) to the description?</param>
            /// <returns></returns>
            public MyFileFilterCollection WithDescription(string description, bool appendExtensions = true)
            {
                this.Description = description;

                if (appendExtensions == true)
                {
                    this.Description = this.AppendExtensions(Description);
                }

                return this.FileFilterCollection;
            }

            /// <summary>
            /// Appends the extensions to a string
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            private string AppendExtensions(string s)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", s, this.FilterExpression);
            }
        }
    }
}
