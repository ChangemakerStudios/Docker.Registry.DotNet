//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
//  and Docker.Registry.DotNet Contributors
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Reflection;

using Docker.Registry.DotNet.QueryParameters;

using JetBrains.Annotations;

namespace Docker.Registry.DotNet.Helpers
{
    internal static class QueryStringExtensions
    {
        /// <summary>
        /// Adds query parameters using reflection. Object must have [QueryParameter] attributes
        /// on it's properties for it to map properly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryString"></param>
        /// <param name="instance"></param>
        internal static void AddFromObjectWithQueryParameters<T>(
            this QueryString queryString,
            [NotNull] T instance)
            where T : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var propertyInfos = instance.GetType().GetProperties();

            foreach (var p in propertyInfos)
            {
                var attribute = p.GetCustomAttribute<QueryParameterAttribute>();
                if (attribute != null)
                {
                    // TODO: Use a nuget like FastMember to improve performance here or switch to static delegate generation
                    var value = p.GetValue(instance, null);
                    if (value != null)
                    {
                        queryString.Add(attribute.Key, value.ToString());
                    }
                }
            }
        }

        /// <summary>
        ///     Adds the value to the query string if it's not null.
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void AddIfNotNull<T>(this QueryString queryString, string key, T? value)
            where T : struct
        {
            if (value != null) queryString.Add(key, $"{value.Value}");
        }

        /// <summary>
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void AddIfNotEmpty(this QueryString queryString, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) queryString.Add(key, value);
        }
    }
}